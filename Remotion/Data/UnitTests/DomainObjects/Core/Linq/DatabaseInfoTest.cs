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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq.Backend;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Backend.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DatabaseInfoTest
  {
    private DatabaseInfo _databaseInfo;
    private SqlServerGenerator _sqlGenerator;

    [SetUp]
    public void SetUp ()
    {
      _databaseInfo = DatabaseInfo.Instance;
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
    }

    [Test]
    public void GetTableForFromClause()
    {
      var orderTable = _databaseInfo.GetTableForFromClause (CreateFromClause<Order> ());
      Assert.That (orderTable.Name, Is.EqualTo ("OrderView"));
      Assert.That (orderTable.Alias, Is.EqualTo ("source"));

      var customerTable = _databaseInfo.GetTableForFromClause (CreateFromClause<Customer> ());
      Assert.That (customerTable.Name, Is.EqualTo ("CustomerView"));
      Assert.That (customerTable.Alias, Is.EqualTo ("source"));

      var classWithValidRelationsTable = _databaseInfo.GetTableForFromClause (CreateFromClause<ClassWithValidRelations> ());
      Assert.That (classWithValidRelationsTable.Name, Is.EqualTo ("ClassWithValidRelationsView"));
      Assert.That (classWithValidRelationsTable.Alias, Is.EqualTo ("source"));
    }

    [Test]
    public void GetTableName_ForFromClauseWithOrderCollection ()
    {
      var fromClause = new MainFromClause ("o", typeof (Order), Expression.Constant (new OrderCollection ()));
      var table = _databaseInfo.GetTableForFromClause (fromClause);
      Assert.That (table.Name, Is.EqualTo ("OrderView"));
      Assert.That (table.Alias, Is.EqualTo ("o"));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage =
        "The from clause with identifier 'source' and item type 'System.String' does not identify a queryable table.")]
    public void GetTableName_InvalidType ()
    {
      var stringSource = new DummyQueryable<string>();
      var stringClause = new MainFromClause ("source", typeof (string), Expression.Constant (stringSource));

      _databaseInfo.GetTableForFromClause(stringClause);
    }

    [Test]
    public void IsTable_True ()
    {
      Type type = typeof (Order);
      Assert.IsTrue (_databaseInfo.IsTableType (type));
    }

    [Test]
    public void IsTable_False ()
    {
      Type type = typeof (int);
      Assert.IsFalse (_databaseInfo.IsTableType (type));
    }

    [Test]
    public void GetColumnName()
    {
      Assert.AreEqual ("OrderNo", _databaseInfo.GetColumnName (typeof (Order).GetProperty ("OrderNumber")));
      Assert.AreEqual ("DeliveryDate", _databaseInfo.GetColumnName (typeof (Order).GetProperty ("DeliveryDate")));
      Assert.AreEqual ("OrderID", _databaseInfo.GetColumnName (typeof (OrderTicket).GetProperty ("Order")));
    }

    [Test]
    public void GetColumnName_ForID ()
    {
      Assert.AreEqual ("ID", _databaseInfo.GetColumnName (typeof (Order).GetProperty ("ID")));
    }

    [Test]
    public void GetColumnName_Null ()
    {
      Assert.IsNull (_databaseInfo.GetColumnName (typeof (Order).GetProperty ("OrderTicket")));
      Assert.IsNull (_databaseInfo.GetColumnName (typeof (Order).GetProperty ("NotInMapping")));
      Assert.IsNull (_databaseInfo.GetColumnName (typeof (string).GetProperty ("Length")));
      Assert.IsNull (_databaseInfo.GetColumnName (typeof (DatabaseInfoTest).GetMethod ("SetUp")));
      Assert.IsNull (_databaseInfo.GetColumnName (typeof (DatabaseInfoTest).GetField ("_databaseInfo", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetJoinColumns_FK_Right()
    {
      var columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("OrderItems"));
      Assert.AreEqual ("ID", columns.Value.PrimaryKey);
      Assert.AreEqual ("OrderID", columns.Value.ForeignKey);
    }

    [Test]
    public void GetJoinColumns_FK_Left ()
    {
      var columns = _databaseInfo.GetJoinColumnNames (typeof (OrderItem).GetProperty ("Order"));
      Assert.AreEqual ("OrderID", columns.Value.PrimaryKey);
      Assert.AreEqual ("ID", columns.Value.ForeignKey);
    }

    [Test]
    public void GetJoinColumns_NotInMapping ()
    {
      var columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("NotInMapping"));
      Assert.IsNull (columns);
    }

    [Test]
    public void GetJoinColumns_NoRelationProperty ()
    {
      var columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("OrderNumber"));
      Assert.IsNull (columns);
    }

    [Test]
    public void GetRelatedTable_FK_Right ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (Order).GetProperty ("OrderItems"));
      Assert.AreEqual ("OrderItemView", tableName);
    }

    [Test]
    public void GetRelatedTable_FK_Left ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (OrderItem).GetProperty ("Order"));
      Assert.AreEqual ("OrderView", tableName);
    }

    [Test]
    public void GetRelatedTable_NotInMapping ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (Order).GetProperty ("NotInMapping"));
      Assert.IsNull (tableName);
    }

    [Test]
    public void GetRelatedTable_NoRelationProperty ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (Order).GetProperty ("OrderNumber"));
      Assert.IsNull (tableName);
    }

    [Test]
    public void ProcessWhereParameter_Entity ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Order order = Order.NewObject();
        object processed = _databaseInfo.ProcessWhereParameter (order);
        Assert.AreEqual (order.ID, processed);
      }
    }

    [Test]
    public void ProcessWhereParameter_NoEntity ()
    {
      object processed = _databaseInfo.ProcessWhereParameter (5);
      Assert.AreEqual (5, processed);
    }

    [Test]
    public void GetPrimaryKeyMember_Entity()
    {
      MemberInfo actual = _databaseInfo.GetPrimaryKeyMember (typeof (Computer));
      MemberInfo expected = typeof (DomainObject).GetProperty ("ID");
      Assert.AreEqual (expected, actual);
    }

    [Test]
    public void GetPrimaryKeyMember_NoEntity ()
    {
      MemberInfo actual = _databaseInfo.GetPrimaryKeyMember (typeof (string));
      Assert.IsNull (actual);
    }

    private FromClauseBase CreateFromClause<T> ()
    where T : DomainObject
    {
      IQueryable querySource = new DomainObjectQueryable<T> (_sqlGenerator);
      return new MainFromClause ("source", querySource.ElementType, Expression.Constant (querySource));
    }
  }
}
