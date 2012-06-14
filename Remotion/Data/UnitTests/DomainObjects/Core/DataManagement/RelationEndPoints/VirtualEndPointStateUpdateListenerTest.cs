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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualEndPointStateUpdateListenerTest : StandardMappingTest
  {
    private ClientTransactionEventSinkWithMock _eventSinkWithWock;
    private RelationEndPointID _endPointID;

    private VirtualEndPointStateUpdateListener _stateUpdateListener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _eventSinkWithWock = ClientTransactionEventSinkWithMock.CreateWithDynamicMock();
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      _stateUpdateListener = new VirtualEndPointStateUpdateListener (_eventSinkWithWock);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_Null ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated (_endPointID, null);

      _eventSinkWithWock.AssertWasCalledMock (mock => mock.VirtualRelationEndPointStateUpdated (_eventSinkWithWock.ClientTransaction, _endPointID, null));
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_True ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated (_endPointID, true);

      _eventSinkWithWock.AssertWasCalledMock (mock => mock.VirtualRelationEndPointStateUpdated (_eventSinkWithWock.ClientTransaction, _endPointID, true));
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_False ()
    {
      _stateUpdateListener.VirtualEndPointStateUpdated (_endPointID, false);

      _eventSinkWithWock.AssertWasCalledMock (mock => mock.VirtualRelationEndPointStateUpdated (_eventSinkWithWock.ClientTransaction, _endPointID, false));
    }

    [Test]
    public void Serializable ()
    {
      var eventSink = new SerializableClientTransactionEventSinkFake();
      var stateUpdateListener = new VirtualEndPointStateUpdateListener (eventSink);

      var deserializedInstance = Serializer.SerializeAndDeserialize (stateUpdateListener);

      Assert.That (deserializedInstance.TransactionEventSink, Is.Not.Null);
    }
  }
}