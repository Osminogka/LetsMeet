using Microsoft.EntityFrameworkCore;

namespace LetsMeet.Models
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
            : base(opts) { }

        public DbSet<Record> Records => Set<Record>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<UserInfo> UserInfos => Set<UserInfo>();
        public DbSet<GroupRecords> GroupRecords => Set<GroupRecords>();
        public DbSet<UserFriendList> userFriendLists => Set<UserFriendList>();
        public DbSet<FriendInvite> InviteList => Set<FriendInvite>();
    }
}
