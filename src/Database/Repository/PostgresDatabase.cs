using System.Data;
using EmitService.Database.Interface;

namespace EmitService.Database.Repository
{
  public class PostgresDatabase : IDatabase
  {
    private readonly IConfiguration _configuration;

    public PostgresDatabase(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IDbConnection GetDbConnection()
    {
      return new Npgsql.NpgsqlConnection(_configuration.GetConnectionString("DatabaseEmit"));
    }
  }

}
