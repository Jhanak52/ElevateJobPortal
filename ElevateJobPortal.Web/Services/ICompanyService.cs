using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;

namespace ElevateJobPortal.Web.Services;

public interface ICompanyService
{
    Task<Company?> GetByOwnerIdAsync(string ownerId);
    Task<DashboardStats> GetStatsAsync(int companyId);
}
