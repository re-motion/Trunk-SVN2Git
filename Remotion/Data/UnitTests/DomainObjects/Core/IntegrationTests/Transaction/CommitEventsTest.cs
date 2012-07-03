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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class CommitEventsTest : ClientTransactionBaseTest
  {
    private Customer _customer;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _customer = Customer.GetObject (DomainObjectIDs.Customer1);
    }

    [Test]
    public void CommitEvents ()
    {
      _customer.Name = "New name";

      var domainObjectEventReceiver = new DomainObjectEventReceiver (_customer);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.IsTrue (domainObjectEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (domainObjectEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (1, committedDomainObjects.Count);

      Assert.AreSame (_customer, committingDomainObjects[0]);
      Assert.AreSame (_customer, committedDomainObjects[0]);
    }

    [Test]
    public void ModifyOtherObjectInDomainObjectCommitting ()
    {
      var ceo = _customer.Ceo;

      _customer.Name = "New name";
      _customer.Committing += (sender, e) => ceo.Name = "New CEO name";

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (2, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects1 = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjects2 = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjects1, Is.EqualTo (new[] { _customer }));
      Assert.That (committingDomainObjects2, Is.EqualTo (new[] { ceo }));
      Assert.That (committedDomainObjects, Is.EquivalentTo (new DomainObject[] { _customer, ceo }));
    }

    [Test]
    public void ModifyOtherObjectInClientTransactionCommitting ()
    {
      _customer.Name = "New name";
      TestableClientTransaction.Committing += ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting;

      Ceo ceo = _customer.Ceo;

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (2, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjectsForFirstCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjectsForSecondCommit = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (1, committingDomainObjectsForFirstCommitEvent.Count);
      Assert.AreEqual (1, committingDomainObjectsForSecondCommit.Count);
      Assert.AreEqual (2, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjectsForFirstCommitEvent.Contains (_customer));
      Assert.IsFalse (committingDomainObjectsForFirstCommitEvent.Contains (ceo));

      Assert.IsFalse (committingDomainObjectsForSecondCommit.Contains (_customer));
      Assert.IsTrue (committingDomainObjectsForSecondCommit.Contains (ceo));

      Assert.IsTrue (committedDomainObjects.Contains (_customer));
      Assert.IsTrue (committedDomainObjects.Contains (ceo));
    }

    [Test]
    public void ModifyOtherObjects ()
    {
      _customer.Name = "New name";

      Ceo ceo = _customer.Ceo;
      ceo.Name = "New CEO name";

      Order order = _customer.Orders[DomainObjectIDs.Order1];
      IndustrialSector industrialSector = _customer.IndustrialSector;

      var ceoEventReceiver = new DomainObjectEventReceiver (ceo);
      var customerEventReceiver = new DomainObjectEventReceiver (_customer);
      var orderEventReceiver = new DomainObjectEventReceiver (order);
      var industrialSectorEventReceiver = new DomainObjectEventReceiver (industrialSector);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      _customer.Committing += (sender, e) => order.OrderNumber = 1000;
      TestableClientTransaction.Committing += (sender1, args) =>
      {
        var customer = (Customer) args.DomainObjects.SingleOrDefault (obj => obj.ID == DomainObjectIDs.Customer1);
        if (customer != null)
          customer.IndustrialSector.Name = "New industrial sector name";
      };

      TestableClientTransaction.Commit();

      Assert.IsTrue (ceoEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (ceoEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (orderEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (orderEventReceiver.HasCommittedEventBeenCalled);

      Assert.IsTrue (industrialSectorEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (industrialSectorEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (2, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjectsForFirstCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committingDomainObjectsForSecondCommitEvent = clientTransactionEventReceiver.CommittingDomainObjectLists[1];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.That (committingDomainObjectsForFirstCommitEvent, Is.EquivalentTo (new DomainObject[] { _customer, ceo }));
      Assert.That (committingDomainObjectsForSecondCommitEvent, Is.EquivalentTo (new DomainObject[] { order, industrialSector }));
      Assert.That (committedDomainObjects, Is.EquivalentTo (new DomainObject[] { _customer, ceo, order, industrialSector }));
    }

    [Test]
    public void CommitWithoutChanges ()
    {
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (0, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);
    }

    [Test]
    public void CommitWithExistingObjectDeleted ()
    {
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ObjectID classWithAllDataTypesID = classWithAllDataTypes.ID;

      classWithAllDataTypes.Delete();

      TestableClientTransaction.Commit();

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);

      Assert.IsTrue (committingDomainObjects.Any (obj => obj.ID == classWithAllDataTypesID));
    }

    [Test]
    public void CommittedEventForObjectChangedBackToOriginal ()
    {
      _customer.Name = "New name";

      var customerEventReceiver = new DomainObjectEventReceiver (_customer);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);
      _customer.Committing += (sender, e) => { _customer.Name = _customer.Properties[typeof (Company), "Name"].GetOriginalValue<string>(); };

      TestableClientTransaction.Commit();

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsFalse (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (0, committedDomainObjects.Count);
    }

    [Test]
    public void CommittedEventForMarkAsChanged ()
    {
      _customer.MarkAsChanged();

      var customerEventReceiver = new DomainObjectEventReceiver (_customer);
      var clientTransactionEventReceiver = new ClientTransactionEventReceiver (TestableClientTransaction);

      TestableClientTransaction.Commit();

      Assert.IsTrue (customerEventReceiver.HasCommittingEventBeenCalled);
      Assert.IsTrue (customerEventReceiver.HasCommittedEventBeenCalled);

      Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjectLists.Count);
      Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjectLists.Count);

      var committingDomainObjects = clientTransactionEventReceiver.CommittingDomainObjectLists[0];
      var committedDomainObjects = clientTransactionEventReceiver.CommittedDomainObjectLists[0];

      Assert.AreEqual (1, committingDomainObjects.Count);
      Assert.AreEqual (1, committedDomainObjects.Count);

      Assert.Contains (_customer, committedDomainObjects);
      Assert.Contains (_customer, committingDomainObjects);
    }

    private void ClientTransaction_CommittingForModifyOtherObjectInClientTransactionCommitting (object sender, ClientTransactionEventArgs args)
    {
      var customer = args.DomainObjects[0] as Customer;
      if (customer != null)
        customer.Ceo.Name = "New CEO name";
    }
  }
}
