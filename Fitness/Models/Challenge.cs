using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ChallengeParticipants> ChallengeParticipants { get; set; } = new List<ChallengeParticipants>();

    public virtual ICollection<Leaderboard> Leaderboard { get; set; } = new List<Leaderboard>();
}
