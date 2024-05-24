using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;

namespace MyApp.Namespace
{
    public class RateModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public RateModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int SelectedChallengeId { get; set; }

        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string Comment { get; set; }

        public IList<Challenge> AvailableChallenges { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AvailableChallenges = await _context.Challenges.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.Identity.Name;

            var userRate = new UserRate
            {
                UserId = userId,
                ChallengeId = SelectedChallengeId,
                Rate = Rating,
                Comment = Comment, 
            };

            _context.UserRates.Add(userRate);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ChallengeStatistics");
        }
    }
}
