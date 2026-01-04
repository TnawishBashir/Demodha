using System.ComponentModel.DataAnnotations;

namespace Demodha.Models;

public class NdcAppointment
{
    public long Id { get; set; }

    public long NdcApplicationId { get; set; }
    public NdcApplication NdcApplication { get; set; } = default!;

    public DateTime AppointmentDateTime { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool NotifiedOwner { get; set; } = false;
    public bool NotifiedDealer { get; set; } = false;

    [Required, MaxLength(100)]
    public string CreatedByUserId { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
