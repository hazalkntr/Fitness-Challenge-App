using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;

namespace MyApp.Namespace
{
    public class ChallengeStatisticsModel : PageModel
    {
        private readonly FitnessChallengeContext _context;

        public ChallengeStatisticsModel(FitnessChallengeContext context)
        {
            _context = context;
        }

        public List<ChallengeStatisticViewModel> ChallengeStatistics { get; set; }

        public void OnGet()
        {
            //avg ratings and comments for each challenge
            ChallengeStatistics = _context.Challenges
                .Include(c => c.UserRates)
                .Select(c => new ChallengeStatisticViewModel
                {
                    Challenge = c,
                    AverageRating = c.UserRates.Any() ? c.UserRates.Average(ur => ur.Rate) : 0,
                    Comments = c.UserRates.Select(ur => ur.Comment).ToList()
                })
                .ToList(); 
        }
    }

    public class ChallengeStatisticViewModel
    {
        public Challenge Challenge { get; set; }
        public double AverageRating { get; set; }
        public List<string> Comments { get; set; }
    }
}
