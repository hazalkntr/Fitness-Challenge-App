using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Leaderboard
{
    public int LeaderboardId { get; set; }

    public int ChallengeId { get; set; }

    public string UserId { get; set; } = null!;

    public int Rank { get; set; }

    public double Score { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
