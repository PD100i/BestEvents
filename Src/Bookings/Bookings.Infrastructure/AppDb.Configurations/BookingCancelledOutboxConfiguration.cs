using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure
{
    public class BookingCancelledOutboxConfiguration : IEntityTypeConfiguration<BookingEventMessageEntity>
    {
        /// <summary>
        /// Конфигурирует сущность BookingEntity, задавая имя таблицы, ключи, свойства и связи
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingEventMessageEntity> builder)
        {
            builder.ToTable("booking_cancelled_outbox");
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
