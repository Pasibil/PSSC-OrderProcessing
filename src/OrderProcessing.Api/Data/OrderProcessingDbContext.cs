using Microsoft.EntityFrameworkCore;

namespace OrderProcessing.Api.Data
{
    public class OrderProcessingDbContext : DbContext
    {
        public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderLineEntity> OrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure OrderEntity
            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.PlacedAt)
                    .IsRequired();

                // One-to-many relationship
                entity.HasMany(e => e.OrderLines)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderLineEntity
            modelBuilder.Entity<OrderLineEntity>(entity =>
            {
                entity.ToTable("OrderLines");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Quantity)
                    .IsRequired();
                
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.LineTotal)
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}
