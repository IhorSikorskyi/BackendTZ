using BackendTZ.Entities;
using BackendTZ.Services.Interfaces;

namespace BackendTZ.Services.Implementations;

public class PricingService : IPricingService
{
    public decimal CalculateRentalCost(Room room, DateTime date, TimeSpan start, TimeSpan end, List<int> serviceIds)
    {
        throw new NotImplementedException();
    }
}