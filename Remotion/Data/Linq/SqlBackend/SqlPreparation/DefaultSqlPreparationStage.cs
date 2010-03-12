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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// Provides a default implementation of <see cref="ISqlPreparationStage"/>.
  /// </summary>
  public class DefaultSqlPreparationStage : ISqlPreparationStage
  {
    private readonly SqlPreparationContext _context;

    public DefaultSqlPreparationStage (SqlPreparationContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public Expression PrepareSelectExpression (Expression expression)
    {
      return SqlPreparationExpressionVisitor.TranslateExpression (expression, _context);
    }

    public Expression PrepareWhereExpression (Expression expression)
    {
      return SqlPreparationExpressionVisitor.TranslateExpression (expression, _context);
    }

    public Expression PrepareTopExpression (Expression expression)
    {
      return SqlPreparationExpressionVisitor.TranslateExpression (expression, _context);
    }

    public Expression PrepareFromExpression (Expression expression)
    {
      return SqlPreparationExpressionVisitor.TranslateExpression (expression, _context);
    }

    public SqlTableBase GetTableForFromExpression (Expression fromExpression, Type itemType)
    {
      return SqlPreparationFromExpressionVisitor.GetTableForFromExpression (fromExpression, itemType);
    }
  }
}