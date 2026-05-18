using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsIntegrationTest
{
    [Collection("Database collection")]
    public class MigrationTest
    {
        readonly DatabaseFixture _fixture;
        public MigrationTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Migrate_ShouldCreateExpectedSсheme()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            await context.Database.EnsureDeletedAsync(CancellationToken.None);

            // Act
            await context.Database.MigrateAsync(CancellationToken.None);
            
            //Assert
            using var assertContext = _fixture.CreateContext();
            var model = assertContext.Model.GetRelationalModel();

            var tables = model.Tables.Select(t => t.Name).ToList();
            Assert.Contains("events", tables);
            Assert.Contains("bookings", tables);

            var eventsTable = model.Tables.FirstOrDefault(t => t.Name == "events");
            Assert.NotNull(eventsTable);
            var eventsPK = eventsTable.PrimaryKey;
            Assert.NotNull(eventsPK);
            var eventsPkColumn = Assert.Single(eventsPK.Columns);
            Assert.NotNull(eventsPkColumn);
            Assert.Equal("id", eventsPkColumn.Name);

            var bookingTable = model.Tables.FirstOrDefault(t => t.Name == "bookings");
            Assert.NotNull(bookingTable);
            var bookingPK = bookingTable.PrimaryKey; 
            Assert.NotNull(bookingPK);
            var bookingPkColumn = Assert.Single(bookingPK.Columns);
            Assert.NotNull(bookingPkColumn);
            Assert.Equal("id", bookingPkColumn.Name);
            var hasRequiredForeignKey = bookingTable.ForeignKeyConstraints.Any(fk =>
             fk.PrincipalTable.Name == "events" &&
             fk.Columns.Any(c => c.Name == "event_id"));
            Assert.True(hasRequiredForeignKey);
        }
    }
}
