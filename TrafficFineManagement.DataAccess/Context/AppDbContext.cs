using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Entities.Identity;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}