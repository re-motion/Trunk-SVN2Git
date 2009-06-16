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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Utilities;
using System;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The class extends <see cref="QueryProviderBase"/>.
  /// </summary>
  public class DomainObjectQueryProvider : QueryProviderBase
  {
    public DomainObjectQueryProvider (DomainObjectQueryExecutor executor)
        : base (executor)
    {
    }

    /// <summary>
    /// The method returns a specific instance of <see cref="IQueryable"/>.
    /// </summary>
    /// <typeparam name="T">The type of the expression.</typeparam>
    /// <param name="expression">The query as expression chain.</param>
    /// <returns></returns>
    protected override IQueryable<T> CreateQueryable<T> (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      Type queryableType = typeof (DomainObjectQueryable<>).MakeGenericType (typeof (T));
      return (IQueryable<T>) Activator.CreateInstance (queryableType, this, expression);
    }

    
  }
}
