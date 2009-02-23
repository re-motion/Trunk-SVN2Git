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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.Structure;

namespace Remotion.Data.DomainObjects.Linq
{
  public static class ExtensionMethods
  {
    public static ObjectList<T> ToObjectList<T> (this IEnumerable<T> source) 
        where T : DomainObject
    {
      ObjectList<T> result = new ObjectList<T>();
      foreach (T item in source)
        result.Add (item);
      return result;
    }

    //public static QueryModel Fetch<T, TR> (this IEnumerable<T> source, Expression<Func<T, TR>> selector) where T : DomainObject
    //{
    //  var ex = ((IQueryable) source).Expression;
    //  QueryParser parser = new QueryParser (ex);
    //  FromExpressionData expressionData = new FromExpressionData (selector, selector.Parameters[0]);
    //  QueryModel model = parser.GetParsedQueryFetch (expressionData);
    //  return model;
    //}
  }
}
