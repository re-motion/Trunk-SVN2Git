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
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  public class SqlCommandBuilder
  {
    private readonly StringBuilder _sb;
    private readonly List<CommandParameter> _parameters;

    private readonly List<Type> _supportedTypes;

    public SqlCommandBuilder ()
    {
      _sb = new StringBuilder();
      _parameters = new List<CommandParameter>();

      _supportedTypes = new List<Type>();
      _supportedTypes.Add (typeof (string));
      _supportedTypes.Add (typeof (int));
      _supportedTypes.Add (typeof (bool));
    }

    public void Append (string stringToAppend)
    {
      ArgumentUtility.CheckNotNull ("stringToAppend", stringToAppend);
      _sb.Append (stringToAppend);
    }

    public CommandParameter AddParameter (object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      if (IsSupportedType (value.GetType ()))
      {

        var parameter = new CommandParameter ("@" + (_parameters.Count + 1), value);
        _parameters.Add (parameter);
        return parameter;
      }
      throw new NotSupportedException (string.Format ("Specific type of '{0}' is not supported.", value.GetType().Name));
    } 

    public string GetCommandText ()
    {
      return _sb.ToString();
    }

    public CommandParameter[] GetCommandParameters ()
    {
      return _parameters.ToArray();
    }

    private bool IsSupportedType (Type type)
    {
      return _supportedTypes.Contains (type);
    }

  }
}