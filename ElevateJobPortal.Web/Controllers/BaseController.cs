using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ElevateJobPortal.Web.Controllers;

/// <summary>Shared behaviour for all controllers (inheritance).</summary>
public abstract class BaseController : Controller
{
    protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
