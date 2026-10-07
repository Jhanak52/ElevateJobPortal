using System.ComponentModel.DataAnnotations;

namespace ElevateJobPortal.Web.Models;

public enum JobType
{
    [Display(Name = "Full Time")] FullTime,
    [Display(Name = "Part Time")] PartTime,
    [Display(Name = "Contract")] Contract,
    [Display(Name = "Internship")] Internship,
    [Display(Name = "Remote")] Remote
}

public enum ApplicationStatus
{
    [Display(Name = "Pending")] Pending,
    [Display(Name = "Reviewed")] Reviewed,
    [Display(Name = "Shortlisted")] Shortlisted,
    [Display(Name = "Rejected")] Rejected
}

public static class AppRoles
{
    public const string Employer = "Employer";
    public const string JobSeeker = "JobSeeker";
}
