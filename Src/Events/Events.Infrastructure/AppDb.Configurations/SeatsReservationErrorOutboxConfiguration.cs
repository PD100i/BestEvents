using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.AppDb.Configurations
{
    public class SeatsReservationErrorOutboxConfiguration : IEntityTypeConfiguration<BookingEventMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу оутбокс для публикации сообщений об ошибках резервирования
        /// </summary>

        public void Configure(EntityTypeBuilder<BookingEventMessageEntity> builder)
        {
            builder.ToTable("seats_reservation_error_outbox");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id");

            builder.Property(m => m.BookingId)
                .HasColumnName("booking_id")
                .IsRequired();

            builder.HasIndex(m => m.BookingId)
                .IsUnique();

            builder.Property(m => m.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .HasColumnName("created_at");
        }
    }
}

