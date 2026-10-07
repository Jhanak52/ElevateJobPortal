using ElevateJobPortal.Web.Data;
using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElevateJobPortal.Web.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _db;

    public CompanyService(ApplicationDbContext db) => _db = db;

    public Task<Company?> GetByOwnerIdAsync(string ownerId) =>
        _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.OwnerId == ownerId);

    public async Task<DashboardStats> GetStatsAsync(int companyId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var jobs = _db.Jobs.Where(j => j.CompanyId == companyId);
        var applications = _db.JobApplications.Where(a => a.Job!.CompanyId == companyId);

        return new DashboardStats
        {
            TotalJobs = await jobs.CountAsync(),
            ActiveJobs = await jobs.CountAsync(j => j.Deadline >= today),
            TotalApplications = await applications.CountAsync(),
            PendingApplications = await applications.CountAsync(a => a.Status == ApplicationStatus.Pending)
        };
    }
}
