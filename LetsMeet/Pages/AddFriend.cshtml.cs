using LetsMeet.Models;
using LetsMeet.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LetsMeet.Pages
{
    public class AddFriendModel : UserPageModel
    {
        public IdentityUser User;
        public UserManager<IdentityUser> UserManager;
        public DataContext Context;
        public UserContext ContextUser;


        public List<string> UsersNames = new List<string>();


        [BindProperty]
        public string FriendUserName { get; set; } = string.Empty;

        public AddFriendModel(UserManager<IdentityUser> userManager, DataContext ctx, UserContext ctx2)
        {
            UserManager = userManager;
            Context = ctx;
            ContextUser = ctx2;
        }

        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalItems { get; set; } = 0;
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
            }
        }

        public async Task OnGetAsync(string? username, int? tpage)
        {
            if (!string.IsNullOrEmpty(username))
            {
                User tempUser = ContextUser.Users.SingleOrDefault(obj => obj.UserName == username);
                if (tempUser == null)
                    return;

                UsersNames.Add(username);
                TotalItems = 1;
                return;
            }

            if (tpage.HasValue)
            {
                CurrentPage = tpage.Value;
            }

            TotalItems = await GetTotalUsers();
            UsersNames = await GetPagedUser();
        }

        private async Task<List<string>> GetPagedUser()
        {
            //User = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);
            //UsersNames = ContextUser.Users.Select(obj => obj.UserName).Where(obj => obj != User.UserName).ToList();
            List<string> FinalList = new List<string>();

            int startIndex = (CurrentPage - 1) * ItemsPerPage;
            for (int i = startIndex; i < ItemsPerPage + startIndex; i++)
            {
                if (i >= 0 && i < UsersNames.Count)
                    FinalList.Add(UsersNames[i]);
                else
                    break;
            }

            return FinalList;
        }

        private async Task<int> GetTotalUsers()
        {
            string UserName = HttpContext.User.Identity.Name;
            UsersNames = ContextUser.Users.Select(obj => obj.UserName).Where(obj => obj != UserName).ToList();
            List<string> TempFriendList = Context.userFriendLists.Where(obj => obj.MainUserName == UserName || obj.FriendUserName == UserName)
                .Select(obj => obj.MainUserName == UserName ? obj.FriendUserName : obj.MainUserName).ToList();

            foreach (string fName in TempFriendList)
                UsersNames.Remove(fName);

            return UsersNames.Count;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser LocalUser = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            try
            {
                var tempRecord = Context.userFriendLists.Where(obj => (obj.MainUserName == LocalUser.UserName && obj.FriendUserName == FriendUserName)
                        || (obj.MainUserName == FriendUserName && obj.FriendUserName == LocalUser.UserName));

                if (tempRecord.Any())
                    return Redirect("~/");

                FriendInvite invite = new FriendInvite
                {
                    MainUserName = LocalUser.UserName,
                    FriendUserName = FriendUserName
                };

                FriendInvite? check = Context.InviteList.SingleOrDefault(obj => obj.MainUserName == LocalUser.UserName && obj.FriendUserName == FriendUserName);

                if (check != null)
                    return Redirect("~/");

                await Context.InviteList.AddAsync(invite);
                await Context.SaveChangesAsync();

                return Redirect("~/");
            }
            catch (Exception ex)
            {
                return Redirect("~/");
            }
        }
    }
}
