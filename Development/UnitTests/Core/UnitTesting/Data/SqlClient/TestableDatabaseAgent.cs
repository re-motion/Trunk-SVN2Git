using System.Data;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Data.SqlClient
{
  public class TestableDatabaseAgent : DatabaseAgent
  {
    private readonly IDbConnection _connection;

    public TestableDatabaseAgent (IDbConnection connection) : base ("blabla")
    {
      _connection = connection;
    }

    protected override IDbConnection CreateConnection ()
    {
      return _connection;
    }
  }
}