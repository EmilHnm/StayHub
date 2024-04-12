using main_project.Models;

namespace main_project.Service;

public class UtilityService
{
    private static UtilityService? _instance = null;
    private UtilityService() { }
    public static UtilityService Instance()
    {
        return _instance ??= new UtilityService();
    }

    public ICollection<Utility> GetUtilities()
    {
        return Utility.Query().OrderBy(u => u.Name).ToList();
    }
}