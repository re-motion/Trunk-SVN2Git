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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerCollectionTest : ClientTransactionBaseTest
  {
    private DataContainer _dataContainer;
    private DataContainerCollection _collection;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      _collection = new DataContainerCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_dataContainer);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void ObjectIDIndexer ()
    {
      _collection.Add (_dataContainer);
      Assert.AreSame (_dataContainer, _collection[_dataContainer.ID]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_dataContainer);
      Assert.AreSame (_dataContainer, _collection[0]);
    }

    [Test]
    public void ContainsObjectIDTrue ()
    {
      _collection.Add (_dataContainer);
      Assert.IsTrue (_collection.Contains (_dataContainer.ID));
    }

    [Test]
    public void ContainsObjectIDFalse ()
    {
      Assert.IsFalse (_collection.Contains (_dataContainer.ID));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_dataContainer);

      DataContainerCollection copiedCollection = new DataContainerCollection (_collection, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_dataContainer, copiedCollection[0]);
    }

    [Test]
    public void GetEmptyDifference ()
    {
      DataContainerCollection difference = _collection.GetDifference (new DataContainerCollection ());
      Assert.AreEqual (0, difference.Count);
    }

    [Test]
    public void GetDifferenceFromEmptySet ()
    {
      _collection.Add (_dataContainer);
      DataContainerCollection difference = _collection.GetDifference (new DataContainerCollection ());
      Assert.AreEqual (1, difference.Count);
      Assert.AreSame (_dataContainer, difference[0]);
    }

    [Test]
    public void GetDifference ()
    {
      DataContainer differentDataContainer = TestDataContainerFactory.CreateOrder2DataContainer ();

      _collection.Add (_dataContainer);
      _collection.Add (differentDataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection ();

      secondCollection.Add (_dataContainer);

      DataContainerCollection difference = _collection.GetDifference (secondCollection);

      Assert.AreEqual (1, difference.Count);
      Assert.AreSame (differentDataContainer, difference[0]);
    }

    [Test]
    public void EmptyMerge ()
    {
      DataContainerCollection mergedCollection = _collection.Merge (new DataContainerCollection ());
      Assert.AreEqual (0, mergedCollection.Count);
    }

    [Test]
    public void MergeCollectionAndEmptyCollection ()
    {
      _collection.Add (_dataContainer);
      DataContainerCollection mergedCollection = _collection.Merge (new DataContainerCollection ());

      Assert.AreEqual (1, mergedCollection.Count);
      Assert.AreSame (_dataContainer, mergedCollection[0]);
    }

    [Test]
    public void MergeEmptyCollectionAndCollection ()
    {
      DataContainerCollection secondCollection = new DataContainerCollection ();
      secondCollection.Add (_dataContainer);

      DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

      Assert.AreEqual (0, mergedCollection.Count);
    }

    [Test]
    public void MergeTwoCollectionsWithEqualDataContainer ()
    {
      _collection.Add (_dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection ();
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      secondCollection.Add (container);

      DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

      Assert.AreEqual (1, mergedCollection.Count);
      Assert.AreSame (container, mergedCollection[0]);
    }

    [Test]
    public void MergeTwoCollections ()
    {
      _collection.Add (_dataContainer);
      DataContainer order2 = TestDataContainerFactory.CreateOrder2DataContainer ();
      _collection.Add (order2);

      DataContainerCollection secondCollection = new DataContainerCollection ();
      DataContainer order1 = TestDataContainerFactory.CreateOrder1DataContainer ();
      secondCollection.Add (order1);

      DataContainerCollection mergedCollection = _collection.Merge (secondCollection);

      Assert.AreEqual (2, mergedCollection.Count);
      Assert.AreSame (order1, mergedCollection[_dataContainer.ID]);
      Assert.AreSame (order2, mergedCollection[order2.ID]);
    }

    [Test]
    public void GetByOriginalState ()
    {
      _collection.Add (_dataContainer);
      DataContainerCollection originalContainers = _collection.GetByState (StateType.Unchanged);

      Assert.IsNotNull (originalContainers);
      Assert.AreEqual (1, originalContainers.Count);
      Assert.AreSame (_dataContainer, originalContainers[0]);
    }

    [Test]
    public void GetByChangedState ()
    {
      _collection.Add (_dataContainer);
      _collection.Add (TestDataContainerFactory.CreateCustomer1DataContainer ());

      _dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 10;

      DataContainerCollection changedContainers = _collection.GetByState (StateType.Changed);

      Assert.IsNotNull (changedContainers);
      Assert.AreEqual (1, changedContainers.Count);
      Assert.AreSame (_dataContainer, changedContainers[0]);
    }

    [Test]
    public void RemoveByID ()
    {
      _collection.Add (_dataContainer);
      Assert.AreEqual (1, _collection.Count);

      _collection.Remove (_dataContainer.ID);
      Assert.AreEqual (0, _collection.Count);
    }

    [Test]
    public void RemoveByDataContainer ()
    {
      _collection.Add (_dataContainer);
      Assert.AreEqual (1, _collection.Count);

      _collection.Remove (_dataContainer);
      Assert.AreEqual (0, _collection.Count);
    }

    [Test]
    public void RemoveByIndex ()
    {
      _collection.Add (_dataContainer);
      Assert.AreEqual (1, _collection.Count);

      _collection.Remove (0);
      Assert.AreEqual (0, _collection.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RemoveNullDataContainer ()
    {
      _collection.Remove ((DataContainer) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RemoveNullObjectID ()
    {
      _collection.Remove ((ObjectID) null);
    }

    [Test]
    public void ContainsDataContainerTrue ()
    {
      _collection.Add (_dataContainer);

      Assert.IsTrue (_collection.Contains (_dataContainer));
    }

    [Test]
    public void ContainsDataContainerFalse ()
    {
      _collection.Add (_dataContainer);

      DataContainer copy = DataContainer.CreateNew (_dataContainer.ID);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsDataContainerNull ()
    {
      _collection.Contains ((DataContainer) null);
    }

    [Test]
    public void Clear ()
    {
      _collection.Add (_dataContainer);
      Assert.AreEqual (1, _collection.Count);

      _collection.Clear ();
      Assert.AreEqual (0, _collection.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetByInvalidState ()
    {
      _collection.GetByState ((StateType) 1000);
    }

    [Test]
    public void Join ()
    {
      DataContainer firstDataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainerCollection firstCollection = new DataContainerCollection ();
      firstCollection.Add (firstDataContainer);

      DataContainer secondDataContainer = TestDataContainerFactory.CreateOrder2DataContainer ();
      DataContainerCollection secondCollection = new DataContainerCollection ();
      secondCollection.Add (secondDataContainer);

      DataContainerCollection joinedCollection = DataContainerCollection.Join (firstCollection, secondCollection);
      Assert.AreEqual (2, joinedCollection.Count);
      Assert.AreEqual (firstDataContainer.ID, joinedCollection[0].ID);
      Assert.AreEqual (secondDataContainer.ID, joinedCollection[1].ID);
    }

    [Test]
    public void JoinWithSameDataContainer ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainerCollection firstCollection = new DataContainerCollection ();
      firstCollection.Add (dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection ();
      secondCollection.Add (dataContainer);

      DataContainerCollection joinedCollection = DataContainerCollection.Join (firstCollection, secondCollection);
      Assert.AreEqual (1, joinedCollection.Count);
      Assert.AreEqual (dataContainer.ID, joinedCollection[0].ID);
    }

    [Test]
    public void JoinWithDataContainersOfSameID ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainerCollection firstCollection = new DataContainerCollection ();
      firstCollection.Add (dataContainer);

      DataContainerCollection secondCollection = new DataContainerCollection ();
      secondCollection.Add (TestDataContainerFactory.CreateOrder1DataContainer ());

      DataContainerCollection joinedCollection = DataContainerCollection.Join (firstCollection, secondCollection);
      Assert.AreEqual (1, joinedCollection.Count);
      Assert.AreSame (dataContainer, joinedCollection[0]);
    }
  }
}
