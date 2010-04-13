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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.Utilities;
using Remotion.Data.Linq.Clauses;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel
{
  [TestFixture]
  public class SqlStatementTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void WhereCondition_ChecksType ()
    {
      new SqlStatement (Expression.Constant (1), new SqlTable[] { }, new Ordering[] { }, Expression.Constant (1), null, false, false);
    }

    [Test]
    public void WhereCondition_CanBeSetToNull ()
    {
      var sqlStatement = new SqlStatement (Expression.Constant (1), new SqlTable[] { }, new Ordering[] { }, null, null, false, false);
      
      Assert.That (sqlStatement.WhereCondition, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void BuildSelectPart_WithCountAndTop_ThrowsException ()
    {
      new SqlStatement (Expression.Constant (1), new SqlTable[] { }, new Ordering[] { }, null, Expression.Constant ("top"), true, false);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void BuildSelectPart_WithCountAndDistinct_ThrowsException ()
    {
      new SqlStatement (Expression.Constant (1), new SqlTable[] { }, new Ordering[] { }, null, null, true, true);
    }
  }
}