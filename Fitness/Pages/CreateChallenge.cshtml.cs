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

            //the start date cant be earlier than the current date
            if (Challenge.StartDate < DateTime.Now)
            {
                ModelState.AddModelError("Challenge.StartDate", "Start date cannot be earlier than the current date and hour.");
                return Page();
            }

            //end date must be later than the start date -if entered
            if (Challenge.EndDate.HasValue && Challenge.EndDate.Value <= Challenge.StartDate.AddHours(1))
            {
                ModelState.AddModelError(nameof(Challenge.EndDate), "End date must be at least 1 hour later than the start date.");
                return Page();
            }

            Challenge.IsDeleted = IsDeleted;

            _context.Challenges.Add(Challenge);
            await _context.SaveChangesAsync();

            return RedirectToPage("AvailableChallenges");
        }
    }
}
