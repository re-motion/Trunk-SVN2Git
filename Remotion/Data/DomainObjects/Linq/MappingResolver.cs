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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The implementation of <see cref="IMappingResolver"/>  for getting detailed information from re-store which is the underlying data source.
  /// </summary>
  public class MappingResolver : IMappingResolver
  {
    public AbstractTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[tableInfo.ItemType];
      if (classDefinition == null)
      {
        string message = string.Format ("The item type '{0}' does not identify a queryable table.", tableInfo.ItemType.Name);
        throw new UnmappedItemException (message);
      }
      else
      {
        var viewName = classDefinition.GetViewName ();
        return new ResolvedSimpleTableInfo (tableInfo.ItemType, viewName, generator.GetUniqueIdentifier ("t"));
      }
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      throw new NotImplementedException();
    }

    public Expression ResolveTableReferenceExpression (SqlTableReferenceExpression tableReferenceExpression, UniqueIdentifierGenerator generator)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[tableReferenceExpression.SqlTable.ItemType];

      var tableAlias = tableReferenceExpression.SqlTable.GetResolvedTableInfo().TableAlias;

      if (classDefinition == null)
        return null;

      var propertyInfo = typeof (DomainObject).GetProperty ("ID");
      var primaryKeyColumn = new SqlColumnExpression (propertyInfo.PropertyType, tableAlias, propertyInfo.Name);
      var columns = new List<SqlColumnExpression>();

      foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions ())
      {
          var storageSpecificName = propertyDefinition.StorageSpecificName;
          if (!string.IsNullOrEmpty (storageSpecificName))
            columns.Add (new SqlColumnExpression (propertyDefinition.PropertyType, tableAlias, storageSpecificName));
      }

      return new SqlEntityExpression (tableReferenceExpression.SqlTable.ItemType, primaryKeyColumn, columns.ToArray());
    }

    public Expression ResolveMemberExpression (SqlMemberExpression memberExpression, UniqueIdentifierGenerator generator)
    {
      throw new NotImplementedException();
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      throw new NotImplementedException();
    }
  }
}