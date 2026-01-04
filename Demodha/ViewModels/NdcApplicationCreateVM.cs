using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Demodha.ViewModels
{
    public class NdcApplicationVM
    {
        [Required, StringLength(50)]
        public string PlotOrFileNo { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Block { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string SectorOrPhase { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string SocietyOrScheme { get; set; } = "DHA";

        [Required, StringLength(120)]
        public string SellerName { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string SellerCnic { get; set; } = string.Empty; 

        [Required, Phone]
        public string SellerPhone { get; set; } = string.Empty;

        [EmailAddress]
        public string? SellerEmail { get; set; }

        [StringLength(250)]
        public string? SellerAddress { get; set; }

        [StringLength(120)]
        public string? PurchaserName { get; set; }

        [StringLength(15)]
        public string? PurchaserCnic { get; set; }

        [Required]
        public string DealerUserId { get; set; } = "";

        public List<SelectListItem> DealerOptions { get; set; } = new();

        [Display(Name = "Owner CNIC Copy")]
        public IFormFile? OwnerCnicCopy { get; set; }

        [Display(Name = "Photograph 1")]
        public IFormFile? Photo1 { get; set; }

        [Display(Name = "Photograph 2")]
        public IFormFile? Photo2 { get; set; }

        [Display(Name = "Application to Secretary DHAG")]
        public IFormFile? ApplicationToSecretary { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public bool SubmitToDealer { get; set; } = false;
    }
}
