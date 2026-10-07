using System.ComponentModel.DataAnnotations;
using ElevateJobPortal.Web.Models;

namespace ElevateJobPortal.Web.ViewModels;

public class JobFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 30)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Job type")]
    public JobType Type { get; set; } = JobType.FullTime;

    [Range(typeof(decimal), "0", "100000000")]
    [Display(Name = "Minimum salary (NPR)")]
    public decimal? SalaryMin { get; set; }

    [Range(typeof(decimal), "0", "100000000")]
    [Display(Name = "Maximum salary (NPR)")]
    public decimal? SalaryMax { get; set; }

    [Required]
    [Display(Name = "Application deadline")]
    public DateOnly Deadline { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SalaryMin.HasValue && SalaryMax.HasValue && SalaryMax < SalaryMin)
            yield return new ValidationResult("Maximum salary cannot be lower than minimum salary.", new[] { nameof(SalaryMax) });
    }

    public static JobFormViewModel FromJob(Job job) => new()
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Location = job.Location,
        Type = job.Type,
        SalaryMin = job.SalaryMin,
        SalaryMax = job.SalaryMax,
        Deadline = job.Deadline
    };
}
