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
using System.Data;
using System.Text;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class WhereClauseBuilder
  {
    // types

    // static members and constants

    public static WhereClauseBuilder Create (CommandBuilder builder, IDbCommand command)
    {
      return ObjectFactory.Create<WhereClauseBuilder>().With (builder, command);
    }

    // member fields

    private readonly CommandBuilder _commandBuilder;
    private readonly IDbCommand _command;
    private readonly StringBuilder _whereClauseBuilder;

    // construction and disposing

    public WhereClauseBuilder (CommandBuilder commandBuilder, IDbCommand command)
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

    public virtual void SetInExpression (string columnName, object[] values)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("values", values);

      if (_whereClauseBuilder.Length > 0)
        throw new InvalidOperationException ("SetInExpression can only be used with an empty WhereClauseBuilder.");

      _whereClauseBuilder.AppendFormat ("{0} IN (", _commandBuilder.Provider.DelimitIdentifier (columnName));

      for (int i = 0; i < values.Length; i++)
      {
        if (i > 0)
          _whereClauseBuilder.Append (", ");

        string incrementedColumnName = string.Format ("{0}{1}", columnName, i + 1);
        string parameterName = _commandBuilder.Provider.GetParameterName (incrementedColumnName);

        _whereClauseBuilder.Append (parameterName);
        _commandBuilder.AddCommandParameter (_command, parameterName, values[i]);
      }

      _whereClauseBuilder.Append (")");
    }

    public override string ToString ()
    {
      return _whereClauseBuilder.ToString();
    }
  }
}
