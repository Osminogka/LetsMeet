using LetsMeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsMeet.Controllers
{
    [ApiController]
    [Route("/api/table")]
    [Authorize(AuthenticationSchemes = "Identity.Application, Bearer")] 
    public class TableController: ControllerBase
    {
        public DataContext context;

        public TableController(DataContext ctx)
        {
            context = ctx;
        }

        [HttpGet("maintable")]
        public async Task<string> GetRecords()
        {
            List<Record>? records = new List<Record>();

            string UserId = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"] ?? "No cookies";

            return UserId;
        }
    }
}
