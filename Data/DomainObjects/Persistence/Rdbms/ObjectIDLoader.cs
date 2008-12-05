// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
