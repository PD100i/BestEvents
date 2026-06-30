using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace BestEvents.Infrastructure
{
    /// <summary>
    /// Конфигурация сущности BookingEntity для EntityFrameworkCore
    /// </summary>
    public class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
    {
        /// <summary>
        /// Конфигурирует сущность BookingEntity, задавая имя таблицы, ключи, свойства и связи
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<BookingEntity> builder)
        {
            builder.ToTable("bookings");
            builder.HasKey(b => b.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(b => b.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(b => b.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(b => b.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(b => b.ProcessedAt)
                .HasColumnName("processed_at");

            builder.HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}