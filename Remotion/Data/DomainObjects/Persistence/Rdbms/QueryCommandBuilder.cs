// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class QueryCommandBuilder: CommandBuilder
  {
    // types

    // static members and constants

    // member fields

    private IQuery _query;

    // construction and disposing

    public QueryCommandBuilder (RdbmsProvider provider, IQuery query)
        : base (provider)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      _query = query;
    }

    // methods and properties

    public override IDbCommand Create()
    {
      IDbCommand command = Provider.CreateDbCommand();

      string statement = _query.Statement;
      foreach (QueryParameter parameter in _query.Parameters)
      {
        if (parameter.ParameterType == QueryParameterType.Text)
          statement = statement.Replace (parameter.Name, parameter.Value.ToString());
        else
          AddCommandParameter (command, parameter.Name, parameter.Value);
      }

      command.CommandText = statement;
      return command;
    }

    protected override void AppendColumn (string columnName, string parameterName)
    {
      throw new NotSupportedException ("'AppendColumn' is not supported by 'QueryCommandBuilder'.");
    }
  }
}
