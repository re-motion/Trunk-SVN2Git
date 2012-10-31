// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Ast.Compiler;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Replaces all occurences of <see cref="OriginalBodyExpression"/> and other expressions that can not be emitted by 
  /// our customized <see cref="LambdaCompiler"/>.
  /// </summary>
  public class UnemittableExpressionVisitor : TypePipeExpressionVisitorBase
  {
    private readonly MutableType _declaringType;
    private readonly IEmittableOperandProvider _emittableOperandProvider;

    public UnemittableExpressionVisitor (MutableType declaringType, IEmittableOperandProvider emittableOperandProvider)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);

      _declaringType = declaringType;
      _emittableOperandProvider = emittableOperandProvider;
    }

    protected internal override Expression VisitConstant (ConstantExpression node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      if (node.Value == null)
        return base.VisitConstant (node);

      var emittableValue = _emittableOperandProvider.GetEmittableOperand (node.Value);
      if (emittableValue != node.Value)
      {
        if (!node.Type.IsInstanceOfType (emittableValue))
          throw NewNotSupportedExceptionWithDescriptiveMessage (node);

        return Expression.Constant (emittableValue, node.Type);
      }

      return base.VisitConstant (node);
    }

    protected internal override Expression VisitLambda<T> (Expression<T> node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      var body = Visit (node.Body);

      // TODO: Collect should build a set, not a list of expressions to avoid key-already-exists exception below
      var thisExpressions = body.Collect<ThisExpression>();
      if (thisExpressions.Count == 0)
        return node.Update (body, node.Parameters);

      var thisClosureVariable = Expression.Variable (_declaringType, "thisClosure");
      var replacements = thisExpressions.ToDictionary (exp => (Expression) exp, exp => (Expression) thisClosureVariable);

      foreach (var baseCall in body.Collect<MethodCallExpression> (expr => expr.Method is NonVirtualCallMethodInfoAdapter))
      {
        var baseMethod = ((NonVirtualCallMethodInfoAdapter) baseCall.Method).AdaptedMethodInfo;
        var parameters = ParameterDeclaration.CreateForEquivalentSignature (baseMethod);
        var baseCallMethod = _declaringType.AddMethod (
            "xxx",
            MethodAttributes.Private,
            baseMethod.ReturnType,
            parameters,
            ctx => Expression.Call (ctx.This, new NonVirtualCallMethodInfoAdapter (baseMethod), ctx.Parameters.Cast<Expression>()));

        var baseCallReplacement = Expression.Call (thisClosureVariable, baseCallMethod, baseCall.Arguments);

        replacements.Add (baseCall, baseCallReplacement);
      }

      var newBody = body.Replace (replacements);
      var newLambda = node.Update (newBody, node.Parameters);

      var block = Expression.Block (
          new[] { thisClosureVariable },
          Expression.Assign (thisClosureVariable, CreateThisExpression()),
          newLambda);

      return Visit (block);
    }

    protected override Expression VisitOriginalBody (OriginalBodyExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var methodBase = expression.MethodBase;
      var thisExpression = methodBase.IsStatic ? null : CreateThisExpression();
      var methodRepresentingOriginalBody = AdaptOriginalMethodBase (methodBase);

      var baseCall = Expression.Call (thisExpression, methodRepresentingOriginalBody, expression.Arguments);

      return Visit (baseCall);
    }

    private MethodInfo AdaptOriginalMethodBase (MethodBase methodBase)
    {
      Assertion.IsTrue (methodBase is MethodInfo || methodBase is ConstructorInfo);

      var method = methodBase as MethodInfo ?? new ConstructorAsMethodInfoAdapter ((ConstructorInfo) methodBase);
      return new NonVirtualCallMethodInfoAdapter (method);
    }

    private ThisExpression CreateThisExpression ()
    {
      return new ThisExpression (_declaringType);
    }

    private Exception NewNotSupportedExceptionWithDescriptiveMessage (ConstantExpression node)
    {
      var message =
          string.Format (
              "It is not supported to have a ConstantExpression of type '{0}' because instances of '{0}' exist only at code generation "
              + "time, not at runtime." + Environment.NewLine
              + "To embed a reference to a generated method or type, construct the ConstantExpression as follows: "
              + "Expression.Constant (myMutableMethod, typeof (MethodInfo)) or "
              + "Expression.Constant (myMutableType, typeof (Type))." + Environment.NewLine
              + "To embed a reference to another reflection object, embed a method call to the Reflection APIs, like this: " + Environment.NewLine
              + "Expression.Call (" + Environment.NewLine
              + "    Expression.Constant (myMutableField.DeclaringType, typeof (Type))," + Environment.NewLine
              + "    typeof (Type).GetMethod (\"GetField\", new[] {{ typeof (string), typeof (BindingFlags) }})," + Environment.NewLine
              + "    Expression.Constant (myMutableField.Name)," + Environment.NewLine
              + "    Expression.Constant (BindingFlags.NonPublic | BindingFlags.Instance)). (BindingFlags depend on the visibility of the member.)",
              node.Type.Name);

      return new NotSupportedException (message);
    }
  }
}