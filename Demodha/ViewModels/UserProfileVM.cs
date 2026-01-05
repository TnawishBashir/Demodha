using System.ComponentModel.DataAnnotations;

namespace Demodha.ViewModels;

public class UserProfileVM
{
    [Required, StringLength(80)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = "";

    [Required, StringLength(80)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = "";

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}
