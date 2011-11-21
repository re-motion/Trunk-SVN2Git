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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    
    private IPersistenceStrategy _persistenceStrategyMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;
    private IDataContainerLifetimeManager _lifetimeManagerStub;

    private ObjectLoader _objectLoader;

    private DomainObject _domainObject1;
    private DomainObject _domainObject2;

    private IQuery _fakeQuery;
    private ILoadedObjectData _loadedObjectDataStub1;
    private ILoadedObjectData _loadedObjectDataStub2;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent>();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider>();
      _lifetimeManagerStub = _mockRepository.StrictMock<IDataContainerLifetimeManager>();

      _objectLoader = new ObjectLoader (_persistenceStrategyMock, _loadedObjectDataRegistrationAgentMock, _lifetimeManagerStub, _loadedObjectDataProviderStub);

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order2);

      _fakeQuery = CreateFakeQuery();

      _loadedObjectDataStub1 = MockRepository.GenerateStub<ILoadedObjectData> ();
      _loadedObjectDataStub2 = MockRepository.GenerateStub<ILoadedObjectData> ();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock.Expect (mock => mock.LoadObjectData (_domainObject1.ID)).Return (_loadedObjectDataStub1);
      _loadedObjectDataRegistrationAgentMock.Expect (mock => mock.RegisterIfRequired (Arg.Is (_loadedObjectDataStub1), Arg.Is (_lifetimeManagerStub)));

      _mockRepository.ReplayAll();

      var result = _objectLoader.LoadObject (_domainObject1.ID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_loadedObjectDataStub1));
    }

    [Test]
    public void LoadObjects ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject1);
      _loadedObjectDataStub2.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject2);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID }), 
              Arg.Is (true)))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }),
              Arg.Is (_lifetimeManagerStub)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID }, true);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    [Test]
    public void LoadObjects_WithNotFoundObjects ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_domainObject1);

      var nullLoadedObjectData1 = new NullLoadedObjectData ();
      var nullLoadedObjectData2 = new NullLoadedObjectData ();
      _persistenceStrategyMock
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { _domainObject1.ID, _domainObject2.ID, DomainObjectIDs.Order3 }),
              Arg.Is (false)))
          .Return (new[] { _loadedObjectDataStub1, nullLoadedObjectData1, nullLoadedObjectData2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, nullLoadedObjectData1, nullLoadedObjectData2 }), 
              Arg.Is (_lifetimeManagerStub)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { _domainObject1.ID, _domainObject2.ID, DomainObjectIDs.Order3 }, false);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, nullLoadedObjectData1, nullLoadedObjectData2 }));
    }

    [Test]
    public void GetOrLoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order3, "OrderTicket");

      _persistenceStrategyMock
          .Expect (mock => mock.ResolveObjectRelationData (endPointID, _loadedObjectDataProviderStub))
          .Return (_loadedObjectDataStub1);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (_loadedObjectDataStub1, _lifetimeManagerStub));
      _mockRepository.ReplayAll();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_loadedObjectDataStub1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "GetOrLoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.GetOrLoadRelatedObject (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject1.ID, "OrderItems");
      _objectLoader.GetOrLoadRelatedObject (endPointID);
    }

    [Test]
    public void GetOrLoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _persistenceStrategyMock
          .Expect (mock => mock.ResolveCollectionRelationData (endPointID, _loadedObjectDataProviderStub))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }), 
                  Arg.Is (_lifetimeManagerStub)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.GetOrLoadRelatedObjects (endPointID);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }),
              Arg.Is (_lifetimeManagerStub)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult (_fakeQuery);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          _domainObject1.ID.StorageProviderDefinition,
          "TEST",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }
  }
}