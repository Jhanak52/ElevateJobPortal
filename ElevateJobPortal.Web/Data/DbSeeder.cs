using ElevateJobPortal.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElevateJobPortal.Web.Data;

/// <summary>Creates the database, the two roles, and (in Development only) demo data.</summary>
public static class DbSeeder
{
    private const string DemoPassword = "Password@123";

    public static async Task InitialiseAsync(IServiceProvider services, bool seedDemoData)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Use migrations if they have been created, otherwise create the schema directly.
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync();
        else
            await db.Database.EnsureCreatedAsync();

        foreach (var role in new[] { AppRoles.Employer, AppRoles.JobSeeker })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (!seedDemoData || await db.Jobs.AnyAsync()) return;

        var employer1 = await CreateUserAsync(userManager, "employer1@elevate.com", "Sita Sharma", AppRoles.Employer);
        var employer2 = await CreateUserAsync(userManager, "employer2@elevate.com", "Ram Thapa", AppRoles.Employer);
        var seeker = await CreateUserAsync(userManager, "seeker@elevate.com", "Anita Gurung", AppRoles.JobSeeker);

        var company1 = new Company { Name = "Himalayan Tech Pvt. Ltd.", Description = "Software and IT services in Kathmandu.", OwnerId = employer1.Id };
        var company2 = new Company { Name = "Everest Hospitality Group", Description = "Hotels and trekking services across Nepal.", OwnerId = employer2.Id };
        db.Companies.AddRange(company1, company2);
        await db.SaveChangesAsync();

        var techTitles = new[] { "Junior .NET Developer", "QA Engineer", "UI/UX Designer", "DevOps Engineer", "Data Analyst", "Technical Support Officer",
                                 "Mobile App Developer", "Project Coordinator", "Database Administrator", "Frontend Developer", "Business Analyst", "Intern - Web Development" };
        var hotelTitles = new[] { "Front Desk Officer", "Head Chef", "Trekking Guide", "Housekeeping Supervisor", "Marketing Executive", "Accountant",
                                  "Restaurant Manager", "Tour Operator", "Customer Care Representative", "Barista", "Events Coordinator", "Hotel Receptionist" };
        var locations = new[] { "Kathmandu", "Lalitpur", "Pokhara", "Bhaktapur", "Biratnagar", "Chitwan" };
        var types = Enum.GetValues<JobType>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var jobs = new List<Job>();
        for (var i = 0; i < techTitles.Length; i++)
        {
            jobs.Add(new Job
            {
                Title = techTitles[i], CompanyId = company1.Id, Location = locations[i % locations.Length], Type = types[i % types.Length],
                Description = "We are looking for a motivated person to join our growing team. You will work with experienced colleagues, learn modern tools and contribute to real client projects.",
                SalaryMin = 30000 + i * 2500, SalaryMax = 50000 + i * 3000, Deadline = today.AddDays(10 + i * 3),
                CreatedAt = DateTime.UtcNow.AddHours(-(i * 5 + 2))
            });
        }
        for (var i = 0; i < hotelTitles.Length; i++)
        {
            jobs.Add(new Job
            {
                Title = hotelTitles[i], CompanyId = company2.Id, Location = locations[(i + 2) % locations.Length], Type = types[(i + 1) % types.Length],
                Description = "Join our hospitality team and help guests from around the world enjoy Nepal. Training is provided and growth opportunities are available.",
                SalaryMin = 22000 + i * 2000, SalaryMax = 38000 + i * 2500, Deadline = today.AddDays(7 + i * 4),
                CreatedAt = DateTime.UtcNow.AddHours(-(i * 5 + 4))
            });
        }
        db.Jobs.AddRange(jobs);
        await db.SaveChangesAsync();

        db.JobApplications.AddRange(
            new JobApplication { JobId = jobs[0].Id, ApplicantId = seeker.Id, CoverLetter = "I recently completed my BIT degree and have built several ASP.NET Core projects. I would love to join your team.", PortfolioUrl = "https://github.com/example" },
            new JobApplication { JobId = jobs[3].Id, ApplicantId = seeker.Id, CoverLetter = "I have hands-on experience with Docker and CI pipelines from my university projects and am keen to learn more." });
        await db.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, DemoPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException("Could not seed user: " + string.Join("; ", result.Errors.Select(e => e.Description)));
        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
