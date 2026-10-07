using Microsoft.AspNetCore.Identity;

namespace ElevateJobPortal.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Only set for users registered as Employer.</summary>
    public Company? Company { get; set; }
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
