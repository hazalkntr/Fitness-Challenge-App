using System;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fitness.Pages
{
    public class CreateChallengeModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public CreateChallengeModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Challenge Challenge { get; set; }

        [BindProperty]
        public bool IsDeleted { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Map the IsDeleted boolean to the Challenge entity
            Challenge.IsDeleted = IsDeleted;

            _context.Challenges.Add(Challenge);
            await _context.SaveChangesAsync();

            return RedirectToPage("AvailableChallenges");
        }
    }
}
