/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
