using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class UserRate
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public int? ChallengeId { get; set; }

    public int Rate { get; set; }

    public string? Comment { get; set; }

    public virtual Challenge? Challenge { get; set; }
}
