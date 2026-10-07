using System.ComponentModel.DataAnnotations;
using ElevateJobPortal.Web.Models;

namespace ElevateJobPortal.Web.ViewModels;

public class RegisterViewModel : IValidatableObject
{
    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "I am a")]
    public string Role { get; set; } = AppRoles.JobSeeker;

    [StringLength(150)]
    [Display(Name = "Company name")]
    public string? CompanyName { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Role != AppRoles.JobSeeker && Role != AppRoles.Employer)
            yield return new ValidationResult("Please choose Job Seeker or Employer.", new[] { nameof(Role) });

        if (Role == AppRoles.Employer && string.IsNullOrWhiteSpace(CompanyName))
            yield return new ValidationResult("Company name is required for employers.", new[] { nameof(CompanyName) });
    }
}
