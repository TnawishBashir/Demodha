using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcTask
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public int TaskDefinitionId { get; set; }
    public NdcTaskDefinition TaskDefinition { get; set; } = default!;

    public bool IsCompleted { get; set; } = false;

    [MaxLength(100)]
    public string? CompletedByUserId { get; set; }
    public DateTime? CompletedOn { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
