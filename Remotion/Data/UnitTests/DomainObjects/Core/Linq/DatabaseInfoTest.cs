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
      var orderTable = _databaseInfo.GetTableForFromClause (CreateFromClause<Order> (), "x");
      Assert.That (orderTable.Name, Is.EqualTo ("OrderView"));
      Assert.That (orderTable.Alias, Is.EqualTo ("x"));

      var customerTable = _databaseInfo.GetTableForFromClause (CreateFromClause<Customer> (), "y");
      Assert.That (customerTable.Name, Is.EqualTo ("CustomerView"));
      Assert.That (customerTable.Alias, Is.EqualTo ("y"));

      var classWithValidRelationsTable = _databaseInfo.GetTableForFromClause (CreateFromClause<ClassWithValidRelations> (), null);
      Assert.That (classWithValidRelationsTable.Name, Is.EqualTo ("ClassWithValidRelationsView"));
      Assert.That (classWithValidRelationsTable.Alias, Is.Null);
    }

    [Test]
    public void GetTableName_ForFromClauseWithOrderCollection ()
    {
      var fromClause = new MainFromClause ("o", typeof (Order), Expression.Constant (new OrderCollection ()));
      var table = _databaseInfo.GetTableForFromClause (fromClause, "order");
      Assert.That (table.Name, Is.EqualTo ("OrderView"));
      Assert.That (table.Alias, Is.EqualTo ("order"));
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage =
        "The from clause with identifier 'source' and item type 'System.String' does not identify a queryable table.")]
    public void GetTableName_InvalidType ()
    {
      var stringSource = new DummyQueryable<string>();
      var stringClause = new MainFromClause ("source", typeof (string), Expression.Constant (stringSource));

      _databaseInfo.GetTableForFromClause(stringClause, "whatever");
    }

    [Test]
    public void IsRelationMember_FK_Right ()
    {
      Assert.That (_databaseInfo.IsRelationMember (typeof (Order).GetProperty ("OrderItems")), Is.True);
    }

    [Test]
    public void IsRelationMember_FK_Left ()
    {
      Assert.That (_databaseInfo.IsRelationMember (typeof (OrderItem).GetProperty ("Order")), Is.True);
    }

    [Test]
    public void IsRelationMember_NotInMapping ()
    {
      Assert.That (_databaseInfo.IsRelationMember (typeof (Order).GetProperty ("NotInMapping")), Is.False);
    }

    [Test]
    public void IsRelationMember_NoRelationProperty ()
    {
      Assert.That (_databaseInfo.IsRelationMember (typeof (Order).GetProperty ("OrderNumber")), Is.False);
    }

    [Test]
    public void GetTableForRelation_FK_Right ()
    {
      var table = _databaseInfo.GetTableForRelation (typeof (Order).GetProperty ("OrderItems"), "x");
      Assert.AreEqual ("OrderItemView", table.Name);
      Assert.AreEqual ("x", table.Alias);
    }

    [Test]
    public void GetTableForRelation_FK_Left ()
    {
      var table = _databaseInfo.GetTableForRelation (typeof (OrderItem).GetProperty ("Order"), "y");
      Assert.AreEqual ("OrderView", table.Name);
      Assert.AreEqual ("y", table.Alias);
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = 
        "The member 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.NotInMapping' does not identify a mapped relation.")]
    public void GetTableForRelation_NotInMapping ()
    {
      _databaseInfo.GetTableForRelation (typeof (Order).GetProperty ("NotInMapping"), "z");
    }

    [Test]
    [ExpectedException (typeof (UnmappedItemException), ExpectedMessage = 
        "The member 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber' does not identify a mapped relation.")]
    public void GetTableForRelation_NoRelationProperty ()
    {
      _databaseInfo.GetTableForRelation (typeof (Order).GetProperty ("OrderNumber"), "z");
    }

    [Test]
    public void HasAssociatedColumn_True ()
    {
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (Order).GetProperty ("OrderNumber")), Is.True);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (Order).GetProperty ("DeliveryDate")), Is.True);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (OrderTicket).GetProperty ("Order")), Is.True);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (Order).GetProperty ("ID")), Is.True);
    }

    [Test]
    public void HasAssociatedColumn_False ()
    {
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (Order).GetProperty ("OrderTicket")), Is.False);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (Order).GetProperty ("NotInMapping")), Is.False);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (string).GetProperty ("Length")), Is.False);
      Assert.That (_databaseInfo.HasAssociatedColumn (typeof (DatabaseInfoTest).GetMethod ("SetUp")), Is.False);
      Assert.That (
          _databaseInfo.HasAssociatedColumn (typeof (DatabaseInfoTest).GetField ("_databaseInfo", BindingFlags.NonPublic | BindingFlags.Instance)), 
          Is.False);
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
