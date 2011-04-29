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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualEndPointStateUpdateListenerTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private IClientTransactionListener _transactionListenerMock;
    private RelationEndPointID _endPointID;

    private VirtualEndPointStateUpdateListener _stateUpdateListener;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _transactionListenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_clientTransaction);
      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");

      _stateUpdateListener = new VirtualEndPointStateUpdateListener (_clientTransaction, _endPointID);
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_Null ()
    {
      _stateUpdateListener.StateUpdated (null);

      _transactionListenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, null));
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_True ()
    {
      _stateUpdateListener.StateUpdated (true);

      _transactionListenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, true));
    }

    [Test]
    public void StateUpdates_RoutedToTransactionEventSink_False ()
    {
      _stateUpdateListener.StateUpdated (false);

      _transactionListenerMock.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_clientTransaction, _endPointID, false));
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      var stateUpdateListener = new VirtualEndPointStateUpdateListener (clientTransaction, _endPointID);

      var deserializedInstance = Serializer.SerializeAndDeserialize (stateUpdateListener);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EndPointID, Is.EqualTo (_endPointID));
    }
  }
}