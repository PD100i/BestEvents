using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Infrastructure.AppDb.Configurations
{
    public class SeatsReservationErrorOutboxConfiguration : IEntityTypeConfiguration<BookingMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу оутбокс для публикации сообщений об ошибках резервирования
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingMessageEntity> builder)
        {
            builder.ToTable("seats_reservation_error_outbox");
            builder.HasKey(b => b.BookingId);

            builder.Property(e => e.BookingId)
                .HasColumnName("id");

            builder.Property(b => b.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .HasColumnName("created_at");
        }
    }
}

