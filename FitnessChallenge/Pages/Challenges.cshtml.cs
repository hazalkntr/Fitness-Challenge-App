using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MyApp.Namespace
{
    [Authorize]
    public class ChallengesModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
