using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcTaskDefinition
{
    public int Id { get; set; }

    public NdcStage Stage { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = default!;

    public int SortOrder { get; set; }
    public bool IsMandatory { get; set; } = true;

    public ICollection<NdcTask> Tasks { get; set; } = new List<NdcTask>();
}
