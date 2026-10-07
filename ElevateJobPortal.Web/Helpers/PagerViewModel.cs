namespace ElevateJobPortal.Web.Helpers;

/// <summary>Everything the shared pager partial needs to build page links.</summary>
public class PagerViewModel
{
    public int PageIndex { get; }
    public int TotalPages { get; }
    public string Action { get; }
    public string Controller { get; }
    private readonly Dictionary<string, string> _routeValues;

    public PagerViewModel(int pageIndex, int totalPages, string action, string controller,
                          IDictionary<string, string?>? routeValues = null)
    {
        PageIndex = pageIndex;
        TotalPages = totalPages;
        Action = action;
        Controller = controller;
        _routeValues = new Dictionary<string, string>();
        if (routeValues is not null)
        {
            foreach (var pair in routeValues)
            {
                if (!string.IsNullOrWhiteSpace(pair.Value)) _routeValues[pair.Key] = pair.Value!;
            }
        }
    }

    public Dictionary<string, string> RouteFor(int page)
    {
        var copy = new Dictionary<string, string>(_routeValues) { ["page"] = page.ToString() };
        return copy;
    }
}
