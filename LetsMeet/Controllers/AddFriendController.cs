using LetsMeet.Models;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetsMeet.Controllers
{
    public class AddFriendController : ControllerBase
    {
        public UserManager<IdentityUser> UserManager;
        public DataContext Context;

        public AddFriendController(UserManager<IdentityUser> userManager, DataContext dataContext)
        {
            UserManager = userManager;
            Context = dataContext;
        }

        [HttpPost("/api/addfriend")]
        public async Task<IActionResult> AddFriend()
        {
            string LocalUserId = Request.Headers["LocalUserId"];
            string FriendUserName = Request.Headers["FriendUserName"];
            IdentityUser tempFriend = await UserManager.FindByNameAsync(FriendUserName);

            try
            {
                UserFriendList tempRecord = Context.userFriendLists.Where(obj => (obj.MainUserId == LocalUserId && obj.FriendUserId == tempFriend.Id)
                        || (obj.MainUserId == tempFriend.Id && obj.FriendUserId == LocalUserId)).First();

                if (tempRecord != null)
                    return StatusCode(400);

                IdentityUser User = await UserManager.FindByIdAsync(LocalUserId);

                if (User == null)
                    return StatusCode(400);

                FriendInvite invite = new FriendInvite
                {
                    MainUserId = LocalUserId,
                    FriendUserName = FriendUserName
                };

                FriendInvite? check = Context.InviteList.SingleOrDefault(obj => obj.MainUserId == LocalUserId && obj.FriendUserName == FriendUserName);

                if (check != null)
                    return StatusCode(400);

                await Context.InviteList.AddAsync(invite);
                await Context.SaveChangesAsync();

                return StatusCode(200);
            }
            catch(Exception ex)
            {
                return StatusCode(401);
            }
        }

        [HttpPost("/api/acceptinvite")]
        public async Task<IActionResult> AcceptInvite()
        {
            string LocalUserId = Request.Headers["LocalUserId"];
            string FriendUserName = Request.Headers["FriendUserName"];
            string InviteInfo = Request.Headers["Inviteinfo"];

            try
            {
                IdentityUser tempFriend = await UserManager.FindByNameAsync(FriendUserName);
                IdentityUser tempLocalUser = await UserManager.FindByIdAsync(LocalUserId);

                if (InviteInfo == "remove")
                {
                    UserFriendList tempRecord = Context.userFriendLists.Where(obj => (obj.MainUserId == LocalUserId && obj.FriendUserId == tempFriend.Id)
                        || (obj.MainUserId == tempFriend.Id && obj.FriendUserId == LocalUserId)).First();

                    Context.userFriendLists.Remove(tempRecord);
                    await Context.SaveChangesAsync();
                    return StatusCode(500);
                }

                FriendInvite inviteTemp = Context.InviteList.Where(obj => obj.MainUserId == tempFriend.Id).Where(obj => obj.FriendUserName == tempLocalUser.UserName).First();

                Context.InviteList.Remove(inviteTemp);

                if (InviteInfo == "reject")
                {
                    await Context.SaveChangesAsync();
                    return StatusCode(500);
                }

                UserFriendList tempFriendRecord = new UserFriendList
                {
                    MainUserId = LocalUserId,
                    FriendUserId = tempFriend.Id
                };

                await Context.userFriendLists.AddAsync(tempFriendRecord);

                await Context.SaveChangesAsync();
                return StatusCode(500);
            }
            catch(Exception ex)
            {
                return StatusCode(401);
            }
        }
    }
}
