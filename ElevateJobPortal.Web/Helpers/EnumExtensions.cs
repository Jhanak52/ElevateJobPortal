using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ElevateJobPortal.Web.Helpers;

public static class EnumExtensions
{
    /// <summary>Returns the [Display(Name)] of an enum value, or its name.</summary>
    public static string ToDisplay(this Enum value)
    {
        var member = value.GetType().GetField(value.ToString());
        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }
}
