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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Backend.DetailParsing.WhereConditionParsing
{
  [TestFixture]
  public class ConstantExpressionParserTest : DetailParserTestBase
  {
    [Test]
    public void Parse ()
    {
      object expected = new Constant (5);
      var parser = new ConstantExpressionParser (StubDatabaseInfo.Instance);
      object result = parser.Parse (Expression.Constant (5, typeof (int)), ParseContext);
      Assert.AreEqual (expected, result);
    }
  }
}
