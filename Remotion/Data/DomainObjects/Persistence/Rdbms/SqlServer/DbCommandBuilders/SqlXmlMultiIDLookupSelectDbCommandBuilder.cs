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
using System.Collections.Generic;
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="SqlXmlMultiIDLookupSelectDbCommandBuilder"/> builds a command that allows retrieving a set of records whose ID column is 
  /// contained in a range of <see cref="ObjectID"/> values.
  /// </summary>
  public class SqlXmlMultiIDLookupSelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _table;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly ObjectID[] _objectIDs;

    public SqlXmlMultiIDLookupSelectDbCommandBuilder (
        TableDefinition table,
        ISelectedColumnsSpecification selectedColumns,
        ObjectID[] objectIDs,
        ISqlDialect sqlDialect,
        IValueConverter valueConverter) : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      _table = table;
      _selectedColumns = selectedColumns;
      _objectIDs = objectIDs;
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      var command = commandExecutionContext.CreateDbCommand ();

      var statement = new StringBuilder ();
      AppendSelectClause (statement, _selectedColumns);
      AppendFromClause (statement, _table);

      var xmlString = new StringBuilder ("<L>");
      foreach (var value in GetObjectIDValueStrings (_objectIDs))
        xmlString.Append ("<I>").Append (value).Append ("</I>");
      xmlString.Append ("</L>");
      var xmlStringParameter = AddCommandParameter (command, _table.ObjectIDColumn.Name, xmlString.ToString());
      xmlStringParameter.DbType = DbType.Xml;

      statement.Append (" WHERE ");
      statement.Append (SqlDialect.DelimitIdentifier (_table.ObjectIDColumn.Name));
      statement.Append (" IN (");
      statement.Append ("SELECT T.c.value('.', '").Append (_table.ObjectIDColumn.StorageTypeInfo.StorageType).Append ("')");
      statement.Append (" FROM ");
      statement.Append (xmlStringParameter.ParameterName);
      statement.Append (".nodes('/L/I') T(c))");
      statement.Append (SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString ();
      return command;
    }

    private IEnumerable<string> GetObjectIDValueStrings (IEnumerable<ObjectID> objectIDs)
    {
      foreach (ObjectID t in objectIDs)
      {
        if (!ValueConverter.IsOfSameStorageProvider (t))
          throw new ArgumentException ("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.", "objectIDs");
        yield return t.Value.ToString();
      }
    }
  }
}