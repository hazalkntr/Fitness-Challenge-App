using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class UserRate
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public int? ChallengeId { get; set; }

    public short? Rate { get; set; }
}
