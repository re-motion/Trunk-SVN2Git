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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing.Structure.Legacy;
using Remotion.Data.UnitTests.Linq.Parsing.Structure.WhereExpressionParserTests;
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.SelectManyExpressionParserTests
{
  [TestFixture]
  public class RecursiveSelectManyExpressionParserTest
  {
    private IQueryable<Student> _querySource1;
    private IQueryable<Student> _querySource2;
    private IQueryable<Student> _querySource3;
    private MethodCallExpression _expression;
    private ExpressionTreeNavigator _navigator;
    private BodyHelper _bodyWhereHelper;
    private ParseResultCollector _result;

    [SetUp]
    public void SetUp ()
    {
      _querySource1 = ExpressionHelper.CreateQuerySource();
      _querySource2 = ExpressionHelper.CreateQuerySource();
      _querySource3 = ExpressionHelper.CreateQuerySource();
      _expression = MixedTestQueryGenerator.CreateThreeFromWhereQuery_SelectManyExpression (_querySource1, _querySource2, _querySource3);
      _navigator = new ExpressionTreeNavigator (_expression);
      _result = new ParseResultCollector (_expression);
      new SelectManyExpressionParser ().Parse (_result, _expression);
      _bodyWhereHelper = new BodyHelper (_result.BodyExpressions);
    }

    [Test]
    public void ParsesFromExpressions ()
    {
      Assert.IsNotNull (_bodyWhereHelper.FromExpressions);
      Assert.That (_bodyWhereHelper.FromExpressions, Is.EqualTo (new object[]
          {
              _navigator.Arguments[0].Arguments[0].Arguments[0].Expression,
              _navigator.Arguments[0].Arguments[0].Arguments[1].Operand.Expression,
              _navigator.Arguments[1].Operand.Expression
          }));

      Assert.IsInstanceOfType (typeof (ConstantExpression), _bodyWhereHelper.FromExpressions[0]);
      Assert.IsInstanceOfType (typeof (LambdaExpression), _bodyWhereHelper.FromExpressions[1]);
      Assert.IsInstanceOfType (typeof (LambdaExpression), _bodyWhereHelper.FromExpressions[2]);

      Assert.AreSame (_querySource1, ((ConstantExpression) _bodyWhereHelper.FromExpressions[0]).Value);

      LambdaExpression fromExpression1 = (LambdaExpression) _bodyWhereHelper.FromExpressions[1];
      Assert.AreSame (_querySource2, ExpressionHelper.ExecuteLambda (fromExpression1, (Student) null));

      LambdaExpression fromExpression2 = (LambdaExpression) _bodyWhereHelper.FromExpressions[2];
      Assert.AreSame (_querySource3, ExpressionHelper.ExecuteLambda (fromExpression2, (Student) null));
    }

    [Test]
    public void ParsesFromIdentifiers ()
    {
      Assert.IsNotNull (_bodyWhereHelper.FromIdentifiers);
      Assert.That (_bodyWhereHelper.FromIdentifiers,
          Is.EqualTo (new object[]
              {
                  _navigator.Arguments[0].Arguments[0].Arguments[2].Operand.Parameters[0].Expression,
                  _navigator.Arguments[0].Arguments[0].Arguments[2].Operand.Parameters[1].Expression,
                  _navigator.Arguments[2].Operand.Parameters[1].Expression
              }));

      Assert.IsInstanceOfType (typeof (ParameterExpression), _bodyWhereHelper.FromIdentifiers[0]);
      Assert.IsInstanceOfType (typeof (ParameterExpression), _bodyWhereHelper.FromIdentifiers[1]);
      Assert.IsInstanceOfType (typeof (ParameterExpression), _bodyWhereHelper.FromIdentifiers[2]);

      Assert.AreEqual ("s1", _bodyWhereHelper.FromIdentifiers[0].Name);
      Assert.AreEqual ("s2", _bodyWhereHelper.FromIdentifiers[1].Name);
      Assert.AreEqual ("s3", _bodyWhereHelper.FromIdentifiers[2].Name);
    }

    [Test]
    public void ParsesWhereExpressions()
    {
      Assert.IsNotNull (_bodyWhereHelper.WhereExpressions);
      Assert.That (_bodyWhereHelper.WhereExpressions, Is.EqualTo (new object[]
          {
              _navigator.Arguments[0].Arguments[1].Operand.Expression
          }));
    }

    [Test]
    public void ParsesProjectionExpressions ()
    {
      Assert.IsNotNull (_result.ProjectionExpressions);
      Assert.That (_result.ProjectionExpressions, Is.EqualTo (new object[]
          {
              _navigator.Arguments[0].Arguments[0].Arguments[2].Operand.Expression,
              _navigator.Arguments[2].Operand.Expression
          }));
      Assert.IsInstanceOfType (typeof (LambdaExpression), _result.ProjectionExpressions[0]);
      Assert.IsInstanceOfType (typeof (LambdaExpression), _result.ProjectionExpressions[1]);
    }
  }
}
