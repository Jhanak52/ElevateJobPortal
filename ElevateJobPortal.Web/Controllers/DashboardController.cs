using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.Services;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElevateJobPortal.Web.Controllers;

[Authorize(Roles = AppRoles.Employer)]
public class DashboardController : BaseController
{
    private const int PageSize = 5;

    private readonly ICompanyService _companyService;
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;

    public DashboardController(ICompanyService companyService, IJobService jobService, IApplicationService applicationService)
    {
        _companyService = companyService;
        _jobService = jobService;
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var model = new DashboardViewModel
        {
            Company = company,
            Stats = await _companyService.GetStatsAsync(company.Id),
            Jobs = await _jobService.GetByCompanyAsync(company.Id, page, PageSize),
            RecentApplications = await _applicationService.GetRecentForCompanyAsync(company.Id, 5)
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Applications(int? jobId)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var model = new CompanyApplicationsViewModel
        {
            Company = company,
            SelectedJobId = jobId,
            CompanyJobs = await _jobService.GetAllByCompanyAsync(company.Id),
            Applications = await _applicationService.GetForCompanyAsync(company.Id, jobId)
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int applicationId, ApplicationStatus status, int? jobId)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var ok = await _applicationService.UpdateStatusAsync(applicationId, company.Id, status);
        TempData[ok ? "Success" : "Error"] = ok ? "Application status updated." : "Application not found.";
        return RedirectToAction(nameof(Applications), new { jobId });
    }
}
