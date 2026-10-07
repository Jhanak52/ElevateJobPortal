using ElevateJobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ElevateJobPortal.Web.Services;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);
    Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
}
