using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class InboxConfigutation : IEntityTypeConfiguration<MessageEntity>
    {
        /// <summary>
        /// Конфигурирует таблицу инбокс для принятых сообщений отмены бронирования
        /// </summary>

        public void Configure(EntityTypeBuilder<MessageEntity> builder)
        {
            builder.ToTable("inbox");
            builder.HasKey(b => b.BookingId);

            
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

