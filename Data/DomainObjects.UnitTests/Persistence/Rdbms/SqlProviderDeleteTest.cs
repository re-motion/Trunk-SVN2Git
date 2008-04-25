using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderDeleteTest : ClientTransactionBaseTest
  {
    private SqlProvider _provider;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      RdbmsProviderDefinition definition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (SqlProvider), TestDomainConnectionString);

      _provider = new SqlProvider (definition);
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void DeleteSingleDataContainer ()
    {
      DataContainerCollection containers = CreateDataContainerCollection (GetDeletedOrderTicketContainer());
      _provider.Connect ();
      _provider.Save (containers);

      Assert.IsNull (_provider.LoadDataContainer (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void SetTimestampOfDeletedDataContainer ()
    {
      DataContainerCollection containers = CreateDataContainerCollection (GetDeletedOrderTicketContainer ());
      _provider.Connect ();
      _provider.Save (containers);
      _provider.SetTimestamp (containers);

      // expectation: no exception
    }

    [Test]
    public void DeleteRelatedDataContainers ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee2);
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);

      supervisor.Delete ();
      subordinate.Delete ();
      computer.Delete ();

			DataContainerCollection containers = CreateDataContainerCollection (supervisor.InternalDataContainer, subordinate.InternalDataContainer,
					computer.InternalDataContainer);

      _provider.Connect ();
      _provider.Save (containers);
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.NewRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.NewRootTransaction();

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

      _provider.Connect ();
      _provider.Save (CreateDataContainerCollection (changedDataContainer));
      _provider.Save (CreateDataContainerCollection (deletedDataContainer));
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithoutForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.NewRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.NewRootTransaction();

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
        deletedObject.Delete ();
      }
      
      _provider.Connect ();
      _provider.Save (CreateDataContainerCollection (changedDataContainer));
      _provider.Save (CreateDataContainerCollection (deletedDataContainer));
    }

    private DataContainerCollection CreateDataContainerCollection (params DataContainer[] dataContainers)
    {
      DataContainerCollection collection = new DataContainerCollection ();
      foreach (DataContainer dataContainer in dataContainers)
        collection.Add (dataContainer);

      return collection;
    }

    private DataContainer GetDeletedOrderTicketContainer ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.Delete ();
			return orderTicket.InternalDataContainer;
    }
  }
}
