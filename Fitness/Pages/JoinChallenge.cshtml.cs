using System.Threading.Tasks;
using Fitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



namespace Fitness.Pages
{
    public class JoinChallengeModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public JoinChallengeModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int ChallengeId { get; set; }

        public async Task<IActionResult> OnPostAsync()
    {
        var challenge = await _context.Challenges.FindAsync(ChallengeId);
        if (challenge == null)
        {
            return NotFound();
        }

        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return BadRequest("You must be logged in to join a challenge.");
        }

        // Check if the user is already participating in the challenge
        var existingParticipant = await _context.ChallengeParticipants
        .FirstOrDefaultAsync(cp => cp.ChallengeId == ChallengeId && cp.UserId == userId);

        if (existingParticipant != null)
        {
            // User is already participating in the challenge
            return BadRequest("You are already participating in this challenge.");
        }

        // Create a new ChallengeParticipants entity and add it to the context
        var newParticipant = new ChallengeParticipant
        {
            ChallengeId = ChallengeId,
            UserId = userId,
            JoinDate = DateTime.Now 
        };

        _context.ChallengeParticipants.Add(newParticipant);
        await _context.SaveChangesAsync();

        // Redirect to a page indicating successful join
        return RedirectToPage("/JoinSuccess");
    }
    }
}
