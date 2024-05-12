using System;
using System.Collections.Generic;

namespace FitnessChallenge.Models;

public partial class TblFitness
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? UserId { get; set; }

    public string? Category { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? EndDate { get; set; }
}
