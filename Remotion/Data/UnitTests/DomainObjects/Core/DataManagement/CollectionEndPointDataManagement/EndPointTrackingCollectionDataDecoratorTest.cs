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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class EndPointTrackingCollectionDataDecoratorTest : StandardMappingTest
  {
    private IDomainObjectCollectionData _wrappedData;
    private IRelationEndPointProvider _endPointProviderStub;

    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;

    private IObjectEndPoint _orderItemEndPoint1;
    private IObjectEndPoint _orderItemEndPoint2;
    private IObjectEndPoint _orderItemEndPoint3;
    private IRelationEndPointDefinition _definition;

    private EndPointTrackingCollectionDataDecorator _decorator;

    public override void SetUp ()
    {
      base.SetUp();

      _wrappedData = new DomainObjectCollectionData();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();

      _orderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _orderItem2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      _orderItem3 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      
      _definition = Configuration.ClassDefinitions[typeof (OrderItem)].GetRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");

      _orderItemEndPoint1 = MockRepository.GenerateStub<IObjectEndPoint> ();
      _orderItemEndPoint2 = MockRepository.GenerateStub<IObjectEndPoint> ();
      _orderItemEndPoint3 = MockRepository.GenerateStub<IObjectEndPoint> ();

      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (_orderItem1, oi => oi.Order)))
          .Return (_orderItemEndPoint1);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (_orderItem2, oi => oi.Order)))
          .Return (_orderItemEndPoint2);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (_orderItem3, oi => oi.Order)))
          .Return (_orderItemEndPoint3);

      _wrappedData.Insert (0, _orderItem1);
      _wrappedData.Insert (1, _orderItem2);

      _decorator = new EndPointTrackingCollectionDataDecorator (_wrappedData, _endPointProviderStub, _definition);
    }

    [Test]
    public void Initialization_WithItemsInCollection ()
    {
      Assert.That (_decorator.GetOppositeEndPoints(), Is.EquivalentTo (new[] { _orderItemEndPoint1, _orderItemEndPoint2 }));
    }

    [Test]
    public void Clear ()
    {
      _decorator.Clear ();

      Assert.That (_wrappedData.ToArray (), Is.Empty);
      Assert.That (_decorator.GetOppositeEndPoints (), Is.Empty);
    }

    [Test]
    public void Insert ()
    {
      _decorator.Insert (0, _orderItem3);

      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _orderItem3, _orderItem1, _orderItem2 }));
      Assert.That (_decorator.GetOppositeEndPoints(), Is.EquivalentTo (new[] { _orderItemEndPoint3, _orderItemEndPoint1, _orderItemEndPoint2 }));
    }

    [Test]
    public void Remove_Object ()
    {
      _decorator.Remove (_orderItem2);

      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _orderItem1 }));
      Assert.That (_decorator.GetOppositeEndPoints (), Is.EquivalentTo (new[] { _orderItemEndPoint1 }));
    }

    [Test]
    public void Remove_ID ()
    {
      _decorator.Remove (_orderItem2.ID);

      Assert.That (_wrappedData.ToArray (), Is.EqualTo (new[] { _orderItem1 }));
      Assert.That (_decorator.GetOppositeEndPoints (), Is.EquivalentTo (new[] { _orderItemEndPoint1 }));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (RelationEndPointID.Create (_orderItem1, oi => oi.Order)))
          .Return (new SerializableObjectEndPointFake (DomainObjectMother.CreateFakeObject<Order>()));
      _wrappedData.Clear();
      _wrappedData.Add (_orderItem1);
      var decorator = new EndPointTrackingCollectionDataDecorator (_wrappedData, endPointProviderStub, _definition);
      
      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedInstance.EndPointProvider, Is.Not.Null);
      Assert.That (deserializedInstance.ObjectEndPointDefinition, Is.Not.Null);
      Assert.That (deserializedInstance.OppositeEndPoints.Count, Is.EqualTo (1));
      Assert.That (deserializedInstance.Count, Is.EqualTo (1));
    }
  }
}