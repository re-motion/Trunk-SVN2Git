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
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// Acts as a base class for expression nodes that need to take part in the SQL generation peformed by <see cref="SqlGeneratingExpressionVisitor"/>.
  /// </summary>
  public abstract class SqlCustomTextGeneratorExpressionBase : ExtensionExpression
  {

    protected SqlCustomTextGeneratorExpressionBase (Type expressionType) : base(expressionType)
    {
    }

    public abstract void Generate (ISqlCommandBuilder commandBuilder, ExpressionTreeVisitor textGeneratingExpressionVisitor, ISqlGenerationStage stage);

    // TODO Review 2564: Test by deriving a TestableSqlCustomTextGeneratorExpression in the unit tests project and call Accept on it
    // TODO Review 2564: Create an integration test 
    
    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as ISqlCustomTextGeneratorExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlCustomTextGeneratorExpression (this);
      else
        return base.Accept (visitor);
    }
  }
}