using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Pages
{
    public class ViewLeaderboardModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public ViewLeaderboardModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ChallengeId { get; set; }

        public Challenge? Challenge { get; set; }
        public List<LeaderboardViewModel> Leaderboard { get; set; } = new List<LeaderboardViewModel>();

        public class LeaderboardViewModel
        {
            public int Rank { get; set; }
            public string Username { get; set; } = "";
            public double Score { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ChallengeId == 0)
            {
                return NotFound();
            }

            Challenge = await _context.Challenges.FindAsync(ChallengeId);

            if (Challenge == null)
            {
                return NotFound();
            }

            Leaderboard = await _context.Leaderboards
                .Where(l => l.ChallengeId == ChallengeId)
                .OrderBy(l => l.Rank)
                .Select(l => new LeaderboardViewModel
                {
                    Rank = l.Rank,
                    Username = GetUserUsername(l.UserId),
                    Score = l.Score
                })
                .ToListAsync();

            return Page();
        }

        // Make the GetUserUsername method static
        private static string GetUserUsername(string userId)
        {
            // Access the database context statically
            using var context = new FitnessChallengeContext();
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null ? user.Username : "";
        }

    }
}
