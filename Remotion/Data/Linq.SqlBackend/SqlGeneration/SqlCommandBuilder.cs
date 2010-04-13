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
using System.Text;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  public class SqlCommandBuilder
  {
    private readonly StringBuilder _stringBuilder;
    private readonly List<CommandParameter> _parameters;

    public SqlCommandBuilder ()
    {
      _stringBuilder = new StringBuilder();
      _parameters = new List<CommandParameter>();
    }

    public CommandParameter CreateParameter (object value)
    {
      var parameter = new CommandParameter ("@" + (_parameters.Count + 1), value);
      _parameters.Add (parameter);

      return parameter;
    }

    public void Append (string stringToAppend)
    {
      ArgumentUtility.CheckNotNull ("stringToAppend", stringToAppend);
      _stringBuilder.Append (stringToAppend);
    }

    public void AppendSeparated<T> (string separator, IEnumerable<T> values, Action<SqlCommandBuilder, T> appender)
    {
      ArgumentUtility.CheckNotNull ("separator", separator);
      ArgumentUtility.CheckNotNull ("values", values);
      ArgumentUtility.CheckNotNull ("appender", appender);

      bool first = true;
      foreach (T value in values)
      {
        if (!first)
          _stringBuilder.Append (separator);
        first = false;
        appender (this, value);
      }
    }

    public void AppendIdentifier (string identifier)
    {
      AppendFormat ("[{0}]", identifier);
    }

    public void AppendFormat (string stringToAppend, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("stringToAppend", stringToAppend);

      _stringBuilder.AppendFormat (stringToAppend, parameters);
    }

    public CommandParameter AppendParameter (object value)
    {
      var parameter = CreateParameter (value);
      Append (parameter.Name);
      return parameter;
    }

    public string GetCommandText ()
    {
      return _stringBuilder.ToString();
    }

    public CommandParameter[] GetCommandParameters ()
    {
      return _parameters.ToArray();
    }

    public SqlCommandData GetCommand ()
    {
      return new SqlCommandData (GetCommandText(), GetCommandParameters());
    }
  }
}