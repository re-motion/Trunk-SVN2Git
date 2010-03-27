// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Data;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class MultiIDLookupCommandBuilder: CommandBuilder
  {
    private readonly string _selectColumns;
    private readonly string _entityName;
    private readonly ObjectID[] _objectIDs;

    // construction and disposing

    public MultiIDLookupCommandBuilder (
        RdbmsProvider provider,
        string selectColumns, 
        string entityName, 
        ObjectID[] ids)
      : base (provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
      ArgumentUtility.CheckNotNullOrEmpty ("entityName", entityName);
      ArgumentUtility.CheckNotNull ("ids", ids);

      _selectColumns = selectColumns;
      _entityName = entityName;
      _objectIDs = ids;
    }

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();
      var whereClauseBuilder = WhereClauseBuilder.Create (this, command);

      whereClauseBuilder.SetInExpression ("ID", Provider.GetIDColumnTypeName(), GetValueArrayForParameter (_objectIDs));

      command.CommandText = string.Format (
          "SELECT {0} FROM {1} WHERE {2}{3}",
          _selectColumns,
          Provider.DelimitIdentifier (_entityName),
          whereClauseBuilder,
          Provider.StatementDelimiter);

      return command;
    }

    private object[] GetValueArrayForParameter (ObjectID[] objectIDs)
    {
      var values = new object[objectIDs.Length];

      for (int i = 0; i < objectIDs.Length; i++)
      {
        if (!IsOfSameStorageProvider (objectIDs[i]))
          throw new ArgumentException ("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.", "objectIDs");
        values[i] = objectIDs[i].Value;
      }

      return values;
    }
  }
}
