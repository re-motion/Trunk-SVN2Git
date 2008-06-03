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
