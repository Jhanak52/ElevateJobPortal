using ElevateJobPortal.Web.Helpers;
using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;

namespace ElevateJobPortal.Web.Services;

public interface IJobService
{
    Task<PaginatedList<Job>> SearchAsync(string? search, string? location, JobType? type, int page, int pageSize);
    Task<IReadOnlyList<Job>> GetLatestAsync(int count);
    Task<Job?> GetByIdAsync(int id);
    Task<PaginatedList<Job>> GetByCompanyAsync(int companyId, int page, int pageSize);
    Task<IReadOnlyList<Job>> GetAllByCompanyAsync(int companyId);
    Task<Job> CreateAsync(JobFormViewModel model, int companyId);
    Task<bool> UpdateAsync(int id, JobFormViewModel model, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
}
