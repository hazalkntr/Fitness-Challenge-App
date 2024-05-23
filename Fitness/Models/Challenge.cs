using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Challenge
{
    public int ChallengeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? UserId { get; set; }

    public string? Category { get; set; }

    public string? DifficultyLevel { get; set; }

    public string? Instructions { get; set; }

    public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; } = new List<ChallengeParticipant>();

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<UserRate> UserRates { get; set; } = new List<UserRate>();
}
