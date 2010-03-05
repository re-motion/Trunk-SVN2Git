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
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.MappingResolution
{
  [TestFixture]
  public class ResolvingSqlStatementVisitorTest
  {
    private SqlStatement _sqlStatement;
    private SqlStatementResolverStub _resolver;
    private ResolvingSqlStatementVisitor _sqlStatementVisitor;

    [SetUp]
    public void SetUp ()
    {
      var source = SqlStatementModelObjectMother.CreateConstantTableSource_TypeIsCook();
      var sqlTable = SqlStatementModelObjectMother.CreateSqlTable (source);
      var tableReferenceExpression = new SqlTableReferenceExpression (sqlTable);
      _sqlStatement = new SqlStatement (tableReferenceExpression, sqlTable, new UniqueIdentifierGenerator());
      _resolver = new SqlStatementResolverStub();
      _sqlStatementVisitor = new ResolvingSqlStatementVisitor (_resolver);
    }

    [Test]
    public void VisitFromExpression_ReplacesTableSource ()
    {
      _sqlStatementVisitor.VisitSqlStatement (_sqlStatement);

      Assert.That (_sqlStatement.FromExpression.TableSource, Is.InstanceOfType (typeof (SqlTableSource)));
    }

    [Test]
    public void VisitSelectProjection_CreatesSqlColumnListExpression ()
    {
      _sqlStatementVisitor.VisitSqlStatement (_sqlStatement);
      var constraint = new SqlTableSource (typeof (Cook), "Cook", "c");

      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns.Count, Is.EqualTo (3));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[0].OwningTableAlias, Is.EqualTo (constraint.TableAlias));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[0].OwningTableAlias, Is.EqualTo (constraint.TableAlias));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[0].OwningTableAlias, Is.EqualTo (constraint.TableAlias));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[0].ColumnName, Is.EqualTo ("ID"));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[1].ColumnName, Is.EqualTo ("Name"));
      Assert.That (((SqlColumnListExpression) _sqlStatement.SelectProjection).Columns[2].ColumnName, Is.EqualTo ("City"));
    }

    [Test]
    public void VisitTopExpression_CreatesConstantExpression ()
    {
      _sqlStatement.TopExpression = Expression.Constant (1);
      _sqlStatementVisitor.VisitSqlStatement (_sqlStatement);

      Assert.That (((ConstantExpression) _sqlStatement.TopExpression).Value, Is.EqualTo (1));
    }
  }
}