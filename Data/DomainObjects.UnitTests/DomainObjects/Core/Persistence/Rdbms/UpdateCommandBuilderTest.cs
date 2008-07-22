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
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class UpdateCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      new UpdateCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "State of provided DataContainer must not be 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Provider.Connect ();
      new UpdateCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (WhereClauseBuilder)).Clear ().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope ())
      {
        CommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, order.InternalDataContainer);
        using (IDbCommand command = commandBuilder.Create ())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }

    [Test]
    public void Create_WithChangedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Provider.Connect ();
      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, order.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [Order] SET [OrderNo] = @OrderNo WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (order.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@OrderNo"]).Value, Is.EqualTo (order.OrderNumber));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (order.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithForeignKeyChange ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Employee = Employee.NewObject ();
      Provider.Connect ();
      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, computer.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [EmployeeID] = @EmployeeID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@EmployeeID"]).Value, Is.EqualTo (computer.Employee.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (computer.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithNewObject ()
    {
      Computer computer = Computer.NewObject ();
      computer.Employee = Employee.NewObject ();
      computer.SerialNumber = "12345";
      
      Provider.Connect ();
      
      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, computer.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [EmployeeID] = @EmployeeID WHERE [ID] = @ID;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (2));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@EmployeeID"]).Value, Is.EqualTo (computer.Employee.ID.Value));
      }
    }

    [Test]
    public void Create_WithDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Employee = Employee.NewObject ();
      computer.SerialNumber = "12345";
      computer.Delete ();

      Provider.Connect ();

      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, computer.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [EmployeeID] = @EmployeeID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@EmployeeID"]).Value, Is.EqualTo (DBNull.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (computer.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithNoPropertyAffected ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.InternalDataContainer.MarkAsChanged ();
      Provider.Connect ();
      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, order.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command, Is.Null);
      }
    }

    [Test]
    public void Create_WithStorageClassTransactionProperty ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.FileName = "new.txt";
      orderTicket.Int32TransactionProperty = 5;
      Provider.Connect ();
      UpdateCommandBuilder commandBuilder = new UpdateCommandBuilder (Provider, orderTicket.InternalDataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [OrderTicket] SET [FileName] = @FileName WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (orderTicket.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@FileName"]).Value, Is.EqualTo (orderTicket.FileName));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (orderTicket.InternalDataContainer.Timestamp));
      }
    }

  }
}
