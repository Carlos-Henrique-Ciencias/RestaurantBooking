using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Infrastructure.Persistence;

public class RestaurantBookingDbContext : DbContext
{
    public RestaurantBookingDbContext(DbContextOptions<RestaurantBookingDbContext> options) 
        : base(options) { }

    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração "Senior": Mapeamento fluente (nada de DataAnnotations na entidade)
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(r => r.CustomerEmail).IsRequired().HasMaxLength(150);
            // Index para busca rápida por código
            entity.HasIndex(r => r.Code).IsUnique();
        });
    }
}
