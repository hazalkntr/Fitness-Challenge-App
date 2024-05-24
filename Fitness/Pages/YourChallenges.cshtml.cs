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
                            Username = _context.Users.FirstOrDefault(u => u.UserId == l.UserId).Username ?? "Unknown User",
                            Score = l.Score
                        })
                        .ToList()
                })
                .ToListAsync();

            // Ensure the leaderboard includes all participants
            foreach (var challengeWithJoinStatus in UserChallenges)
            {
                Console.WriteLine($"Updating leaderboard for challenge ID: {challengeWithJoinStatus.Challenge.ChallengeId}");
                await UpdateLeaderboardAsync(challengeWithJoinStatus.Challenge.ChallengeId, challengeWithJoinStatus.Leaderboard);
            }

            return Page();
        }

        private async Task UpdateLeaderboardAsync(int challengeId, List<LeaderboardViewModel> leaderboard)
        {
            // Retrieve participants of the challenge
            var participants = await _context.ChallengeParticipants
                .Where(cp => cp.ChallengeId == challengeId)
                .ToListAsync();

            foreach (var participant in participants)
            {
                Console.WriteLine($"Processing participant: {participant.UserId}");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == participant.UserId);
                if (user != null)
                {
                    Console.WriteLine($"Found user: {user.Username}");
                    var existingEntry = leaderboard.FirstOrDefault(l => l.Username == user.Username);
                    if (existingEntry != null)
                    {
                        // Update existing entry
                        existingEntry.Score = CalculateScore(participant.StreakCount ?? 0);
                        Console.WriteLine($"Updated score for {user.Username}: {existingEntry.Score}");
                    }
                    else
                    {
                        // Add new entry
                        leaderboard.Add(new LeaderboardViewModel
                        {
                            Rank = 0, // Will be updated later
                            Username = user.Username,
                            Score = CalculateScore(participant.StreakCount ?? 0)
                        });
                        Console.WriteLine($"Added new entry for {user.Username}: {leaderboard.Last().Score}");
                    }
                }
                else
                {
                    Console.WriteLine($"User not found for ID: {participant.UserId}");
                }
            }

            // Order and assign ranks
            leaderboard = leaderboard.OrderByDescending(l => l.Score).ToList();
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
                Console.WriteLine($"Rank {leaderboard[i].Rank}: {leaderboard[i].Username} with score {leaderboard[i].Score}");
            }
        }

        private double CalculateScore(int streakCount)
        {
            // Scoring logic: Score = StreakCount * 10 (for leaderboards)
            return streakCount * 10;
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
                Console.WriteLine("updating by 1111111111111111111");
                participant.StreakCount = participant.StreakCount.HasValue ? participant.StreakCount + 1 : 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
