using System.ComponentModel.DataAnnotations;
using Demodha.Models;

namespace Demodha.ViewModels;

public class NdcUploadDocsVM
{
    public long ApplicationId { get; set; }
    public string AppNo { get; set; } = "";

    public bool HasCnic { get; set; }
    public bool HasPhoto1 { get; set; }
    public bool HasPhoto2 { get; set; }
    public bool HasApplicationToSecretary { get; set; }

    [Display(Name = "Owner CNIC Copy")]
    public IFormFile? OwnerCnicCopy { get; set; }

    [Display(Name = "Photograph 1")]
    public IFormFile? Photo1 { get; set; }

    [Display(Name = "Photograph 2")]
    public IFormFile? Photo2 { get; set; }

    [Display(Name = "Application to Secretary DHAG")]
    public IFormFile? ApplicationToSecretary { get; set; }
}
