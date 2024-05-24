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

            foreach (var challengeWithJoinStatus in UserChallenges)
            {
                
                var participant = await _context.ChallengeParticipants
                    .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.ChallengeId == challengeWithJoinStatus.Challenge.ChallengeId);

                if (participant != null)
                {
                    participant.StreakCount = participant.StreakCount ?? 0;

                    double score = CalculateScore(participant.StreakCount.Value);

                    var currentUserLeaderboardEntry = challengeWithJoinStatus.Leaderboard
                        .FirstOrDefault(l => l.Username == participant.UserId);

                    if (currentUserLeaderboardEntry != null)
                    {
                        currentUserLeaderboardEntry.Score = score;
                    }
                    else
                    {
                        string usernameOrEmail = GetUserUsername(participant.UserId);
                        Console.WriteLine("HEREEEEEE");
                            Console.WriteLine("AAAAAAAAAAA........");

                        Console.WriteLine($"USERNAME : {usernameOrEmail}");

                        //new entry for the current user
                        challengeWithJoinStatus.Leaderboard.Add(new LeaderboardViewModel
                        {
                            
                            Rank = 0, //temporarily 0 .
                            Username = participant.UserId,
                            Score = score
                        });

                        challengeWithJoinStatus.Leaderboard = challengeWithJoinStatus.Leaderboard
                            .OrderByDescending(l => l.Score)
                            .ToList();

                        for (int i = 0; i < challengeWithJoinStatus.Leaderboard.Count; i++)
                        {
                            challengeWithJoinStatus.Leaderboard[i].Rank = i + 1; //ranks are in descending order
                        }
                    }
                }
            }

            return Page();
        }

        private double CalculateScore(int streakCount)
        {
            // scoring logic: Score = StreakCount * 10 (for leaderboards)
            return streakCount * 10;
        }

        private string GetUserUsername(string userId)
        {
            Console.WriteLine("Getting username for user with ID: " + userId);

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user != null)
            {
                Console.WriteLine("User found with ID: " + userId + ". Username: " + user.Username);
                return user.Username;
            }
            else
            {
                Console.WriteLine("User with ID " + userId + " not found.");
                return "";
            }
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

            if (!performedToday)
            {
                participant.StreakCount = participant.StreakCount.HasValue ? participant.StreakCount + 1 : 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
