using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class CommitEventsTest : ClientTransactionBaseTest
  {
    private Customer _customer;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _customer = Customer.GetObject (DomainObjectIDs.Customer1);
    }

    [Test]
    public void CommitEvents ()
    {
      _customer.Name = "New name";

      DomainObjectEventReceiver domainObjectEventReceiver = new DomainObjectEventReceiver (_customer);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (domainObjectEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (domainObjectEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (1, committedDomainObjects.Count);

      Assert.AreSame (_customer, committingDomainObjects[0]);
      Assert.AreSame (_customer, committedDomainObjects[0]);
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectCommitting ()
    {
      _customer.Name = "New name";
      _customer.Committing += new EventHandler (Customer_CommittingForModifyOtherObjectInDomainObjectCommitting);

      Ceo ceo = _customer.Ceo;

      DomainObjectEventReceiver ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (2, committingDomainObjects.Count);
      Assert.AreEqual (2, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjects.ContainsObject (_customer));
      Assert.IsTrue (committedDomainObjects.ContainsObject (_customer));

      Assert.IsTrue (committingDomainObjects.ContainsObject (ceo));
      Assert.IsTrue (committedDomainObjects.ContainsObject (ceo));
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionCommitting ()
    {
      _customer.Name = "New name";
      ClientTransactionMock.Committing += new ClientTransactionEventHandler (ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting);

      Ceo ceo = _customer.Ceo;

      DomainObjectEventReceiver ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (2, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjectsForFirstCommitEvent = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committingDomainObjectsForSecondCommit = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[1];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (1, committingDomainObjectsForFirstCommitEvent.Count);
      Assert.AreEqual (1, committingDomainObjectsForSecondCommit.Count);
      Assert.AreEqual (2, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjectsForFirstCommitEvent.ContainsObject (_customer));
      Assert.IsFalse (committingDomainObjectsForFirstCommitEvent.ContainsObject (ceo));

      Assert.IsFalse (committingDomainObjectsForSecondCommit.ContainsObject (_customer));
      Assert.IsTrue (committingDomainObjectsForSecondCommit.ContainsObject (ceo));

      Assert.IsTrue (committedDomainObjects.ContainsObject (_customer));
      Assert.IsTrue (committedDomainObjects.ContainsObject (ceo));
    }

    [Test]
    public void ModifyOtherObjects ()
    {
      _customer.Name = "New name";

      Ceo ceo = _customer.Ceo;
      ceo.Name = "New CEO name";

      Order order = _customer.Orders[DomainObjectIDs.Order1];
      IndustrialSector industrialSector = _customer.IndustrialSector;

      DomainObjectEventReceiver ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      DomainObjectEventReceiver customerEventReceiver = new DomainObjectEventReceiver (_customer);
      DomainObjectEventReceiver orderEventReceiver = new DomainObjectEventReceiver (order);
      DomainObjectEventReceiver industrialSectorEventReceiver = new DomainObjectEventReceiver (industrialSector);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      _customer.Committing += new EventHandler (Customer_CommittingForModifyOtherObjects);
      ClientTransactionMock.Committing += new ClientTransactionEventHandler (ClientTransactionMock_CommittingForModifyOtherObjects);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (orderEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (orderEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (industrialSectorEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (industrialSectorEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (2, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjectsForFirstCommitEvent = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committingDomainObjectsForSecondCommit = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[1];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (3, committingDomainObjectsForFirstCommitEvent.Count);
      Assert.AreEqual (1, committingDomainObjectsForSecondCommit.Count);
      Assert.AreEqual (4, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjectsForFirstCommitEvent.ContainsObject (_customer));
      Assert.IsTrue (committingDomainObjectsForFirstCommitEvent.ContainsObject (ceo));
      Assert.IsTrue (committingDomainObjectsForFirstCommitEvent.ContainsObject (order));

      Assert.IsTrue (committingDomainObjectsForSecondCommit.ContainsObject (industrialSector));

      Assert.IsTrue (committedDomainObjects.ContainsObject (_customer));
      Assert.IsTrue (committedDomainObjects.ContainsObject (ceo));
      Assert.IsTrue (committedDomainObjects.ContainsObject (order));
      Assert.IsTrue (committedDomainObjects.ContainsObject (industrialSector));
    }

    [Test]
    public void CommitWithoutChanges ()
    {
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      ClientTransactionMock.Commit ();

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (0, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);
    }

    [Test]
    public void CommitWithExistingObjectDeleted ()
    {
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ObjectID classWithAllDataTypesID = classWithAllDataTypes.ID;

      classWithAllDataTypes.Delete ();

      ClientTransactionMock.Commit ();

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjects.Contains (classWithAllDataTypesID));
    }

    [Test]
    public void CommittedEventForObjectChangedBackToOriginal ()
    {
      _customer.Name = "New name";

      DomainObjectEventReceiver customerEventReceiver = new DomainObjectEventReceiver (_customer);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);
      _customer.Committing += new EventHandler (Customer_CommittingForCommittedEventForObjectChangedBackToOriginal);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsFalse (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (0, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);
    }

    [Test]
    public void CommittedEventForMarkAsChanged ()
    {
      _customer.MarkAsChanged ();

      DomainObjectEventReceiver customerEventReceiver = new DomainObjectEventReceiver (_customer);
      ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);
      _customer.Committing += new EventHandler (Customer_CommittingForCommittedEventForObjectChangedBackToOriginal);

      ClientTransactionMock.Commit ();

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

      DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
      DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (1, committedDomainObjects.Count);

      Assert.Contains (_customer, committedDomainObjects);
      Assert.Contains (_customer, committingDomainObjects);
    }

    private void Customer_CommittingForModifyOtherObjectInDomainObjectCommitting (object sender, EventArgs e)
    {
      Customer customer = (Customer) sender;
      customer.Ceo.Name = "New CEO name";
    }

    private void ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting (object sender, ClientTransactionEventArgs args)
    {
      Customer customer = args.DomainObjects[0] as Customer;
      if (customer != null)
        customer.Ceo.Name = "New CEO name";
    }

    private void Customer_CommittingForModifyOtherObjects (object sender, EventArgs e)
    {
      Customer customer = (Customer) sender;
      Order order = customer.Orders[DomainObjectIDs.Order1];
      order.OrderNumber = 1000;
    }

    private void ClientTransactionMock_CommittingForModifyOtherObjects (object sender, ClientTransactionEventArgs args)
    {
      Customer customer = (Customer) args.DomainObjects[DomainObjectIDs.Customer1];
      if (customer != null)
        customer.IndustrialSector.Name = "New industrial sector name";
    }

    private void Customer_CommittingForCommittedEventForObjectChangedBackToOriginal (object sender, EventArgs e)
    {
      _customer.Name = _customer.Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Name"].GetOriginalValue<string>();
    }
  }
}
