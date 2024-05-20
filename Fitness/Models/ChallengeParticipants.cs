using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class ChallengeParticipants
{
    public int ParticipantId { get; set; }

    public int UserId { get; set; }

    public int ChallengeId { get; set; }

    public DateTime JoinDate { get; set; }

    public string? Progress { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
