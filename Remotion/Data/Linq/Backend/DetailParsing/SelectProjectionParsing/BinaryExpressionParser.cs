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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Backend.DetailParser.SelectProjectionParsing
{
  public class BinaryExpressionParser : ISelectProjectionParser
  {
    private readonly SelectProjectionParserRegistry _parserRegistry;

    public Dictionary<ExpressionType, BinaryEvaluation.EvaluationKind> NodeTypeMap { get; private set; }

    public BinaryExpressionParser (SelectProjectionParserRegistry parserRegistry)
    {
      ArgumentUtility.CheckNotNull ("parserRegistry", parserRegistry);

      _parserRegistry = parserRegistry;
      NodeTypeMap = new Dictionary<ExpressionType, BinaryEvaluation.EvaluationKind>
                    {
                        { ExpressionType.Add, BinaryEvaluation.EvaluationKind.Add },
                        { ExpressionType.Divide, BinaryEvaluation.EvaluationKind.Divide },
                        { ExpressionType.Modulo, BinaryEvaluation.EvaluationKind.Modulo },
                        { ExpressionType.Multiply, BinaryEvaluation.EvaluationKind.Multiply },
                        { ExpressionType.Subtract, BinaryEvaluation.EvaluationKind.Subtract }
                    };
    }

    public virtual IEvaluation Parse (BinaryExpression binaryExpression, ParseContext parseContext)
    {
      ArgumentUtility.CheckNotNull ("binaryExpression", binaryExpression);
      ArgumentUtility.CheckNotNull ("parseContext", parseContext);

      IEvaluation leftSide = _parserRegistry.GetParser (binaryExpression.Left).Parse (binaryExpression.Left, parseContext);
      IEvaluation rightSide = _parserRegistry.GetParser (binaryExpression.Right).Parse (binaryExpression.Right, parseContext);

      BinaryEvaluation.EvaluationKind evaluationKind;
      if (!NodeTypeMap.TryGetValue (binaryExpression.NodeType, out evaluationKind))
      {
        throw new ParserException (
            GetSupportedNodeTypeString(),
            binaryExpression.NodeType,
            "binary expression in select projection");
      }
      return new BinaryEvaluation (leftSide, rightSide, evaluationKind);
    }

    IEvaluation ISelectProjectionParser.Parse (Expression expression, ParseContext parseContext)
    {
      return Parse ((BinaryExpression) expression, parseContext);
    }

    private string GetSupportedNodeTypeString ()
    {
      return SeparatedStringBuilder.Build (", ", NodeTypeMap.Keys);
    }

    public bool CanParse (Expression expression)
    {
      return expression is BinaryExpression;
    }
  }
}