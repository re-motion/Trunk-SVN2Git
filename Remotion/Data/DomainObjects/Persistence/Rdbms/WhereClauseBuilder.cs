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
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Xml;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class WhereClauseBuilder
  {
    // types

    // static members and constants

    public static WhereClauseBuilder Create (ICommandBuilder builder, IDbCommand command)
    {
      return ObjectFactory.Create<WhereClauseBuilder>(true, ParamList.Create (builder, command));
    }

    // member fields

    private readonly ICommandBuilder _commandBuilder;
    private readonly IDbCommand _command;
    private readonly StringBuilder _whereClauseBuilder;

    // construction and disposing

    protected WhereClauseBuilder (ICommandBuilder commandBuilder, IDbCommand command)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("command", command);

      _commandBuilder = commandBuilder;
      _command = command;
      _whereClauseBuilder = new StringBuilder();
    }

    // methods and properties

    protected StringBuilder Builder
    {
      get { return _whereClauseBuilder; }
    }

    public virtual void Add (string columnName, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
      ArgumentUtility.CheckNotNull ("value", value);

      if (_whereClauseBuilder.Length > 0)
        _whereClauseBuilder.Append (" AND ");

      string parameterName = _commandBuilder.Provider.GetParameterName (columnName);
      _whereClauseBuilder.AppendFormat (
          "{0} = {1}",
          _commandBuilder.Provider.DelimitIdentifier (columnName),
          parameterName);
      _commandBuilder.AddCommandParameter (_command, columnName, value);
    }

    /// <summary>
    /// Defines a SQL IN expression matching the column defined by <paramref name="columnName"/> with the given <paramref name="values"/>. The
    /// values are embedded in an XML <see cref="IDataParameter"/>, so they must be convertable to <see cref="string"/> values via their
    /// <see cref="object.ToString"/> method.
    /// </summary>
    /// <param name="columnName">The name of the column to check.</param>
    /// <param name="values">The values to match the column against.</param>
    /// <param name="columnType">The database type of the column to check. The values must be convertable from the textual representation
    /// embedded in the XML file to this type.</param>
    public virtual void SetInExpression (string columnName, string columnType, object[] values)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("values", values);

      if (_whereClauseBuilder.Length > 0)
        throw new InvalidOperationException ("SetInExpression can only be used with an empty WhereClauseBuilder.");

      _whereClauseBuilder.AppendFormat ("{0} IN (", _commandBuilder.Provider.DelimitIdentifier (columnName));

      var xmlString = new StringBuilder ("<L>");

      foreach (var value in values)
        xmlString.Append ("<I>").Append (value).Append ("</I>");

      xmlString.Append ("</L>");
      
      string parameterName = _commandBuilder.Provider.GetParameterName (columnName);
      var parameter = _commandBuilder.AddCommandParameter (_command, parameterName, xmlString.ToString());
      parameter.DbType = DbType.Xml;

      _whereClauseBuilder.Append ("SELECT T.c.value('.', '").Append (columnType).Append ("') FROM ");
      _whereClauseBuilder.Append (parameterName);
      _whereClauseBuilder.Append (".nodes('/L/I') T(c)");

      _whereClauseBuilder.Append (")");
    }

    public override string ToString ()
    {
      return _whereClauseBuilder.ToString();
    }
  }
}
