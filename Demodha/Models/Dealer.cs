using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class Dealer
{
    public long Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Code { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;
}
