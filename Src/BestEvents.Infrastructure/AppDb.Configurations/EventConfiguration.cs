using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Конфигурация сущности EventEntity
    /// </summary>
    public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        /// <summary>
        /// Конфигурирует сущность EventEntity для Entity Framework Core, задавая правила отображения на базу данных.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.ToTable("events");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            builder.Property(e => e.StartAt)
                .HasColumnName("start_at")
                .IsRequired();

            builder.Property(e => e.EndAt)
                .HasColumnName("end_at")
                .IsRequired();

            builder.Property(e => e.TotalSeats)
                .HasColumnName("total_seats")
                .IsRequired();

            builder.Property(e => e.AvailableSeats)
                .HasColumnName("available_seats")
                .IsRequired();

            builder.HasMany(e => e.Bookings)
                .WithOne(b => b.Event)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
