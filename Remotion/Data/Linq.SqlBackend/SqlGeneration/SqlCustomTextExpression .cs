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
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// TODO: add summary
  /// </summary>
  public class SqlCustomTextExpression : SqlCustomTextGeneratorExpressionBase
  {
    private readonly string _sqlText;
    private Type _expressionType;

    public SqlCustomTextExpression (string sqlText, Type expressionType)
    {
      ArgumentUtility.CheckNotNull ("sqlText", sqlText);
      ArgumentUtility.CheckNotNull ("expressionType", expressionType);

      _sqlText = sqlText;
      _expressionType = expressionType;
    }

    public override void Generate (SqlCommandBuilder commandBuilder, ExpressionTreeVisitor textGeneratingExpressionVisitor, ISqlGenerationStage stage)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("textGeneratingExpressionVisitor", textGeneratingExpressionVisitor);
      ArgumentUtility.CheckNotNull ("stage", stage);

      commandBuilder.Append (_sqlText);
    }
  }
}