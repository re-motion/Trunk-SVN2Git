using System;
using System.Collections.Generic;
using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class ObjectIDLoader
  {
    private readonly RdbmsProvider _provider;

    public ObjectIDLoader (RdbmsProvider provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      _provider = provider;
    }

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public List<ObjectID> LoadObjectIDsFromCommandBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (IDbCommand command = commandBuilder.Create())
      {
        if (command == null)
          return new List<ObjectID>();

        using (IDataReader reader = Provider.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          List<ObjectID> objectIDsInCorrectOrder = new List<ObjectID>();
          ValueConverter valueConverter = Provider.CreateValueConverter();
          while (reader.Read())
            objectIDsInCorrectOrder.Add (valueConverter.GetID (reader));

          return objectIDsInCorrectOrder;
        }
      }
    }
  }
}