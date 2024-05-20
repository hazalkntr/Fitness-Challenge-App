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
    public class AvailableChallengesModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public AvailableChallengesModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        public IList<Challenge> AvailableChallenges { get; set; }

        public async Task OnGetAsync()
        {
            AvailableChallenges = await _context.Challenges
                                                .ToListAsync();
        }

        public async Task<IActionResult> OnPostJoinAsync(int challengeId)
        {
            var challenge = await _context.Challenges.FindAsync(challengeId);
            if (challenge == null)
            {
                return NotFound();
            }

            // Get the current user's ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // User is not authenticated
            }

            int userId = int.Parse(userIdClaim.Value);

            // Check if the user is already a participant in the challenge
            var existingParticipant = await _context.ChallengeParticipants
                .FirstOrDefaultAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);

            if (existingParticipant == null)
            {
                var participant = new ChallengeParticipants
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
    }
}
