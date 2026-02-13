using Microsoft.EntityFrameworkCore;

namespace ApllicationsAPI.Models.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts)
    {
    }

    public DbSet<Application> Applications { get; set; }
}