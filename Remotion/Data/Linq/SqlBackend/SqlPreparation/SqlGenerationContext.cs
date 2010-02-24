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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// <see cref="SqlGenerationContext"/> is a helper class which maps <see cref="IQuerySource"/> to <see cref="SqlTable"/>.
  /// </summary>
  public class SqlGenerationContext
  {
    private readonly Dictionary<IQuerySource, SqlTable> _mapping;

    public SqlGenerationContext ()
    {
      _mapping = new Dictionary<IQuerySource, SqlTable>();
    }

    public void AddQuerySourceMapping (IQuerySource source, SqlTable sqlTable)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);

      _mapping.Add (source, sqlTable);
    }

    public Dictionary<IQuerySource, SqlTable> GetQuerySourceMapping ()
    {
      return _mapping;
    }

    public SqlTable GetSqlTable (IQuerySource source)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      if (_mapping.ContainsKey (source))
        return _mapping[source];
      throw new KeyNotFoundException();
    }
  }
}