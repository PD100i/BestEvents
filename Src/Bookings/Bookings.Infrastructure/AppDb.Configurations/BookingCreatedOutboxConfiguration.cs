using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Bookings.Infrastructure
{
    public class BookingCreatedOutboxConfiguration : IEntityTypeConfiguration<BookingMessage>
    {
        /// <summary>
        /// Конфигурирует сущность BookingEntity, задавая имя таблицы, ключи, свойства и связи
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingMessage> builder)
        {
            builder.ToTable("booking_created_outbox");
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
