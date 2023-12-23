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


        //Errors
        public string GroupCreateError { get; set; }

        public CreateGroupModel(UserManager<IdentityUser> userManager, DataContext ctx)
        {
            UserManager = userManager;
            Context = ctx;
        }

        public async Task OnGetAsync()
        {

        }

        [BindProperty]
        public string GroupName { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if(GroupName == string.Empty)
            {
                ModelState.AddModelError("GroupCreateError", "You cannot create group without name");
                return Page();
            }

            string LocalUserName = HttpContext.User.Identity.Name;
            var GroupTest = Context.Groups.SingleOrDefault(g => g.GroupName == GroupName);

            if (GroupTest != null)
            {
                ModelState.AddModelError("GroupCreateError", "Group with such name already exist");
                return Page();
            }

            Group temporaryGroupObject = new Group
            {
                CreatorName = LocalUserName,
                GroupName = GroupName
            };

            GroupRecords temporaryGroupRecord = new GroupRecords
            {
                UserName = LocalUserName,
                GroupName = GroupName
            };

            await Context.Groups.AddAsync(temporaryGroupObject);
            await Context.GroupRecords.AddAsync(temporaryGroupRecord);

            await Context.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
