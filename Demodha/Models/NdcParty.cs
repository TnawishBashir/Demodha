using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcParty
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public NdcPartyType PartyType { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = default!;

    [Required, MaxLength(25)]
    public string CNIC { get; set; } = default!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }
}
