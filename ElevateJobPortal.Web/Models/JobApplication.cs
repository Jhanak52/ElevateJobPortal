namespace ElevateJobPortal.Web.Models;

public class JobApplication : BaseEntity
{
    public int JobId { get; set; }
    public Job? Job { get; set; }

    public string ApplicantId { get; set; } = string.Empty;
    public ApplicationUser? Applicant { get; set; }

    public string CoverLetter { get; set; } = string.Empty;
    public string? PortfolioUrl { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
}
