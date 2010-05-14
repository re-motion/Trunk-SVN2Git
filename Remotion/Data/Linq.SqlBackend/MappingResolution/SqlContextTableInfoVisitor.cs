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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  public class SqlContextTableInfoVisitor : ITableInfoVisitor
  {
    public static ITableInfo ApplyContext (ITableInfo tableInfo, SqlExpressionContext context, IMappingResolutionStage stage)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);
      ArgumentUtility.CheckNotNull ("stage", stage);

      var visitor = new SqlContextTableInfoVisitor (stage, context);
      return tableInfo.Accept (visitor);
    }

    private readonly IMappingResolutionStage _stage;
    private readonly SqlExpressionContext _context;

    protected SqlContextTableInfoVisitor (IMappingResolutionStage stage, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("stage", stage);

      _stage = stage;
      _context = context;
    }

    public ITableInfo VisitSimpleTableInfo (ResolvedSimpleTableInfo tableInfo)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);

      return tableInfo;
    }

    public ITableInfo VisitSubStatementTableInfo (ResolvedSubStatementTableInfo tableInfo)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);

      var newStatement = _stage.ApplyContext (tableInfo.SqlStatement, _context);
      if (newStatement != tableInfo.SqlStatement)
        return new ResolvedSubStatementTableInfo (tableInfo.TableAlias, newStatement);
      return tableInfo;
    }

    public ITableInfo VisitSqlJoinedTable (SqlJoinedTable joinedTable)
    {
      ArgumentUtility.CheckNotNull ("joinedTable", joinedTable);

      var newJoinInfo = _stage.ApplyContext (joinedTable.JoinInfo, _context); 
      joinedTable.JoinInfo = newJoinInfo;

      return joinedTable;
    }

    public ITableInfo VisitUnresolvedTableInfo (UnresolvedTableInfo tableInfo)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);

      return tableInfo;
    }
  }
}