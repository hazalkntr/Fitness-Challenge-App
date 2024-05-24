using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Pages
{
    public class YourChallengesModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public YourChallengesModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        public List<ChallengeWithJoinStatus> UserChallenges { get; set; } = new List<ChallengeWithJoinStatus>();

        public class ChallengeWithJoinStatus
        {
            public Challenge Challenge { get; set; }
            public List<LeaderboardViewModel> Leaderboard { get; set; } = new List<LeaderboardViewModel>();
            public int UserStreakCount { get; set; }
        }

        public class LeaderboardViewModel
        {
            public int Rank { get; set; }
            public string Username { get; set; } = "";
            public double Score { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var joinedChallenges = await _context.ChallengeParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ChallengeId)
                .ToListAsync();

            UserChallenges = await _context.Challenges
                .Where(c => joinedChallenges.Contains(c.ChallengeId))
                .Select(c => new ChallengeWithJoinStatus
                {
                    Challenge = c,
                    /*UserStreakCount = _context.ChallengeParticipants
                        .Where(cp => cp.ChallengeId == c.ChallengeId && cp.UserId == userId)
                        .Select(cp => cp.StreakCount)
                        .FirstOrDefault(),*/
                    Leaderboard = _context.Leaderboards
                        .Where(l => l.ChallengeId == c.ChallengeId)
                        .OrderBy(l => l.Rank)
                        .Select(l => new LeaderboardViewModel
                        {
                            Rank = l.Rank,
                            Username = _context.Users.FirstOrDefault(u => u.UserId == l.UserId).Username,
                            Score = l.Score
                        })
                        .ToList()
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStreakAsync(int challengeId, bool performedToday)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var participant = await _context.ChallengeParticipants
                .FirstOrDefaultAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);

            if (participant == null)
            {
                return NotFound();
            }

            if (performedToday)
            {
                //participant.StreakCount++;
                var leaderboardEntry = await _context.Leaderboards
                    .FirstOrDefaultAsync(l => l.ChallengeId == challengeId && l.UserId == userId);

                if (leaderboardEntry == null)
                {
                    leaderboardEntry = new Leaderboard
                    {
                        ChallengeId = challengeId,
                        UserId = userId,
                        Score = 1,
                        Rank = _context.Leaderboards.Count(l => l.ChallengeId == challengeId) + 1
                    };
                    _context.Leaderboards.Add(leaderboardEntry);
                }
                else
                {
                    leaderboardEntry.Score++;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
