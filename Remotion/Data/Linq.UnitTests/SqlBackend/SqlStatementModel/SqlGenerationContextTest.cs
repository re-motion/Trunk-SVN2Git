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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using System.Linq;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel
{
  [TestFixture]
  public class SqlGenerationContextTest
  {
    private SqlGenerationContext _context;
    private MainFromClause _source;
    private SqlTableExpression _tableExpression;

    [SetUp]
    public void SetUp ()
    {
      _context = new SqlGenerationContext();
      _source = ClauseObjectMother.CreateMainFromClause ();
      _tableExpression = new SqlTableExpression (typeof (int), new ConstantTableSource (Expression.Constant (1, typeof (int))));
    }

    [Test]
    public void AddQuerySourceMapping ()
    {
      _context.AddQuerySourceMapping (_source, _tableExpression);
      Assert.That (_context.GetQuerySourceMapping().Count, Is.EqualTo (1));
    }

    [Test]
    public void GetQuerySourceMapping ()
    {
      _context.AddQuerySourceMapping (_source, _tableExpression);
      Assert.That (_context.GetQuerySourceMapping().Keys.First(), Is.SameAs (_source));
      Assert.That (_context.GetQuerySourceMapping ().Values.First (), Is.SameAs (_tableExpression));
    }

    [Test]
    public void GetSqlTableExpression ()
    {
      _context.AddQuerySourceMapping (_source, _tableExpression);
      Assert.That (_context.GetSqlTableExpression (_source), Is.SameAs (_tableExpression));
    }

    [Test]
    [ExpectedException (typeof(KeyNotFoundException))]
    public void KeyNotFoundException ()
    {
      _source = ClauseObjectMother.CreateMainFromClause ();
      _context.GetSqlTableExpression (_source);
    }

  }
}