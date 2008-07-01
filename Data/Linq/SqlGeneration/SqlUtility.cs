using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.Linq.SqlGeneration
{
  public static class SqlUtility
  {
    public static IDbCommand CreateCommand (string commandText, CommandParameter[] parameters, IDatabaseInfo databaseInfo, IDbConnection connection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      ArgumentUtility.CheckNotNull ("connection", connection);

      IDbCommand command = connection.CreateCommand ();
      command.CommandText = commandText;
      command.CommandType = CommandType.Text;

      foreach (CommandParameter parameter in parameters)
        command.Parameters.Add (parameter.Value);

      return command;
    }
  }
}