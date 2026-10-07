using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.Services;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElevateJobPortal.Web.Controllers;

[Authorize(Roles = AppRoles.JobSeeker)]
public class ApplicationsController : BaseController
{
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IJobService jobService, IApplicationService applicationService)
    {
        _jobService = jobService;
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<IActionResult> Apply(int jobId)
    {
        var job = await _jobService.GetByIdAsync(jobId);
        if (job is null) return NotFound();

        if (job.IsExpired || await _applicationService.HasAppliedAsync(jobId, CurrentUserId!))
        {
            TempData["Error"] = "You cannot apply to this job (it has expired or you already applied).";
            return RedirectToAction("Details", "Jobs", new { id = jobId });
        }

        return View(new ApplyViewModel { JobId = job.Id, JobTitle = job.Title, CompanyName = job.Company?.Name ?? string.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var outcome = await _applicationService.ApplyAsync(CurrentUserId!, model);
        switch (outcome)
        {
            case ApplyOutcome.Success:
                TempData["Success"] = "Application submitted. Good luck!";
                return RedirectToAction(nameof(My));
            case ApplyOutcome.JobNotFound:
                return NotFound();
            case ApplyOutcome.JobExpired:
                TempData["Error"] = "This job has expired.";
                break;
            default:
                TempData["Error"] = "You have already applied to this job.";
                break;
        }
        return RedirectToAction("Details", "Jobs", new { id = model.JobId });
    }

    [HttpGet]
    public async Task<IActionResult> My() => View(await _applicationService.GetByApplicantAsync(CurrentUserId!));
}
