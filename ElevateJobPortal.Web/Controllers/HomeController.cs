using ElevateJobPortal.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElevateJobPortal.Web.Controllers;

public class HomeController : BaseController
{
    private readonly IJobService _jobService;

    public HomeController(IJobService jobService) => _jobService = jobService;

    public async Task<IActionResult> Index() => View(await _jobService.GetLatestAsync(6));

    public IActionResult Error() => View();
}
