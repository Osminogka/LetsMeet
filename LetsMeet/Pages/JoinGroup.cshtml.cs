using LetsMeet.Models;
using LetsMeet.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LetsMeet.Pages
{
    public class JoinGroupModel : UserPageModel
    {
        public DataContext Context;
        public UserManager<IdentityUser> UserManager;

        public JoinGroupModel(DataContext ctx, IHttpClientFactory httpClientFactory, UserManager<IdentityUser> userManager)
        {
            UserManager = userManager;
            Context = ctx;
        }

        public List<string> GroupNames = new List<string>();

        [BindProperty]
        public string GroupName { get; set; } = String.Empty;

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


        public async Task OnGetAsync(int? tpage, string? groupname)
        {
            if (!string.IsNullOrEmpty(groupname))
            {
                GroupRecords tempUser = Context.GroupRecords.SingleOrDefault(obj => obj.GroupName == groupname && obj.UserName != HttpContext.User.Identity.Name);
                if (tempUser != null)
                    return;


                GroupNames.Add(groupname);
                TotalItems = 1;
                return;
            }
            if (tpage.HasValue)
                CurrentPage = tpage.Value;

            TotalItems = await GetTotalGroups();
            GroupNames = await GetPagedGroups();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser TempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            GroupRecords? check = Context.GroupRecords.SingleOrDefault(obj => obj.GroupName == GroupName && obj.UserName == TempUserLocal.UserName);

            if (check != null)
                return StatusCode(400);

            GroupRecords temporaryGroupRecord = new GroupRecords
            {
                UserName = TempUserLocal.UserName,
                GroupName = GroupName
            };

            await Context.GroupRecords.AddAsync(temporaryGroupRecord);
            await Context.SaveChangesAsync();

            return Redirect("~/");
        }


        //////////////////////////HELPING MEHOTD//////////////////////////

        private async Task<int> GetTotalGroups()
        {
            IdentityUser tempUserLocal = await UserManager.FindByNameAsync(HttpContext.User.Identity.Name);

            List<string> GroupList = Context.Groups.Select(obj => obj.GroupName).ToList();
            foreach (string tempG in GroupList)
            {
                var checkIfBelongTo = Context.GroupRecords.SingleOrDefault(obj => obj.UserName == tempUserLocal.UserName && obj.GroupName == tempG);
                if (checkIfBelongTo == null)
                    GroupNames.Add(tempG);
            }

            return GroupNames.Count;
        }

        private async Task<List<string>> GetPagedGroups()
        {
            List<string> FinalList = new List<string>();

            int startIndex = (CurrentPage - 1) * ItemsPerPage;
            for (int i = startIndex; i < ItemsPerPage + startIndex; i++)
            {
                if (i >= 0 && i < GroupNames.Count)
                    FinalList.Add(GroupNames[i]);
                else
                    break;
            }

            return FinalList;
        }
    }
}
