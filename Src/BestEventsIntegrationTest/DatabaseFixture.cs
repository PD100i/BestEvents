using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using BestEvents;

namespace BestEventsIntegrationTest
{
    public sealed class DatabaseFixture : IAsyncLifetime
    {
        public PostgreSqlContainer DbContainer { get; } = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("DbBestEventsTest")
        .Build();

        

        public async ValueTask InitializeAsync()
        {
            await DbContainer.StartAsync();
            var context = CreateContext();
            await context.Database.EnsureCreatedAsync();
        }


        public async ValueTask DisposeAsync()
        {
            await DbContainer.DisposeAsync();
        }

        public AppDbContext CreateContext()
        {
            var options = GetOptions();
            var context = new AppDbContext(options);
            return context;
        }

        public async Task ResetDatabaseAsync()
        {
            NpgsqlConnection.ClearAllPools();
            var context = CreateContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        private DbContextOptions<AppDbContext> GetOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(DbContainer.GetConnectionString())
                .Options;
        }

    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
