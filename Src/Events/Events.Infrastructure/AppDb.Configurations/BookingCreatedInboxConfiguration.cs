using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.AppDb.Configurations
{
    public class BookingCreatedInboxConfiguration : IEntityTypeConfiguration<BookingMessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу инбокс для принятых сообщений создания бронирования
        /// </summary>
        /// <param name="booking_created_outbox"></param>
        public void Configure(EntityTypeBuilder<BookingMessageEntity> builder)
        {
            builder.ToTable("booking_created_inbox");
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