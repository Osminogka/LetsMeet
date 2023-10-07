using LetsMeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace LetsMeet.Controllers
{
    [ApiController]
    [Route("/api/info")]
    public class UserInfoController: ControllerBase
    {
        public DataContext context;
        public UserManager<IdentityUser> UserManager;

        public UserInfoController(DataContext ctx, UserManager<IdentityUser> usrManager)
        {
            context = ctx;
            UserManager = usrManager;
        }

        [HttpGet("/api/friends")]
        public async Task<List<string>> GetFriends()
        {
            string UserId = Request.Headers["UserId"];

            var FriendList = context.userFriendLists.Where(obj => obj.MainUserId == UserId || obj.FriendUserId == UserId).
                Select(obj => obj.FriendUserId == UserId ? obj.MainUserId : obj.FriendUserId);
            return FriendList.ToList();
        }

        [HttpGet("/api/group")]
        public List<string> GetGroups()
        {
            string UserId = Request.Headers["UserId"];
            var GroupsList = context.GroupRecords.Where(obj => obj.UserIdThatBelongsToGroup == UserId).Select(obj => obj.GroupNameThatRecordBelong);

            return GroupsList.ToList();
        }

        [HttpGet("/api/invite")]
        public List<FriendInvite> GetInvites()
        {
            string UserId = Request.Headers["UserName"];
            var InviteList = context.InviteList.Where(obj => obj.FriendUserName == UserId);

            return InviteList.ToList();
        }
    }
}
