/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.Linq.VisitorTest.ExpressionTreeVisitorTest
{
  [TestFixture]
  public class ExpressionTreeVisitor_BinaryExpressionsTest : ExpressionTreeVisitor_SpecificExpressionsTestBase
  {
    [Test]
    public void VisitBinaryExpression_Unchanged ()
    {
      BinaryExpression expression = (BinaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Add);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (expression.Left);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.AreSame (expression, result);
    }

    [Test]
    public void VisitBinaryExpression_LeftChanged ()
    {
      BinaryExpression expression = (BinaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Subtract);
      Expression newOperand = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (newOperand);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.AreNotSame (expression, result);
      Assert.AreEqual (ExpressionType.Subtract, result.NodeType);
      Assert.AreSame (newOperand, result.Left);
      Assert.AreSame (expression.Right, result.Right);
    }

    [Test]
    public void VisitBinaryExpression_RightChanged ()
    {
      BinaryExpression expression = (BinaryExpression) ExpressionInstanceCreator.GetExpressionInstance (ExpressionType.Subtract);
      Expression newOperand = Expression.Constant (1);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (expression.Left);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (newOperand);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.AreNotSame (expression, result);
      Assert.AreEqual (ExpressionType.Subtract, result.NodeType);
      Assert.AreSame (expression.Left, result.Left);
      Assert.AreSame (newOperand, result.Right);
    }

    [Test]
    public void VisitBinaryExpression_ConversionExpression_Unchanged ()
    {
      ParameterExpression conversionParameter = Expression.Parameter (typeof (string), "s");
      LambdaExpression conversion = Expression.Lambda (conversionParameter, conversionParameter);
      BinaryExpression expression = Expression.MakeBinary (
          ExpressionType.Coalesce,
          Expression.Constant ("0"),
          Expression.Constant ("0"),
          false,
          null,
          conversion);

      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (expression.Left);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.That (result, Is.SameAs (expression));
    }

    [Test]
    public void VisitBinaryExpression_ConversionExpression_Changed ()
    {
      ParameterExpression conversionParameter = Expression.Parameter (typeof (string), "s");
      LambdaExpression conversion = Expression.Lambda (conversionParameter, conversionParameter);
      BinaryExpression expression = Expression.MakeBinary (
          ExpressionType.Coalesce,
          Expression.Constant ("0"),
          Expression.Constant ("0"),
          false,
          null,
          conversion);

      LambdaExpression newConversion = Expression.Lambda (conversionParameter, conversionParameter);

      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (expression.Left);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (newConversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.That (result, Is.Not.SameAs (expression));
      Assert.That (result.Conversion, Is.SameAs (newConversion));
    }

    [Test]
    public void VisitBinaryExpression_RespectsIsLiftedToNull ()
    {
      MethodInfo method = ((Func<int, int, bool>) ((i1, i2) => i1 > i2)).Method;

      Expression left = Expression.Constant (0, typeof (int?));
      Expression right = Expression.Constant (0, typeof (int?));

      BinaryExpression expression = Expression.MakeBinary (ExpressionType.GreaterThan, left, right, true, method);

      Expression newOperand = Expression.Constant (1, typeof (int?));
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (newOperand);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.AreNotSame (expression, result);
      Assert.That (result.IsLiftedToNull, Is.True);
    }

    [Test]
    public void VisitBinaryExpression_RespectsMethod ()
    {
      MethodInfo method = ((Func<int, int, bool>) ((i1, i2) => i1 > i2)).Method;

      Expression left = Expression.Constant (0, typeof (int?));
      Expression right = Expression.Constant (0, typeof (int?));

      BinaryExpression expression = Expression.MakeBinary (ExpressionType.GreaterThan, left, right, true, method);

      Expression newOperand = Expression.Constant (1, typeof (int?));
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Left)).Return (newOperand);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Right)).Return (expression.Right);
      Expect.Call (InvokeVisitMethod ("VisitExpression", expression.Conversion)).Return (expression.Conversion);

      BinaryExpression result = (BinaryExpression) InvokeAndCheckVisitExpression ("VisitBinaryExpression", expression);
      Assert.AreNotSame (expression, result);
      Assert.That (result.Method, Is.SameAs (method));
    }
  }
}