using ElevateJobPortal.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElevateJobPortal.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        });

        builder.Entity<Company>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(150).IsRequired();
            e.Property(c => c.Website).HasMaxLength(250);
            e.HasIndex(c => c.OwnerId).IsUnique();
            e.HasOne(c => c.Owner)
             .WithOne(u => u.Company)
             .HasForeignKey<Company>(c => c.OwnerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Job>(e =>
        {
            e.Property(j => j.Title).HasMaxLength(150).IsRequired();
            e.Property(j => j.Location).HasMaxLength(150).IsRequired();
            e.Property(j => j.Description).IsRequired();
            e.Property(j => j.Type).HasConversion<string>().HasMaxLength(30);
            e.Property(j => j.SalaryMin).HasPrecision(12, 2);
            e.Property(j => j.SalaryMax).HasPrecision(12, 2);
            e.HasIndex(j => j.CreatedAt);
            e.HasOne(j => j.Company)
             .WithMany(c => c.Jobs)
             .HasForeignKey(j => j.CompanyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<JobApplication>(e =>
        {
            e.Property(a => a.CoverLetter).HasMaxLength(2000).IsRequired();
            e.Property(a => a.PortfolioUrl).HasMaxLength(250);
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(a => new { a.JobId, a.ApplicantId }).IsUnique();
            e.HasOne(a => a.Job)
             .WithMany(j => j.Applications)
             .HasForeignKey(a => a.JobId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Applicant)
             .WithMany(u => u.Applications)
             .HasForeignKey(a => a.ApplicantId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
