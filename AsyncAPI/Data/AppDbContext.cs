using AsyncAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AsyncAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ListingRequest> ListingRequests => Set<ListingRequest>();
}