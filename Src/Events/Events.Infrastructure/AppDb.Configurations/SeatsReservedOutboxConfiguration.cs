using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class SeatsReservedOutboxConfiguration : IEntityTypeConfiguration<BookingEventMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу оутбокс для публикации сообщения резервирования мест
        /// </summary>

        public void Configure(EntityTypeBuilder<BookingEventMessageEntity> builder)
        {
            builder.ToTable("seats_reserved_outbox");

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

