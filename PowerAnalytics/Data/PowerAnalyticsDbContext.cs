using Microsoft.EntityFrameworkCore;
using PowerAnalytics.Models;

namespace PowerAnalytics.Data;

public class PowerAnalyticsDbContext : DbContext
{
    public PowerAnalyticsDbContext(DbContextOptions<PowerAnalyticsDbContext> options) : base(options) { }

    public DbSet<PowerReading> PowerReadings { get; set; }
}
