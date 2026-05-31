using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Products.API.HealthChecks;
public class SqliteHealthCheck : IHealthCheck
{
private readonly IConfiguration _config;
public SqliteHealthCheck(IConfiguration config) => _config = config;
public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
{
try {
var connectionString = _config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db";
using var conn = new SqliteConnection(connectionString);
await conn.OpenAsync(cancellationToken);
await conn.ExecuteScalarAsync<int>("SELECT 1");
return HealthCheckResult.Healthy("Conexión a SQLite exitosa.");
} catch (Exception ex) { return HealthCheckResult.Unhealthy("Fallo al conectar con SQLite.", ex); }
}
}
