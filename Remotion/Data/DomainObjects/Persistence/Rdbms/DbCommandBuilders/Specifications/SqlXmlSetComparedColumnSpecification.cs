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
using System.IO;
using System.Text;
using System.Xml;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// The <see cref="SqlXmlSetComparedColumnSpecification"/> builds a specification command that allows retrieving a set of records.
  /// </summary>
  public class SqlXmlSetComparedColumnSpecification : IComparedColumnsSpecification
  {
    private readonly ColumnDefinition _columnDefinition;
    private readonly object[] _objectValues;

    public SqlXmlSetComparedColumnSpecification (ColumnDefinition columnDefinition, IEnumerable<object> objectValues)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull ("objectValues", objectValues);

      _columnDefinition = columnDefinition;
      _objectValues = objectValues.ToArray();
    }

    public ColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public object[] ObjectValues
    {
      get { return _objectValues; }
    }

    public void AppendComparisons (StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      var stringWriter = new StringWriter (new StringBuilder());
      var xmlWriter = new XmlTextWriter (stringWriter);
      xmlWriter.WriteStartElement ("L");
      foreach (var value in ObjectValues)
      {
        xmlWriter.WriteStartElement ("I");
        xmlWriter.WriteString (value == null ? null : value.ToString());
        xmlWriter.WriteEndElement ();
      }
      xmlWriter.WriteEndElement();
      
      var parameter = _columnDefinition.StorageTypeInfo.CreateDataParameter (command, stringWriter.ToString());
      parameter.ParameterName = sqlDialect.GetParameterName (_columnDefinition.Name);
      parameter.DbType = DbType.Xml;
      command.Parameters.Add (parameter);

      statement.Append (sqlDialect.DelimitIdentifier (_columnDefinition.Name));
      statement.Append (" IN (");
      statement.Append ("SELECT T.c.value('.', '").Append (_columnDefinition.StorageTypeInfo.StorageTypeName).Append ("')");
      statement.Append (" FROM ");
      statement.Append (parameter.ParameterName);
      statement.Append (".nodes('/L/I') T(c))");
      statement.Append (sqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString ();
    }
  }
}