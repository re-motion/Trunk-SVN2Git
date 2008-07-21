/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.UnitTests.Core.Linq;
using Remotion.Data.Linq;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.Linq.Clauses;
using System.Linq.Expressions;
using Remotion.Data.Linq.SqlGeneration.SqlServer;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class DatabaseInfoTest
  {
    private IDatabaseInfo _databaseInfo;
    private SqlServerGenerator _sqlGenerator;

    [SetUp]
    public void SetUp ()
    {
      _databaseInfo = DatabaseInfo.Instance;
      _sqlGenerator = new SqlServerGenerator (DatabaseInfo.Instance);
    }

    [Test]
    public void GetTableName()
    {
      Assert.AreEqual ("Order", _databaseInfo.GetTableName (CreateFromClause<Order>()));
      Assert.AreEqual ("Company", _databaseInfo.GetTableName (CreateFromClause<Customer> ()));
      Assert.AreEqual ("TableWithValidRelations", _databaseInfo.GetTableName (CreateFromClause<ClassWithValidRelations> ()));
    }

    [Test]
    public void GetTableName_InvalidType ()
    {
      Assert.IsNull (_databaseInfo.GetTableName (CreateFromClause<DomainObject> ()));

      DummyQueryable<string> stringSource = new DummyQueryable<string>();
      MainFromClause stringClause = new MainFromClause (Expression.Parameter (typeof (string), "source"), Expression.Constant (stringSource));

      Assert.IsNull (_databaseInfo.GetTableName (stringClause));
    }

    private FromClauseBase CreateFromClause<T> ()
        where T : DomainObject
    {
      IQueryable querySource = new DomainObjectQueryable<T> (_sqlGenerator);
      return new MainFromClause (Expression.Parameter (querySource.ElementType, "source"), Expression.Constant (querySource));
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
      Tuple<string, string> columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("OrderItems"));
      Assert.AreEqual ("ID", columns.A);
      Assert.AreEqual ("OrderID", columns.B);
    }

    [Test]
    public void GetJoinColumns_FK_Left ()
    {
      Tuple<string, string> columns = _databaseInfo.GetJoinColumnNames (typeof (OrderItem).GetProperty ("Order"));
      Assert.AreEqual ("OrderID", columns.A);
      Assert.AreEqual ("ID", columns.B);
    }

    [Test]
    public void GetJoinColumns_NotInMapping ()
    {
      Tuple<string, string> columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("NotInMapping"));
      Assert.IsNull (columns);
    }

    [Test]
    public void GetJoinColumns_NoRelationProperty ()
    {
      Tuple<string, string> columns = _databaseInfo.GetJoinColumnNames (typeof (Order).GetProperty ("OrderNumber"));
      Assert.IsNull (columns);
    }

    [Test]
    public void GetRelatedTable_FK_Right ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (Order).GetProperty ("OrderItems"));
      Assert.AreEqual ("OrderItem", tableName);
    }

    [Test]
    public void GetRelatedTable_FK_Left ()
    {
      string tableName = _databaseInfo.GetRelatedTableName (typeof (OrderItem).GetProperty ("Order"));
      Assert.AreEqual ("Order", tableName);
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
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
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
  }
}
