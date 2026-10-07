namespace ElevateJobPortal.Web.Models;

public class Job : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobType Type { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public DateOnly Deadline { get; set; }

    public int CompanyId { get; set; }
    public Company? Company { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    /// <summary>Behaviour lives on the entity (encapsulation).</summary>
    public bool IsExpired => Deadline < DateOnly.FromDateTime(DateTime.UtcNow);

    public string SalaryRange
    {
        get
        {
            if (SalaryMin is null && SalaryMax is null) return "Negotiable";
            if (SalaryMin is not null && SalaryMax is not null) return $"NPR {SalaryMin:N0} - {SalaryMax:N0}";
            return SalaryMin is not null ? $"From NPR {SalaryMin:N0}" : $"Up to NPR {SalaryMax:N0}";
        }
    }
}
