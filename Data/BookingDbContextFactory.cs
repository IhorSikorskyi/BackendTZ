using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BackendTZ.Data;

/// <summary>
/// Factory class for creating instances of BookingDbContext at design time.
/// </summary>
public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    /// <summary>
    /// Creates a new instance of BookingDbContext using the provided arguments.
    /// </summary>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>A new instance of BookingDbContext.</returns>
    public BookingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlite(connectionString);

        return new BookingDbContext(optionsBuilder.Options);
    }
}