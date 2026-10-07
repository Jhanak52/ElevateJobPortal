using ElevateJobPortal.Web.Data;
using ElevateJobPortal.Web.Helpers;
using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElevateJobPortal.Web.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<JobService> _logger;

    public JobService(ApplicationDbContext db, ILogger<JobService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<PaginatedList<Job>> SearchAsync(string? search, string? location, JobType? type, int page, int pageSize)
    {
        var query = _db.Jobs.AsNoTracking().Include(j => j.Company).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(s) || j.Company!.Name.ToLower().Contains(s));
        }
        if (!string.IsNullOrWhiteSpace(location))
        {
            var l = location.Trim().ToLower();
            query = query.Where(j => j.Location.ToLower().Contains(l));
        }
        if (type.HasValue)
            query = query.Where(j => j.Type == type.Value);

        return PaginatedList<Job>.CreateAsync(query.OrderByDescending(j => j.CreatedAt), page, pageSize);
    }

    public async Task<IReadOnlyList<Job>> GetLatestAsync(int count) =>
        await _db.Jobs.AsNoTracking().Include(j => j.Company)
                 .OrderByDescending(j => j.CreatedAt).Take(count).ToListAsync();

    public Task<Job?> GetByIdAsync(int id) =>
        _db.Jobs.AsNoTracking().Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id);

    public Task<PaginatedList<Job>> GetByCompanyAsync(int companyId, int page, int pageSize)
    {
        var query = _db.Jobs.AsNoTracking()
                        .Include(j => j.Applications)
                        .Where(j => j.CompanyId == companyId)
                        .OrderByDescending(j => j.CreatedAt);
        return PaginatedList<Job>.CreateAsync(query, page, pageSize);
    }

    public async Task<IReadOnlyList<Job>> GetAllByCompanyAsync(int companyId) =>
        await _db.Jobs.AsNoTracking().Where(j => j.CompanyId == companyId)
                 .OrderBy(j => j.Title).ToListAsync();

    public async Task<Job> CreateAsync(JobFormViewModel model, int companyId)
    {
        var job = new Job { CompanyId = companyId };
        Apply(job, model);
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Job {JobId} created by company {CompanyId}", job.Id, companyId);
        return job;
    }

    public async Task<bool> UpdateAsync(int id, JobFormViewModel model, int companyId)
    {
        // The CompanyId check guarantees a company can only change its own jobs.
        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == companyId);
        if (job is null) return false;

        Apply(job, model);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Job {JobId} updated by company {CompanyId}", id, companyId);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.CompanyId == companyId);
        if (job is null) return false;

        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Job {JobId} deleted by company {CompanyId}", id, companyId);
        return true;
    }

    private static void Apply(Job job, JobFormViewModel model)
    {
        job.Title = model.Title.Trim();
        job.Description = model.Description.Trim();
        job.Location = model.Location.Trim();
        job.Type = model.Type;
        job.SalaryMin = model.SalaryMin;
        job.SalaryMax = model.SalaryMax;
        job.Deadline = model.Deadline;
    }
}
