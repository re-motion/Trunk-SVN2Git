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
using NUnit.Framework;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing.SelectProjectionParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Backend.DetailParsing.SelectProjectionParsing
{
  [TestFixture]
  public class QuerySourceReferenceExpressionParserTest : DetailParserTestBase
  {
    [Test]
    public void Parse ()
    {
      var expression = new QuerySourceReferenceExpression (CookClause);
      var resolver = new FieldResolver (StubDatabaseInfo.Instance, new SelectFieldAccessPolicy());

      var fromSource = ParseContext.JoinedTableContext.GetColumnSource (CookClause);
      var path = new FieldSourcePath (fromSource, new SingleJoin[0]);
      var expectedFieldDescriptor = new FieldDescriptor (null, path, new Column (fromSource, "*"));
      IEvaluation expectedEvaluation = expectedFieldDescriptor.Column;

      var parser = new QuerySourceReferenceExpressionParser (resolver);

      IEvaluation actualEvaluation = parser.Parse (expression, ParseContext);
      Assert.AreEqual (expectedEvaluation, actualEvaluation);
    }
  }
}
