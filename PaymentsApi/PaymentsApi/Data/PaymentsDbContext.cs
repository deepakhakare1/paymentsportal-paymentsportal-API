using Microsoft.EntityFrameworkCore;
using PaymentsApi.Models;
namespace PaymentsApi.Data
{
    public class PaymentsDbContext:DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<DailySequence> DailySequences { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(b =>
            {
                b.HasKey(p => p.Id);
                b.HasIndex(p => p.ClientRequestId).IsUnique();
                b.HasIndex(p => p.Reference).IsUnique();
                b.Property(p => p.Reference).HasMaxLength(50).IsRequired();
                b.Property(p => p.ClientRequestId).HasMaxLength(100).IsRequired();
                b.Property(p => p.Currency).HasMaxLength(3).IsRequired();
                b.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            });
            modelBuilder.Entity<DailySequence>(b =>
            {
                b.HasKey(d => d.DateKey);
                b.Property(d => d.DateKey).HasMaxLength(10);
            });

        }
    }
}
