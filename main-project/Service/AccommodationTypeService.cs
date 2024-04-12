using main_project.Models;
using Microsoft.EntityFrameworkCore;

namespace main_project.Service;

public sealed class AccommodationTypeService
{
    private static AccommodationTypeService? _instance;
    // Private constructor to enforce singleton pattern.
    private AccommodationTypeService() { }

    public static AccommodationTypeService Instance()
    {
        return _instance ??= new AccommodationTypeService();
    }

    public ICollection<AccommodationType> GetAccommodationTypes()
    {
        return AccommodationType.Query().Include((at) => at.Hotels).ToList();
    }

}