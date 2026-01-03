using Microsoft.AspNetCore.Identity;
using System;

namespace Demodha.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public int? UserType { get; set; }
        public string? User_CNIC { get; set; }
        public string? File_Number { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
