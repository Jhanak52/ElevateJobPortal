using System.ComponentModel.DataAnnotations;

namespace ElevateJobPortal.Web.ViewModels;

public class ApplyViewModel
{
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 20)]
    [Display(Name = "Cover letter")]
    public string CoverLetter { get; set; } = string.Empty;

    [Url, StringLength(250)]
    [Display(Name = "Portfolio / CV link (optional)")]
    public string? PortfolioUrl { get; set; }
}
