using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;

namespace ElevateJobPortal.Web.Services;

public enum ApplyOutcome
{
    Success,
    JobNotFound,
    JobExpired,
    AlreadyApplied
}

public interface IApplicationService
{
    Task<ApplyOutcome> ApplyAsync(string applicantId, ApplyViewModel model);
    Task<bool> HasAppliedAsync(int jobId, string applicantId);
    Task<IReadOnlyList<JobApplication>> GetByApplicantAsync(string applicantId);
    Task<IReadOnlyList<JobApplication>> GetForCompanyAsync(int companyId, int? jobId = null);
    Task<IReadOnlyList<JobApplication>> GetRecentForCompanyAsync(int companyId, int count);
    Task<bool> UpdateStatusAsync(int applicationId, int companyId, ApplicationStatus status);
}
