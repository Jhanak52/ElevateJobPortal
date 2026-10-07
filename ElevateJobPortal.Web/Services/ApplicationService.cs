using ElevateJobPortal.Web.Data;
using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElevateJobPortal.Web.Services;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(ApplicationDbContext db, ILogger<ApplicationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ApplyOutcome> ApplyAsync(string applicantId, ApplyViewModel model)
    {
        var job = await _db.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == model.JobId);
        if (job is null) return ApplyOutcome.JobNotFound;
        if (job.IsExpired) return ApplyOutcome.JobExpired;
        if (await HasAppliedAsync(model.JobId, applicantId)) return ApplyOutcome.AlreadyApplied;

        _db.JobApplications.Add(new JobApplication
        {
            JobId = model.JobId,
            ApplicantId = applicantId,
            CoverLetter = model.CoverLetter.Trim(),
            PortfolioUrl = string.IsNullOrWhiteSpace(model.PortfolioUrl) ? null : model.PortfolioUrl.Trim()
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // The unique (JobId, ApplicantId) index protects against double-clicks.
            _logger.LogWarning(ex, "Duplicate application blocked for job {JobId}", model.JobId);
            return ApplyOutcome.AlreadyApplied;
        }

        _logger.LogInformation("User {UserId} applied to job {JobId}", applicantId, model.JobId);
        return ApplyOutcome.Success;
    }

    public Task<bool> HasAppliedAsync(int jobId, string applicantId) =>
        _db.JobApplications.AnyAsync(a => a.JobId == jobId && a.ApplicantId == applicantId);

    public async Task<IReadOnlyList<JobApplication>> GetByApplicantAsync(string applicantId) =>
        await _db.JobApplications.AsNoTracking()
                 .Include(a => a.Job).ThenInclude(j => j!.Company)
                 .Where(a => a.ApplicantId == applicantId)
                 .OrderByDescending(a => a.CreatedAt).ToListAsync();

    public async Task<IReadOnlyList<JobApplication>> GetForCompanyAsync(int companyId, int? jobId = null)
    {
        var query = _db.JobApplications.AsNoTracking()
                       .Include(a => a.Job).Include(a => a.Applicant)
                       .Where(a => a.Job!.CompanyId == companyId);
        if (jobId.HasValue) query = query.Where(a => a.JobId == jobId.Value);
        return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<IReadOnlyList<JobApplication>> GetRecentForCompanyAsync(int companyId, int count) =>
        await _db.JobApplications.AsNoTracking()
                 .Include(a => a.Job).Include(a => a.Applicant)
                 .Where(a => a.Job!.CompanyId == companyId)
                 .OrderByDescending(a => a.CreatedAt).Take(count).ToListAsync();

    public async Task<bool> UpdateStatusAsync(int applicationId, int companyId, ApplicationStatus status)
    {
        var application = await _db.JobApplications
                                   .Include(a => a.Job)
                                   .FirstOrDefaultAsync(a => a.Id == applicationId && a.Job!.CompanyId == companyId);
        if (application is null) return false;

        application.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }
}
