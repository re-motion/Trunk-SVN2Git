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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class InsertCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      Order order = Order.NewObject ();
			new InsertCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "State of provided DataContainer must be 'New', but is 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Provider.Connect ();
			new InsertCommandBuilder (Provider, order.InternalDataContainer);
    }

    [Test]
    public void Create ()
    {
      Provider.Connect ();

      Order order = Order.NewObject ();
      order.OrderNumber = 212;
      order.DeliveryDate = new DateTime(2008, 7, 1);

      InsertCommandBuilder builder = new InsertCommandBuilder (Provider, order.InternalDataContainer);
      using (IDbCommand command = builder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText,
            Is.EqualTo ("INSERT INTO [Order] ([ID], [ClassID], [OrderNo], [DeliveryDate]) VALUES (@ID, @ClassID, @OrderNo, @DeliveryDate);"));

        Assert.That (command.Parameters.Count, Is.EqualTo (4));
        Assert.That (((IDbDataParameter) command.Parameters["@ID"]).Value, Is.EqualTo (order.ID.Value));
        Assert.That (((IDbDataParameter) command.Parameters["@ClassID"]).Value, Is.EqualTo (order.ID.ClassID));
        Assert.That (((IDbDataParameter) command.Parameters["@OrderNo"]).Value, Is.EqualTo (order.OrderNumber));
        Assert.That (((IDbDataParameter) command.Parameters["@DeliveryDate"]).Value, Is.EqualTo (order.DeliveryDate));
      }
    }

    [Test]
    public void Create_WithForeignKey_HasNoParameterForIDColumn ()
    {
      Provider.Connect();

      Computer computer = Computer.NewObject();
      computer.Employee = Employee.NewObject();
      computer.SerialNumber = "123";

      InsertCommandBuilder builder = new InsertCommandBuilder (Provider, computer.InternalDataContainer);
      using (IDbCommand command = builder.Create())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText,
            Is.EqualTo ("INSERT INTO [Computer] ([ID], [ClassID], [SerialNumber]) VALUES (@ID, @ClassID, @SerialNumber);"));

        Assert.That (command.Parameters.Count, Is.EqualTo (3));
      }
    }

    [Test]
    public void Create_WithStorageClassTransactionPrropety ()
    {
      Provider.Connect ();

      OrderTicket orderTicket = OrderTicket.NewObject ();
      orderTicket.FileName = "fx.txt";
      orderTicket.Order = Order.NewObject ();
      orderTicket.Int32TransactionProperty = 7;

      InsertCommandBuilder builder = new InsertCommandBuilder (Provider, orderTicket.InternalDataContainer);
      using (IDbCommand command = builder.Create ())
      {
        Assert.That (command.CommandType, Is.EqualTo (CommandType.Text));
        Assert.That (command.CommandText, Is.EqualTo ("INSERT INTO [OrderTicket] ([ID], [ClassID], [FileName]) VALUES (@ID, @ClassID, @FileName);"));

        Assert.That (command.Parameters.Count, Is.EqualTo (3));
        Assert.That (command.Parameters.Contains ("@Int32TransactionProperty"), Is.False);
      }
    }
  }
}
