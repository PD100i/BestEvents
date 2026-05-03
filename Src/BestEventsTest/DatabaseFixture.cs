using BestEvents;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace BestEventsTest
{
    public sealed class PostgreSqlFixture : IAsyncLifetime
    {
        public PostgreSqlContainer DbContainer { get; } = new PostgreSqlBuilder("postgres:15-alpine")
        .Build();

        public async ValueTask InitializeAsync()
        {
            await DbContainer.StartAsync();
        }

        public DbContextOptions<AppDbContext> GetOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(DbContainer.GetConnectionString())
                .Options;
        }

        public async ValueTask DisposeAsync()
        {
            await DbContainer.DisposeAsync();
        }
    }
}
