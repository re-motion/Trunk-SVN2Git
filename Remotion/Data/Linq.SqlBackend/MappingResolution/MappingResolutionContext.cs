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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="MappingResolutionContext"/> holds context information required during mapping resolution stage.
  /// </summary>
  public class MappingResolutionContext : IMappingResolutionContext
  {
    private readonly Dictionary<SqlEntityExpression, SqlTableBase> _entityMapping;
    private readonly Dictionary<SqlGroupingSelectExpression, SqlTableBase> _groupReferenceMapping;

    public MappingResolutionContext ()
    {
      _entityMapping = new Dictionary<SqlEntityExpression, SqlTableBase>();
      _groupReferenceMapping = new Dictionary<SqlGroupingSelectExpression, SqlTableBase>();
    }

    public void AddSqlEntityMapping (SqlEntityExpression entityExpression, SqlTableBase sqlTable)
    {
      ArgumentUtility.CheckNotNull ("entityExpression", entityExpression);
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);

      _entityMapping[entityExpression] = sqlTable;
    }

    public void AddGroupReferenceMapping (SqlGroupingSelectExpression groupingSelectExpression, SqlTableBase sqlTable)
    {
      ArgumentUtility.CheckNotNull ("expression", groupingSelectExpression);
      ArgumentUtility.CheckNotNull ("table", sqlTable);

      _groupReferenceMapping[groupingSelectExpression] = sqlTable;
    }

    public SqlTableBase GetSqlTableForEntityExpression (SqlEntityExpression entityExpression)
    {
      ArgumentUtility.CheckNotNull ("entityExpression", entityExpression);

      SqlTableBase result;
      if (_entityMapping.TryGetValue (entityExpression, out result))
        return result;

      throw new InvalidOperationException (string.Format ("No associated table found for entity '{0}'.", entityExpression.Type.Name));
    }

    public SqlTableBase GetReferencedGroupSource (SqlGroupingSelectExpression groupingSelectExpression)
    {
      ArgumentUtility.CheckNotNull ("groupingSelectExpression", groupingSelectExpression);
      SqlTableBase result;
      if (_groupReferenceMapping.TryGetValue (groupingSelectExpression, out result))
        return result;

      throw new InvalidOperationException (
          string.Format ("No associated table found for grouping select expression '{0}'.", groupingSelectExpression.Type.Name));
    }

    // TODO: Test this method
    public SqlEntityExpression UpdateEntityAndAddMapping (SqlEntityExpression entityExpression, Type itemType, string tableAlias, string newName)
    {
      ArgumentUtility.CheckNotNull ("entityExpression", entityExpression);
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      ArgumentUtility.CheckNotNullOrEmpty ("tableAlias", tableAlias);

      var newEntityExpression = entityExpression.Update (itemType, tableAlias, newName);
      var tableForEntityExpression = GetSqlTableForEntityExpression (entityExpression);
      AddSqlEntityMapping (newEntityExpression, tableForEntityExpression);
      return newEntityExpression;
    }

    public SqlGroupingSelectExpression UpdateGroupingSelectAndAddMapping (
        SqlGroupingSelectExpression expression, Expression newKey, Expression newElement, IEnumerable<Expression> aggregations)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("newKey", newKey);
      ArgumentUtility.CheckNotNull ("newElement", newElement);
      ArgumentUtility.CheckNotNull ("aggregations", aggregations);

      var newSqlGroupingSelectExpression = expression.Update (newKey, newElement, aggregations);
      var tableForGroupingSelectExpression = GetReferencedGroupSource (expression);
      AddGroupReferenceMapping (newSqlGroupingSelectExpression, tableForGroupingSelectExpression);
      return newSqlGroupingSelectExpression;
    }
  }
}