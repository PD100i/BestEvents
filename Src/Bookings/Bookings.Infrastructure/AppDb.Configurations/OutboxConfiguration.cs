using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Bookings.Infrastructure
{
    public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessageEntity>
    {
        /// <summary>
        /// Конфигурирует сущность BookingEntity, задавая имя таблицы, ключи, свойства и связи
        /// </summary>

        public void Configure(EntityTypeBuilder<OutboxMessageEntity> builder)
        {
            builder.ToTable("outbox");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id");

            builder.Property(m => m.BookingId)
                .HasColumnName("booking_id")
                .IsRequired();

            builder.Property(m => m.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(m => m.MessageType)
                .HasColumnName("message_type")
                .IsRequired();
        }
    }
}
