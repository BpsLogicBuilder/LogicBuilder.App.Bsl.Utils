using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace LogicBuilder.App.Bsl.Utils.Tests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server@sha256:d975fb1ea4c25c95309d7813b9f9133cde880fda16da24ab1379e277a9b2395b")//2026-latest
                                .Build();

        public string GetConnectionString(string initialCatalog)
        {
            return new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
            {
                InitialCatalog = initialCatalog
            }.ToString();
        }

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_msSqlContainer != null)
                await _msSqlContainer.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
