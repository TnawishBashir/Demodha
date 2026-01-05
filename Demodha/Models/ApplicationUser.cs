using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Demodha.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public int? UserType { get; set; }
        public string? User_CNIC { get; set; }
        public string? File_Number { get; set; }
        [StringLength(80)]
        public string? FirstName { get; set; }

        [StringLength(80)]
        public string? LastName { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
