using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.Services;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElevateJobPortal.Web.Controllers;

public class JobsController : BaseController
{
    private const int PageSize = 6;

    private readonly IJobService _jobService;
    private readonly ICompanyService _companyService;
    private readonly IApplicationService _applicationService;

    public JobsController(IJobService jobService, ICompanyService companyService, IApplicationService applicationService)
    {
        _jobService = jobService;
        _companyService = companyService;
        _applicationService = applicationService;
    }

    // ---------- Public: list with pagination + details ----------

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? location, JobType? type, int page = 1)
    {
        var jobs = await _jobService.SearchAsync(search, location, type, page, PageSize);
        return View(new JobListViewModel { Jobs = jobs, Search = search, Location = location, Type = type });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job is null) return NotFound();

        var model = new JobDetailsViewModel { Job = job };
        var userId = CurrentUserId;
        if (userId is not null)
        {
            model.IsOwner = User.IsInRole(AppRoles.Employer) && job.Company?.OwnerId == userId;
            if (User.IsInRole(AppRoles.JobSeeker))
            {
                model.HasApplied = await _applicationService.HasAppliedAsync(id, userId);
                model.CanApply = !model.HasApplied && !job.IsExpired;
            }
        }
        return View(model);
    }

    // ---------- Employer only: create / update / delete ----------

    [HttpGet, Authorize(Roles = AppRoles.Employer)]
    public IActionResult Create() => View(new JobFormViewModel());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Employer)]
    public async Task<IActionResult> Create(JobFormViewModel model)
    {
        if (model.Deadline < DateOnly.FromDateTime(DateTime.UtcNow))
            ModelState.AddModelError(nameof(model.Deadline), "The deadline cannot be in the past.");
        if (!ModelState.IsValid) return View(model);

        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var job = await _jobService.CreateAsync(model, company.Id);
        TempData["Success"] = "Job posted successfully.";
        return RedirectToAction(nameof(Details), new { id = job.Id });
    }

    [HttpGet, Authorize(Roles = AppRoles.Employer)]
    public async Task<IActionResult> Edit(int id)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        var job = await _jobService.GetByIdAsync(id);
        if (job is null) return NotFound();
        if (company is null || job.CompanyId != company.Id) return Forbid();

        return View(JobFormViewModel.FromJob(job));
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Employer)]
    public async Task<IActionResult> Edit(int id, JobFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var updated = await _jobService.UpdateAsync(id, model, company.Id);
        if (!updated) return NotFound();

        TempData["Success"] = "Job updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet, Authorize(Roles = AppRoles.Employer)]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        var job = await _jobService.GetByIdAsync(id);
        if (job is null) return NotFound();
        if (company is null || job.CompanyId != company.Id) return Forbid();

        return View(job);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = AppRoles.Employer)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var company = await _companyService.GetByOwnerIdAsync(CurrentUserId!);
        if (company is null) return Forbid();

        var deleted = await _jobService.DeleteAsync(id, company.Id);
        if (!deleted) return NotFound();

        TempData["Success"] = "Job deleted.";
        return RedirectToAction("Index", "Dashboard");
    }
}
