// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderDeleteTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void DeleteSingleDataContainer ()
    {
      IEnumerable<DataContainer> containers = new[] { GetDeletedOrderTicketContainer() };
      Provider.Connect();
      Provider.Save (containers);

      Assert.That (Provider.LoadDataContainer (DomainObjectIDs.OrderTicket1).LocatedObject, Is.Null);
    }

    [Test]
    public void DeleteRelatedDataContainers ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee2);
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);

      supervisor.Delete();
      subordinate.Delete();
      computer.Delete();

      IEnumerable<DataContainer> containers = new[] 
                                              {
                                                  supervisor.InternalDataContainer,
                                                  subordinate.InternalDataContainer,
                                                  computer.InternalDataContainer
                                              };

      Provider.Connect();
      Provider.Save (containers);
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      OrderTicket changedOrderTicket;
      DataContainer changedDataContainer;
      using (clientTransaction1.EnterDiscardingScope())
      {
        changedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        changedOrderTicket.FileName = @"C:\NewFile.jpg";
        changedDataContainer = changedOrderTicket.InternalDataContainer;
      }

      OrderTicket deletedOrderTicket;
      DataContainer deletedDataContainer;
      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        deletedOrderTicket.Delete();
        deletedDataContainer = deletedOrderTicket.InternalDataContainer;
      }

      Provider.Connect();
      Provider.Save (new[] { changedDataContainer });
      Provider.Save (new[] { deletedDataContainer });
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithoutForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      DataContainer changedDataContainer;
      ClassWithAllDataTypes changedObject;

      using (clientTransaction1.EnterDiscardingScope())
      {
        changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        changedDataContainer = changedObject.InternalDataContainer;
        changedObject.StringProperty = "New text";
      }

      DataContainer deletedDataContainer;
      ClassWithAllDataTypes deletedObject;

      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        deletedDataContainer = deletedObject.InternalDataContainer;
        deletedObject.Delete();
      }

      Provider.Connect();
      Provider.Save (new[] { changedDataContainer });
      Provider.Save (new[] { deletedDataContainer });
    }

    private DataContainer GetDeletedOrderTicketContainer ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.Delete();
      return orderTicket.InternalDataContainer;
    }
  }
}