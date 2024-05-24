using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fitness.Pages
{
    public class AvailableChallengesModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public AvailableChallengesModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        public IList<ChallengeWithJoinStatus> AvailableChallenges { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class ChallengeWithJoinStatus
        {
            public Challenge Challenge { get; set; }
            public bool IsJoined { get; set; }
        }

        public async Task OnGetAsync(string searchKeyword, string difficultyLevel, string category)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var query = _context.Challenges.AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(challenge =>
                    challenge.Title.Contains(searchKeyword) ||
                    challenge.Description.Contains(searchKeyword) ||
                    challenge.Instructions.Contains(searchKeyword)
                );
            }

            if (!string.IsNullOrEmpty(difficultyLevel))
            {
                query = query.Where(challenge => challenge.DifficultyLevel == difficultyLevel);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(challenge => challenge.Category == category);
            }

            var filteredChallenges = await query.ToListAsync();

            var joinedChallenges = await _context.ChallengeParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ChallengeId)
                .ToListAsync();

            AvailableChallenges = filteredChallenges.Select(challenge => new ChallengeWithJoinStatus
            {
                Challenge = challenge,
                IsJoined = joinedChallenges.Contains(challenge.ChallengeId)
            }).ToList();

        }


        public async Task<IActionResult> OnPostJoinAsync(int challengeId)
        {
            var challenge = await _context.Challenges.FindAsync(challengeId);
            if (challenge == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            var existingParticipant = await _context.ChallengeParticipants
                .FirstOrDefaultAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);

            if (existingParticipant == null)
            {
                var participant = new ChallengeParticipant
                {
                    ChallengeId = challengeId,
                    UserId = userId,
                    JoinDate = DateTime.Now
                };

                _context.ChallengeParticipants.Add(participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostGiveUpAsync(int challengeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var participant = await _context.ChallengeParticipants
                .FirstOrDefaultAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);

            if (participant != null)
            {
                _context.ChallengeParticipants.Remove(participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int challengeId)
        {
            var challenge = await _context.Challenges.FindAsync(challengeId);
            if (challenge == null)
            {
                return NotFound();
            }

            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSaveToFavoritesAsync(int challengeId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            var existingFavorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.ChallengeId == challengeId && uf.UserId == userId);

            if (existingFavorite == null)
            {
                var favorite = new UserFavorite
                {
                    ChallengeId = challengeId,
                    UserId = userId,
                    SavedDate = DateTime.Now
                };

                _context.UserFavorites.Add(favorite);
                await _context.SaveChangesAsync();

                
                StatusMessage = "Added to favorite challenges! View them on your profile page.";

                Console.WriteLine($"User {userId} saved challenge {challengeId} to favorites.");
            }
            else
            {
                StatusMessage = "This challenge is already one of your favorite challenges! View them on your profile page.";
                Console.WriteLine($"User {userId} already has challenge {challengeId} in favorites.");
            }

            return RedirectToPage();
        }

    }
}
