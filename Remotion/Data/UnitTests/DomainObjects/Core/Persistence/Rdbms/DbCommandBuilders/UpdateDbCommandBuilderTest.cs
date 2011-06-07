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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class UpdateDbCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "State of provided DataContainer must not be 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Provider.Connect();
      new UpdateDbCommandBuilder (Provider, StorageNameProvider, order.InternalDataContainer, Provider.SqlDialect, Provider);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Provider.Connect();
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope(
              
              ))
      {
        DbCommandBuilder commandBuilder = new UpdateDbCommandBuilder (
            Provider, StorageNameProvider, order.InternalDataContainer, Provider.SqlDialect, Provider);
        using (IDbCommand command = commandBuilder.Create())
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
      Provider.Connect();
      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, order.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
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
      computer.Employee = Employee.NewObject();
      Provider.Connect();
      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, computer.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (
            command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [EmployeeID] = @EmployeeID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@EmployeeID"]).Value, Is.EqualTo (computer.Employee.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (computer.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithNewObject ()
    {
      Computer computer = Computer.NewObject();
      computer.Employee = Employee.NewObject();
      computer.SerialNumber = "12345";

      Provider.Connect();

      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, computer.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
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
      computer.Employee = Employee.NewObject();
      computer.SerialNumber = "12345";
      computer.Delete();

      Provider.Connect();

      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, computer.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (
            command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [EmployeeID] = @EmployeeID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@EmployeeID"]).Value, Is.EqualTo (DBNull.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (computer.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithNoPropertyAffected ()
    {
      var computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Int32TransactionProperty = 12; // change non-persistent property
      Provider.Connect();
      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, computer.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
      {
        Assert.That (command, Is.Null);
      }
    }

    [Test]
    public void Create_WithNoPropertyAffected_ButMarkedAsChanged ()
    {
      var computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.MarkAsChanged();
      Provider.Connect();

      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, computer.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
      {
        Assert.That (command, Is.Not.Null);
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("UPDATE [Computer] SET [ClassID] = [ClassID] WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (2));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (computer.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (computer.InternalDataContainer.Timestamp));
      }
    }

    [Test]
    public void Create_WithStorageClassTransactionProperty ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.FileName = "new.txt";
      orderTicket.Int32TransactionProperty = 5;
      Provider.Connect();
      var commandBuilder = new UpdateDbCommandBuilder (
          Provider, StorageNameProvider, orderTicket.InternalDataContainer, Provider.SqlDialect, Provider);
      using (IDbCommand command = commandBuilder.Create())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (
            command.CommandText, Is.EqualTo ("UPDATE [OrderTicket] SET [FileName] = @FileName WHERE [ID] = @ID AND [Timestamp] = @Timestamp;"));
        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (orderTicket.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@FileName"]).Value, Is.EqualTo (orderTicket.FileName));
        Assert.That (((IDbDataParameter) command.Parameters["@Timestamp"]).Value, Is.EqualTo (orderTicket.InternalDataContainer.Timestamp));
      }
    }
  }
}