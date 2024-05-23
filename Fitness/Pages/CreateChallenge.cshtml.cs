using System;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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

        [BindProperty]
        public string Category { get; set; } 
        
        [BindProperty]
        public string DifficultyLevel { get; set; } 
        
        [BindProperty]
        public string Instructions { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is not valid:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
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

            // Get the current user's ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // User is not authenticated
            }

            var userId = userIdClaim.Value;
            Challenge.UserId = userId;
            Challenge.IsDeleted = IsDeleted;
            Challenge.Category = Category;
            Challenge.DifficultyLevel = DifficultyLevel;
            Challenge.Instructions = Instructions;

            _context.Challenges.Add(Challenge);
            await _context.SaveChangesAsync();

            return RedirectToPage("AvailableChallenges");
        }
    }
}
