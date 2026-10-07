using ElevateJobPortal.Web.Helpers;
using ElevateJobPortal.Web.Models;

namespace ElevateJobPortal.Web.ViewModels;

public class JobListViewModel
{
    public PaginatedList<Job> Jobs { get; set; } = null!;
    public string? Search { get; set; }
    public string? Location { get; set; }
    public JobType? Type { get; set; }
}

public class JobDetailsViewModel
{
    public Job Job { get; set; } = null!;
    public bool IsOwner { get; set; }
    public bool CanApply { get; set; }
    public bool HasApplied { get; set; }
}

public class DashboardViewModel
{
    public Company Company { get; set; } = null!;
    public DashboardStats Stats { get; set; } = null!;
    public PaginatedList<Job> Jobs { get; set; } = null!;
    public IReadOnlyList<JobApplication> RecentApplications { get; set; } = Array.Empty<JobApplication>();
}

public class CompanyApplicationsViewModel
{
    public Company Company { get; set; } = null!;
    public IReadOnlyList<Job> CompanyJobs { get; set; } = Array.Empty<Job>();
    public IReadOnlyList<JobApplication> Applications { get; set; } = Array.Empty<JobApplication>();
    public int? SelectedJobId { get; set; }
}

public class DashboardStats
{
    public int TotalJobs { get; set; }
    public int ActiveJobs { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
}
