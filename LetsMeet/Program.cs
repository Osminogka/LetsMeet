using Microsoft.EntityFrameworkCore;
using LetsMeet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();


builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:NotesConnection"]);  
    opts.EnableSensitiveDataLogging(true);
});
builder.Services.AddDbContext<IdentityContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]));
builder.Services.AddDbContext<UserContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:UsersConnection"]);
    opts.EnableSensitiveDataLogging(true);
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>(); 
builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = true;
    opts.Password.RequireLowercase = true;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireDigit = true;
    opts.User.RequireUniqueEmail = true;
    opts.User.AllowedUserNameCharacters = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
});


builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(opts =>
{
    opts.LoginPath = "/login";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
    options.IdleTimeout = TimeSpan.FromMinutes(20)
    
);

var app = builder.Build();

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

IdentitySeedData.CreateRole(app.Services, app.Configuration);

app.Run();
