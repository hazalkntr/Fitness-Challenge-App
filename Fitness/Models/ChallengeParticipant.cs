using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class ChallengeParticipant
{
    public int ParticipantId { get; set; }

    public string UserId { get; set; } = null!;

    public int ChallengeId { get; set; }

    public DateTime JoinDate { get; set; }

    public string? Progress { get; set; }

    public virtual Challenge Challenge { get; set; } = null!;
}
