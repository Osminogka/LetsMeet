using LetsMeet.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LetsMeet.Pages
{
    public class UserInfoModel : PageModel
    {
        public DataContext Context;

        public string SelectedUser = string.Empty;
        public List<Record> RecordsList = new List<Record>();

        public UserInfoModel(DataContext ctx)
        {
            Context = ctx;
        }

        public void OnGet(string user)
        {
            string LocalUserName = HttpContext.User.Identity.Name;
            List<UserFriendList> checkIfFriends = Context.userFriendLists.Where(obj => (obj.FriendUserName == LocalUserName && obj.MainUserName == user)
                || (obj.MainUserName == LocalUserName && obj.FriendUserName == user)).ToList();

            if (!checkIfFriends.Any())
                return;

            SelectedUser = user;

            RecordsList = Context.Records.Where(obj => ((obj.CreaterUserName == LocalUserName && obj.RelatedUserName == user) 
                || (obj.CreaterUserName == user && obj.RelatedUserName == LocalUserName))).ToList();
        }
    }
}
