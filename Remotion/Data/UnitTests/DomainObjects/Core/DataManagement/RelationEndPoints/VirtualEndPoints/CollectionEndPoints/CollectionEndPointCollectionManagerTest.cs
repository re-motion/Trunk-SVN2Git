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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class CollectionEndPointCollectionManagerTest : StandardMappingTest
  {
    private IAssociatedCollectionDataStrategyFactory _associatedCollectionDataStrategyFactoryMock;
    private CollectionEndPointCollectionManager _manager;

    private RelationEndPointID _endPointID;

    private ICollectionEndPoint _endPointStub;
    private IDomainObjectCollectionData _dataStrategyStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _associatedCollectionDataStrategyFactoryMock = MockRepository.GenerateStrictMock<IAssociatedCollectionDataStrategyFactory>();
      _manager = new CollectionEndPointCollectionManager (_associatedCollectionDataStrategyFactoryMock);

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
    
      _endPointStub = CreateEndPointStubWithID (_endPointID);

      _dataStrategyStub = MockRepository.GenerateStub<IDomainObjectCollectionData> ();
      _dataStrategyStub.Stub (stub => stub.RequiredItemType).Return (typeof (Order));
    }

    [Test]
    public void GetInitialCollection_Once ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub);
      _associatedCollectionDataStrategyFactoryMock.Replay();

      var result = _manager.GetInitialCollection (_endPointStub);

      _associatedCollectionDataStrategyFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (result), Is.SameAs (_dataStrategyStub));
    }

    [Test]
    public void GetInitialCollection_Twice_ForSameID ()
    {
      _associatedCollectionDataStrategyFactoryMock
          .Expect (mock => mock.CreateDataStrategyForEndPoint (_endPointStub))
          .Return (_dataStrategyStub)
          .Repeat.Once();
      _associatedCollectionDataStrategyFactoryMock.Replay ();

      var result1 = _manager.GetInitialCollection (_endPointStub);

      ICollectionEndPoint secondEndPointStub = CreateEndPointStubWithID(_endPointID);

      var result2 = _manager.GetInitialCollection (secondEndPointStub);

      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void GetInitialCollection_CollectionWithWrongCtor ()
    {
      var classDefinition = GetTypeDefinition (typeof (DomainObjectWithCollectionMissingCtor));
      var relationEndPointDefinition = GetEndPointDefinition (typeof (DomainObjectWithCollectionMissingCtor), "OppositeObjects");
      var endPointID = RelationEndPointID.Create (new ObjectID (classDefinition, Guid.NewGuid ()), relationEndPointDefinition);

      var endPointStub = CreateEndPointStubWithID (endPointID);

      _associatedCollectionDataStrategyFactoryMock
          .Stub (mock => mock.CreateDataStrategyForEndPoint (endPointStub))
          .Return (_dataStrategyStub);

      Assert.That (() => _manager.GetInitialCollection (endPointStub), Throws.TypeOf<MissingMethodException> ()
          .With.Message.ContainsSubstring ("does not provide a constructor taking an IDomainObjectCollectionData object"));
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      var result = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);

      Assert.That (result, Is.TypeOf<OrderCollection> ());
      Assert.That (DomainObjectCollectionDataTestHelper.GetDataStrategy (result), Is.SameAs (_dataStrategyStub));
    }

    [Test]
    public void GetCollectionWithOriginalData_Twice ()
    {
      var result1 = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);
      var result2 = _manager.GetCollectionWithOriginalData (_endPointStub, _dataStrategyStub);

      Assert.That (result2, Is.Not.SameAs (result1));
    }

    [Test]
    public void AssociateCollectionWithEndPoint ()
    {
      var oldCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection>();
      oldCollectionMock.Stub (mock => ((IAssociatableDomainObjectCollection) mock).IsAssociatedWith (_endPointStub)).Return (true);
      oldCollectionMock.Expect (mock => ((IAssociatableDomainObjectCollection) mock).TransformToStandAlone ());
      oldCollectionMock.Replay();

      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Expect (
              mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointStub, _associatedCollectionDataStrategyFactoryMock));
      newCollectionMock.Replay();

      _endPointStub.Stub (stub => stub.Collection).Return (oldCollectionMock);
      
      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollectionMock);

      oldCollectionMock.VerifyAllExpectations ();
      newCollectionMock.VerifyAllExpectations ();
    }

    [Test]
    public void AssociateCollectionWithEndPoint_OldCollectionNoLongerAssociated ()
    {
      var oldCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      oldCollectionMock.Stub (mock => ((IAssociatableDomainObjectCollection) mock).IsAssociatedWith (_endPointStub)).Return (false);
      oldCollectionMock.Replay ();

      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Expect (
              mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointStub, _associatedCollectionDataStrategyFactoryMock));
      newCollectionMock.Replay ();

      _endPointStub.Stub (stub => stub.Collection).Return (oldCollectionMock);

      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollectionMock);

      oldCollectionMock.VerifyAllExpectations ();
      newCollectionMock.VerifyAllExpectations ();
    }

    [Test]
    public void AssociateCollectionWithEndPoint_RemembersTheNewCollection ()
    {
      _associatedCollectionDataStrategyFactoryMock.Stub (stub => stub.CreateDataStrategyForEndPoint (_endPointStub)).Return (_dataStrategyStub);

      var defaultCollectionBefore = _manager.GetInitialCollection (_endPointStub);
      Assert.That (_manager.GetInitialCollection (_endPointStub), Is.SameAs (defaultCollectionBefore));

      var oldCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      oldCollectionMock.Stub (mock => ((IAssociatableDomainObjectCollection) mock).IsAssociatedWith (_endPointStub)).Return (false);

      var newCollectionMock = MockRepository.GenerateStrictMock<DomainObjectCollection, IAssociatableDomainObjectCollection> ();
      newCollectionMock
          .Stub (
              mock => ((IAssociatableDomainObjectCollection) mock).TransformToAssociated (_endPointStub, _associatedCollectionDataStrategyFactoryMock));

      _endPointStub.Stub (stub => stub.Collection).Return (defaultCollectionBefore);

      _manager.AssociateCollectionWithEndPoint (_endPointStub, newCollectionMock);

      var defaultCollectionAfter = _manager.GetInitialCollection (_endPointStub);
      Assert.That (_manager.GetInitialCollection (_endPointStub), Is.SameAs (defaultCollectionAfter));

      Assert.That (defaultCollectionAfter, Is.SameAs (newCollectionMock));
    }

    [Test]
    public void Serialization ()
    {
      var instance = new CollectionEndPointCollectionManager (new SerializableAssociatedCollectionDataStrategyFactoryFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.DataStrategyFactory, Is.Not.Null);
    }

    private ICollectionEndPoint CreateEndPointStubWithID (RelationEndPointID relationEndPointID)
    {
      var secondEndPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();
      secondEndPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      return secondEndPointStub;
    }
  }
}