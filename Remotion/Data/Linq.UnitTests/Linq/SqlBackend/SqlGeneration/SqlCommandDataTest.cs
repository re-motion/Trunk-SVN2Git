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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlGeneration
{
  [TestFixture]
  public class SqlCommandDataTest
  {
    private ParameterExpression _rowParameter;

    [SetUp]
    public void SetUp ()
    {
      _rowParameter = Expression.Parameter (typeof (IDatabaseResultRow), "row");
    }

    [Test]
    public void GetInMemoryProjection_NoConversionRequired ()
    {
      var body = Expression.Constant (0);
      var sqlCommandData = new SqlCommandData ("T", new CommandParameter[0], _rowParameter, body);

      var result = sqlCommandData.GetInMemoryProjection<int> ();

      var expectedExpression = Expression.Lambda<Func<IDatabaseResultRow, int>> (body, _rowParameter);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, result);
    }

    [Test]
    public void GetInMemoryProjection_ConversionRequired ()
    {
      var body = Expression.Constant (0);
      var sqlCommandData = new SqlCommandData ("T", new CommandParameter[0], _rowParameter, body);

      var result = sqlCommandData.GetInMemoryProjection<object> ();

      var expectedExpression = Expression.Lambda<Func<IDatabaseResultRow, object>> (Expression.Convert (body, typeof (object)), _rowParameter);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, result);
    }
  }
}