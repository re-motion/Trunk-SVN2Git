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
using System.Linq.Expressions;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// <see cref="SqlCommandData"/> contains the SQL command text and parameters generated for a LINQ query.
  /// </summary>
  public struct SqlCommandData
  {
    private readonly string _commandText;
    private readonly CommandParameter[] _parameters;
    private readonly Expression<Func<IDatabaseResultRow, object>> _inMemoryProjection;

    public SqlCommandData (string commandText, CommandParameter[] parameters) : this (commandText, parameters, null)
    {
    }

    public SqlCommandData (string commandText, CommandParameter[] parameters, Expression<Func<IDatabaseResultRow, object>> inMemoryProjection)
    {
      ArgumentUtility.CheckNotNull ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      
      _commandText = commandText;
      _parameters = parameters;
      _inMemoryProjection = inMemoryProjection;
    }

    public string CommandText
    {
      get { return _commandText; }
    }

    public CommandParameter[] Parameters
    {
      get { return _parameters; }
    }

    public Expression<Func<IDatabaseResultRow, object>> InMemoryProjection
    {
      get { return _inMemoryProjection; }
    }
  }
}