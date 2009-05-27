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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.Linq.Parsing.Structure.Legacy;
using Remotion.Data.UnitTests.Linq.Parsing.Structure.WhereExpressionParserTests;
using Remotion.Data.UnitTests.Linq.TestQueryGenerators;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.LetExpressionParserTests
{
  [TestFixture]
  public class SimpleLetParserTest
  {
    private MethodCallExpression _letExpression;
    private ParseResultCollector _result;
    private BodyHelper _bodyHelper;

    [SetUp]
    public void SetUp ()
    {
      _letExpression = (MethodCallExpression) LetTestQueryGenerator.CreateSimpleSelect_LetExpression (ExpressionHelper.CreateQuerySource ()).Arguments[0];
      _result = new ParseResultCollector (_letExpression);
      new LetExpressionParser ().Parse (_result, _letExpression);
      _bodyHelper = new BodyHelper (_result.BodyExpressions);
    }

    [Test]
    public void ParsesLetExpression ()
    {
      Assert.IsNotNull (_bodyHelper.LetExpressions);
    }

    [Test]
    public void ParsesLetIdentifiers ()
    {
      Assert.IsNotNull (_bodyHelper.LetIdentifiers);
      Assert.IsInstanceOfType (typeof (ParameterExpression), _bodyHelper.LetIdentifiers[0]);
      Assert.AreEqual ("x", _bodyHelper.LetIdentifiers[0].Name);
      Assert.AreEqual (typeof(string), _bodyHelper.LetIdentifiers[0].Type);
    }
  }
}
