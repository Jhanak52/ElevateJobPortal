using ElevateJobPortal.Web.Data;
using ElevateJobPortal.Web.Models;
using ElevateJobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ElevateJobPortal.Web.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          ApplicationDbContext db,
                          ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _logger = logger;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName.Trim(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return result;

        var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return roleResult;
        }

        if (model.Role == AppRoles.Employer)
        {
            _db.Companies.Add(new Company { Name = model.CompanyName!.Trim(), OwnerId = user.Id });
            await _db.SaveChangesAsync();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        _logger.LogInformation("New {Role} registered: {Email}", model.Role, model.Email);
        return IdentityResult.Success;
    }

    public Task<Microsoft.AspNetCore.Identity.SignInResult> LoginAsync(LoginViewModel model) =>
        _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

    public Task LogoutAsync() => _signInManager.SignOutAsync();
}
