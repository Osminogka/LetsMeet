using LetsMeet.Models;
using LetsMeet.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LetsMeet.Pages
{
    public class CreateGroupModel : UserPageModel
    {
        public IdentityUser User;
        public UserManager<IdentityUser> UserManager;
        public DataContext Context;

        public CreateGroupModel(UserManager<IdentityUser> userManager, DataContext ctx, IHttpClientFactory httpClientFactory)
        {
            UserManager = userManager;
            Context = ctx;
        }

        public async Task OnGetAsync()
        {
            User = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }

        [BindProperty]
        public string GroupName { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser TempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var GroupTest = Context.Groups.SingleOrDefault(g => g.GroupName == GroupName);

            if (GroupTest != null)
                return StatusCode(400);

            Group temporaryGroupObject = new Group
            {
                CreatorName = TempUserLocal.UserName,
                GroupName = GroupName
            };

            GroupRecords temporaryGroupRecord = new GroupRecords
            {
                UserName = TempUserLocal.UserName,
                GroupName = GroupName
            };

            await Context.Groups.AddAsync(temporaryGroupObject);
            await Context.GroupRecords.AddAsync(temporaryGroupRecord);

            await Context.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
