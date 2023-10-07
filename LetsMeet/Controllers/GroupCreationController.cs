using LetsMeet.Migrations.User;
using LetsMeet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetsMeet.Controllers
{
    public class GroupCreationController: ControllerBase
    {
        public UserManager<IdentityUser> UserManager;
        public DataContext Context;

        public GroupCreationController(UserManager<IdentityUser> userManager, DataContext dataContext)
        {
            UserManager = userManager;
            Context = dataContext;
        }

        [HttpPost("/api/creategroup")]
        public async Task<GroupCreationStatus> CreateGroup()
        {
            string GroupName = Request.Headers["GroupName"];
            string UserId = Request.Headers["UserId"];

            IdentityUser? User = await UserManager.FindByIdAsync(UserId);

            if (User == null)
                return GroupCreationStatus.UserIdDoesntExit;

            var GroupTest = Context.Groups.Where(g => g.GroupName == GroupName);

            if (GroupTest.ToList().Count > 0)
                return GroupCreationStatus.GroupNameExist;

            Group temporaryGroupObject = new Group
            {
                CreatorId = UserId,
                GroupName = GroupName
            };

            GroupRecords temporaryGroupRecord = new GroupRecords
            {
                UserIdThatBelongsToGroup = UserId,
                GroupNameThatRecordBelong = GroupName
            };

            await Context.Groups.AddAsync(temporaryGroupObject);
            await Context.GroupRecords.AddAsync(temporaryGroupRecord);
            await Context.SaveChangesAsync();

            return GroupCreationStatus.Ok;
        }

        [HttpGet("/api/groupinfo")]
        public async Task<List<string>> GroupInfo()
        {
            var GroupList = Context.Groups.Select(obj => obj.GroupName);

            return GroupList.ToList();
        }

        [HttpPost("/api/joingroup")]
        public async Task<GroupCreationStatus> JoinGroup()
        {
            string GroupName = Request.Headers["GroupName"];
            string UserId = Request.Headers["LocalUserId"];

            GroupRecords? check = Context.GroupRecords.SingleOrDefault(obj => obj.GroupNameThatRecordBelong == GroupName && obj.UserIdThatBelongsToGroup == UserId);

            if (check == null)
                return GroupCreationStatus.Ok;

            GroupRecords temporaryGroupRecord = new GroupRecords
            {
                UserIdThatBelongsToGroup = UserId,
                GroupNameThatRecordBelong = GroupName
            };

            await Context.GroupRecords.AddAsync(temporaryGroupRecord);
            await Context.SaveChangesAsync();

            return GroupCreationStatus.Ok;
        }

    }
}
