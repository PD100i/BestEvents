using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class BookingCancelledInboxConfigutation : IEntityTypeConfiguration<BookingMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу инбокс для принятых сообщений отмены бронирования
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingMessageEntity> builder)
        {
            builder.ToTable("booking_cancelled_inbox");
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

