namespace ElevateJobPortal.Web.Models;

/// <summary>Abstract base class shared by all domain entities (inheritance).</summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
