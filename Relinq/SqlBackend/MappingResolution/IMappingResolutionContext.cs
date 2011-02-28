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
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;

namespace Remotion.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="IMappingResolutionContext"/> provides methods to handle a concrete mapping resolution context.
  /// </summary>
  // TODO Review: Consider removing this interface
  public interface IMappingResolutionContext
  {
    void AddSqlEntityMapping (SqlEntityExpression entityExpression, SqlTableBase sqlTable);
    void AddGroupReferenceMapping (SqlGroupingSelectExpression expression, SqlTableBase table);
    SqlTableBase GetSqlTableForEntityExpression (SqlEntityExpression entityExpression);
    SqlTableBase GetReferencedGroupSource (SqlGroupingSelectExpression groupingSelectExpression);
    SqlEntityExpression UpdateEntityAndAddMapping (SqlEntityExpression entityExpression, Type itemType, string tableAlias, string newName);
    SqlGroupingSelectExpression UpdateGroupingSelectAndAddMapping (
        SqlGroupingSelectExpression expression, Expression newKey, Expression newElement, IEnumerable<Expression> aggregations);
    void AddSqlTable (SqlTableBase sqlTableBase, SqlStatementBuilder sqlStatementBuilder);
  }
}