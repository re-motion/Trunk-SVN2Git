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
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  
  /// <summary>
  /// <see cref="SqlPreparationContext"/> is a helper class which maps <see cref="IQuerySource"/> to <see cref="SqlTable"/>.
  /// </summary>
  public class SqlPreparationContext : ISqlPreparationContext
  {
    private readonly ISqlPreparationContext _parentContext;
    private readonly SqlPreparationQueryModelVisitor _visitor;
    private readonly Dictionary<Expression, Expression> _mapping;

    public SqlPreparationContext () : this(null, null)
    {
      
    }

    public SqlPreparationContext (ISqlPreparationContext parentContext, SqlPreparationQueryModelVisitor visitor)
    {
      _parentContext = parentContext;
      _visitor = visitor;
      _mapping = new Dictionary<Expression, Expression>();
    }

    public int QuerySourceMappingCount
    {
      get { return _mapping.Count; }
    }

    public void AddContextMapping (Expression key, Expression value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _mapping[key] = value;
    }

    public Expression GetContextMapping (Expression key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      Expression result = TryGetContextMappingFromHierarchy (key);
      if (result != null) // search this context and parent context's for query source
        return result;

      if (_visitor != null)
      {
        // if whole hierarchy doesn't contain source, check whether it's a group join; group joins are lazily added
        var keyAsQuerySourceReferenceExpression = key as QuerySourceReferenceExpression;
        if (keyAsQuerySourceReferenceExpression != null)
        {
          var groupJoinClause = keyAsQuerySourceReferenceExpression.ReferencedQuerySource as GroupJoinClause;
          if (groupJoinClause != null)
            return new SqlTableReferenceExpression (_visitor.AddJoinClause (groupJoinClause.JoinClause));
        }
      }

      // nobody knows this source in the whole hierarchy, and it is no lazy join => error
      var message = string.Format (
           "The expression '{0}' could not be found in the list of processed expressions. Probably, the feature declaring '{0}' isn't "
           + "supported yet.",
           key.Type.Name);
      throw new KeyNotFoundException (message);

      //Expression result;
      //if (!_mapping.TryGetValue (key, out result))
      //{
      //  var message = string.Format (
      //      "The expression '{0}' could not be found in the list of processed expressions. Probably, the feature declaring '{0}' isn't "
      //      + "supported yet.", 
      //      key.Type.Name
      //   );
      //  throw new KeyNotFoundException (message);
      //}

      //return result;
    }

    public Expression TryGetContextMappingFromHierarchy (Expression key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      Expression result;
      if (_mapping.TryGetValue (key, out result))
        return result;

      if (_parentContext != null)
        return _parentContext.TryGetContextMappingFromHierarchy (key);
      else
        return null;

      //Expression result;
      //if(_mapping.TryGetValue (key, out result))
      //  return result;
      //return null;
    }
  }
}