using System;
using System.ComponentModel.DataAnnotations;

public class ScenarioStatus
{
    [Key]
    public Guid ScenarioId { get; set; }

    [Required]
    public string Status { get; set; }

    public string? CurrentStep { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime LastUpdateTime { get; set; }
}