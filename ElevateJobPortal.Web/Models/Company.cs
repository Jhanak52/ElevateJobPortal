namespace ElevateJobPortal.Web.Models;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser? Owner { get; set; }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
