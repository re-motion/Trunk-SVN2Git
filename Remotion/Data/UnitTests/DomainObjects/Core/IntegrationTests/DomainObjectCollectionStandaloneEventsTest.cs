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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class DomainObjectCollectionStandaloneEventsTest : ClientTransactionBaseTest
  {
    private Customer _customer1;
    private Customer _customer2;
    private Customer _customer3NotInCollection;

    private DomainObjectCollection _collection;

    public override void SetUp ()
    {
      base.SetUp();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
      _customer3NotInCollection = Customer.GetObject (DomainObjectIDs.Customer3);

      _collection = new DomainObjectCollection (new[] { _customer1, _customer2 }, typeof (Customer));
    }

    [Test]
    public void Add_Events ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, false);

      _collection.Add (_customer3NotInCollection);

      Assert.That (_collection.Count, Is.EqualTo (3));

      Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (true));

      Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer3NotInCollection));
      Assert.That (eventReceiver.AddedDomainObject, Is.SameAs (_customer3NotInCollection));
    }

    [Test]
    public void Add_Events_Cancel ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, true);

      try
      {
        _collection.Add (_customer3NotInCollection);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_collection.Count, Is.EqualTo (2));
        Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer3NotInCollection));
        Assert.That (eventReceiver.AddedDomainObject, Is.Null);
      }
    }
    
    [Test]
    public void Clear_Events_Cancel ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, true);

      try
      {
        _collection.Clear ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_collection.Count, Is.EqualTo (2));
        Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
      }
    }

    [Test]
    public void Remove_Events ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, false);

      _collection.Remove (_customer1.ID);

      Assert.That (_collection.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.RemovingDomainObjects[0], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovedDomainObjects[0], Is.SameAs (_customer1));
    }


    [Test]
    public void Remove_Null_Events ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((DomainObject) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }

    [Test]
    public void Remove_ObjectNotInCollection_Events ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      collection.Remove (customer);

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }
    
    [Test]
    public void Remove_Events_Cancel ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, true);

      try
      {
        _collection.Remove (_customer1.ID);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_collection.Count, Is.EqualTo (2));
        Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
        Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (false));
        Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
        Assert.That (eventReceiver.RemovingDomainObjects[0], Is.SameAs (_customer1));
      }
    }

    [Test]
    public void Remove_ID_Null_Events ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      var eventReceiver = new DomainObjectCollectionEventReceiver (collection);

      try
      {
        collection.Remove ((ObjectID) null);
        Assert.Fail ("ArgumentNullException was expected");
      }
      catch (ArgumentNullException)
      {
      }

      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.False);
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.False);
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (0));
    }


    [Test]
    public void Insert_Events ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));

      var eventReceiver = new DomainObjectCollectionEventReceiver (
          collection, false);

      collection.Insert (0, _customer1);

      Assert.That (collection.Count, Is.EqualTo (1));
      Assert.That (eventReceiver.HasAddingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasAddedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.AddingDomainObject, Is.SameAs (_customer1));
      Assert.That (eventReceiver.AddedDomainObject, Is.SameAs (_customer1));
    }


    [Test]
    public void Item_Set_Events ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection);

      _collection[0] = _customer3NotInCollection;

      var expectedStates = new ChangeState[]
                           {
                               new CollectionChangeState (_collection, _customer1, "1. Removing event"),
                               new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event"),
                               new CollectionChangeState (_collection, _customer1, "3. Removed event"),
                               new CollectionChangeState (_collection, _customer3NotInCollection, "4. Added event")
                           };

      Assert.That (_collection[0], Is.SameAs (_customer3NotInCollection));
      Assert.That (_collection.Count, Is.EqualTo (2));

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void Item_Set_Events_Cancel ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection, 2);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[]
                             {
                                 new CollectionChangeState (_collection, _customer1, "1. Removing event"),
                                 new CollectionChangeState (_collection, _customer3NotInCollection, "2. Adding event")
                             };

        Assert.That (_collection[0], Is.SameAs (_customer1));
        Assert.That (_collection.Count, Is.EqualTo (2));

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void Item_Set_Null_Events_Cancel ()
    {
      var eventReceiver = new SequenceEventReceiver (_collection, 1);

      try
      {
        _collection[0] = _customer3NotInCollection;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        var expectedStates = new ChangeState[] { new CollectionChangeState (_collection, _customer1, "1. Removing event") };

        Assert.That (_collection[0], Is.SameAs (_customer1));
        Assert.That (_collection.Count, Is.EqualTo (2));

        eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void Clear_Events ()
    {
      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection, false);

      _collection.Clear ();

      Assert.That (_collection.Count, Is.EqualTo (0));
      Assert.That (eventReceiver.HasRemovingEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.HasRemovedEventBeenCalled, Is.EqualTo (true));
      Assert.That (eventReceiver.RemovingDomainObjects.Count, Is.EqualTo (2));
      Assert.That (eventReceiver.RemovedDomainObjects.Count, Is.EqualTo (2));
      Assert.That (eventReceiver.RemovingDomainObjects[_customer1.ID], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovingDomainObjects[_customer2.ID], Is.SameAs (_customer2));
      Assert.That (eventReceiver.RemovedDomainObjects[_customer1.ID], Is.SameAs (_customer1));
      Assert.That (eventReceiver.RemovedDomainObjects[_customer2.ID], Is.SameAs (_customer2));
    }

    [Test]
    public void GetNonNotifyingData_DoesNotRaiseEvents ()
    {
      IDomainObjectCollectionData nonNotifyingData = CallGetNonNotifyingData (_collection);

      var eventReceiver = new DomainObjectCollectionEventReceiver (_collection);

      nonNotifyingData.Insert (1, _customer3NotInCollection);

      Assert.That (eventReceiver.AddingDomainObject, Is.Null);
      Assert.That (eventReceiver.AddedDomainObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\nParameter name: domainObject"
        )]
    public void GetNonNotifyingData_PerformsArgumentChecks ()
    {
      IDomainObjectCollectionData nonNotifyingData = CallGetNonNotifyingData (_collection);

      nonNotifyingData.Insert (1, _customer1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetNonNotifyingData_PerformsTypeChecks ()
    {
      IDomainObjectCollectionData nonNotifyingData = CallGetNonNotifyingData (_collection);

      nonNotifyingData.Insert (1, Order.NewObject ());
    }

    [Test]
    public void GetNonNotifyingData_RepresentsCollectionData ()
    {
      IDomainObjectCollectionData nonNotifyingData = CallGetNonNotifyingData (_collection);

      _collection.Add (_customer3NotInCollection);
      Assert.That (nonNotifyingData.ToArray (), Is.EqualTo (new[] { _customer1, _customer2, _customer3NotInCollection }));

      nonNotifyingData.Remove (_customer1.ID);
      Assert.That (_collection, Is.EqualTo (new[] { _customer2, _customer3NotInCollection }));
    }

    [Test]
    public void GetNonNotifyingData_UsesUndecoratedDataStore ()
    {
      var dataStore = new DomainObjectCollectionData ();

      var dataDecoratorMock = MockRepository.GenerateMock<IDomainObjectCollectionData> ();
      dataDecoratorMock.Stub (mock => mock.GetUndecoratedDataStore ()).Return (dataStore);

      var collection = new DomainObjectCollection (dataDecoratorMock);
      IDomainObjectCollectionData nonNotifyingData = CallGetNonNotifyingData (collection);

      nonNotifyingData.Insert (0, _customer1);
      dataDecoratorMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));

      Assert.That (dataStore.ToArray (), Is.EqualTo (new[] { _customer1 }));
    }

    private IDomainObjectCollectionData CallGetNonNotifyingData (DomainObjectCollection collection)
    {
      return (IDomainObjectCollectionData) PrivateInvoke.InvokeNonPublicMethod (collection, "GetNonNotifyingData");
    }

  }
}