namespace LetsMeet.Models.ViewModels
{
    public class UsersListViewModel
    {
        public IEnumerable<string> Users { get; set; }
            = Enumerable.Empty<string>();
        public PagingInfo PagingInfo { get; set; } = new();
    }
}
