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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.DetailParsing.SelectProjectionParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.Backend.DetailParsing
{
  [TestFixture]
  public class SelectProjectionParserRegistryTest
  {
    private IDatabaseInfo _databaseInfo;
    private SelectProjectionParserRegistry _selectProjectionParserRegistry;
    private ParseMode _parseMode;
    private ParseContext _parseContext;

    [SetUp]
    public void SetUp ()
    {
      _databaseInfo = StubDatabaseInfo.Instance;
      _parseMode = new ParseMode();
      _selectProjectionParserRegistry = new SelectProjectionParserRegistry (_databaseInfo, _parseMode);
      
      _parseContext = new ParseContext (
          ExpressionHelper.CreateQueryModel(), 
          new List<FieldDescriptor>(), 
          new JoinedTableContext(StubDatabaseInfo.Instance));
    }

    [Test]
    public void Initialization_AddsDefaultParsers ()
    {
      var selectProjectionParserRegistry =
        new SelectProjectionParserRegistry (_databaseInfo,_parseMode);

      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (BinaryExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (ConstantExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (MemberExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (MethodCallExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (NewExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (SubQueryExpression)).ToArray (), Is.Not.Empty);
      Assert.That (selectProjectionParserRegistry.GetParsers (typeof (QuerySourceReferenceExpression)).ToArray (), Is.Not.Empty);
    }

    [Test]
    [ExpectedException (typeof (ParserException), 
        ExpectedMessage = "This version of re-linq does not support subqueries in the select projection of a query.")]
    public void Initialization_AddsErrorParser_ForSubQueryExpression ()
    {
      var selectProjectionParserRegistry = new SelectProjectionParserRegistry (_databaseInfo, _parseMode);

      var subQueryExpression = new SubQueryExpression (ExpressionHelper.CreateQueryModel());
      var parser = selectProjectionParserRegistry.GetParser (subQueryExpression);
      Assert.That (parser, Is.Not.Null);
      parser.Parse (subQueryExpression, _parseContext);
    }

    [Test]
    public void RegisterNewMethodCallExpressionParser_IsInsertedFirst ()
    {
      Assert.That (_selectProjectionParserRegistry.GetParsers (typeof (MethodCallExpression)).Count (), Is.EqualTo (1));

      var methodCallExpressionParser = new MethodCallExpressionParser (_selectProjectionParserRegistry);
      _selectProjectionParserRegistry.RegisterParser (typeof (MethodCallExpression), methodCallExpressionParser);
      Assert.That (_selectProjectionParserRegistry.GetParsers (typeof (MethodCallExpression)).Count (), Is.EqualTo (2));
      Assert.That (_selectProjectionParserRegistry.GetParsers (typeof (MethodCallExpression)).First(), Is.SameAs (methodCallExpressionParser));
    }

    [Test]
    public void GetParser ()
    {
      ConstantExpression constantExpression = Expression.Constant ("test");
      ISelectProjectionParser expectedParser = _selectProjectionParserRegistry.GetParsers (typeof (ConstantExpression)).First();

      Assert.AreSame (expectedParser, _selectProjectionParserRegistry.GetParser (constantExpression));
    }
  }
}
