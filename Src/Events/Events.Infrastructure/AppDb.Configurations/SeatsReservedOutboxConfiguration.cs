using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class SeatsReservedOutboxConfiguration : IEntityTypeConfiguration<BookingMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу оутбокс для публикации сообщения резервирования мест
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingMessageEntity> builder)
        {
            builder.ToTable("seats_reserved_outbox");
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

