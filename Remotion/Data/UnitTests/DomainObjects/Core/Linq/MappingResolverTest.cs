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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class MappingResolverTest
  {
    private MappingResolver _resolver;
    private UniqueIdentifierGenerator _generator;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new MappingResolver();
      _generator = new UniqueIdentifierGenerator();
    }

    [Test]
    public void ResolveTableReferenceExpression ()
    {
      var sqlTable = new SqlTable (new ResolvedSimpleTableInfo(typeof(Order), "Order", "o"));
      var tableReferenceExpression = new SqlTableReferenceExpression(sqlTable);

      var primaryKeyColumn = new SqlColumnExpression (typeof (ObjectID), "o", "ID");
      var column1 = new SqlColumnExpression (typeof (int), "o", "OrderNo");
      var column2 = new SqlColumnExpression (typeof (DateTime), "o", "DeliveryDate");
      var column3 = new SqlColumnExpression (typeof (ObjectID), "o", "OfficialID");
      var column4 = new SqlColumnExpression (typeof (ObjectID), "o", "CustomerID");
      
      SqlEntityExpression sqlEntityExpression = (SqlEntityExpression) _resolver.ResolveTableReferenceExpression (tableReferenceExpression, _generator);

      Assert.That (sqlEntityExpression, Is.Not.Null);
      Assert.That (sqlEntityExpression.ProjectionColumns.Count, Is.EqualTo (4));
      Assert.That (sqlEntityExpression.PrimaryKeyColumn.ColumnName, Is.EqualTo (primaryKeyColumn.ColumnName));
      Assert.That (sqlEntityExpression.PrimaryKeyColumn.OwningTableAlias, Is.EqualTo (primaryKeyColumn.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[0].ColumnName, Is.EqualTo (column1.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[0].OwningTableAlias, Is.EqualTo (column1.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[1].ColumnName, Is.EqualTo (column2.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[1].OwningTableAlias, Is.EqualTo (column2.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[2].ColumnName, Is.EqualTo (column3.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[2].OwningTableAlias, Is.EqualTo (column3.OwningTableAlias));
      Assert.That (sqlEntityExpression.ProjectionColumns[3].ColumnName, Is.EqualTo (column4.ColumnName));
      Assert.That (sqlEntityExpression.ProjectionColumns[3].OwningTableAlias, Is.EqualTo (column4.OwningTableAlias));      
    }
  }
}