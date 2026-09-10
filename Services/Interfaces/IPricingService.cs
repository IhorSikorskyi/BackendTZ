using BackendTZ.Entities;

namespace BackendTZ.Services.Interfaces;

public interface IPricingService
{
    decimal CalculateRentalCost(Room room, DateTime date, TimeSpan start, TimeSpan end, List<int> serviceIds);
}