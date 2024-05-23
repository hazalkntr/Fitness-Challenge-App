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

        public IList<ChallengeWithJoinStatus> AvailableChallenges { get; set; }

        public class ChallengeWithJoinStatus
        {
            public Challenge Challenge { get; set; }
            public bool IsJoined { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim?.Value;

            var challenges = await _context.Challenges.ToListAsync();
            var joinedChallenges = await _context.ChallengeParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ChallengeId)
                .ToListAsync();

            AvailableChallenges = challenges.Select(challenge => new ChallengeWithJoinStatus
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
    }
}
