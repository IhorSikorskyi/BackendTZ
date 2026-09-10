using BackendTZ.Data;
using BackendTZ.Entities;
using BackendTZ.Repositories.Interfaces;

namespace BackendTZ.Repositories.Implementations;

public class BookingRepository(BookingDbContext context) : Repository<Booking>(context), IBookingRepository
{
    
}