using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace LetsMeet.Pages.Account
{
    [Authorize(Roles = "User")]
    public class UserPageModel: PageModel
    {
    }
}
