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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerEventListenerTest : StandardMappingTest
  {
    private ClientTransactionEventSinkWithMock _eventSinkWithMock;
    private DataContainerEventListener _eventListener;

    private DataContainer _dataContainer;
    private PropertyValue _propertyValue;

    public override void SetUp ()
    {
      base.SetUp ();

      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock(ClientTransaction.CreateRootTransaction());
      _eventListener = new DataContainerEventListener (_eventSinkWithMock);

      _dataContainer = DataContainerObjectMother.CreateDataContainer();
      _propertyValue = PropertyValueObjectMother.Create();
    }

    [Test]
    public void PropertyValueReading ()
    {
      CheckEventDelegation (
          l => l.PropertyValueReading (_dataContainer, _propertyValue, ValueAccess.Original), 
          (tx, mock) => mock.PropertyValueReading (tx, _dataContainer, _propertyValue, ValueAccess.Original));
    }

    [Test]
    public void PropertyValueRead ()
    {
      CheckEventDelegation (
          l => l.PropertyValueRead (_dataContainer, _propertyValue, "value", ValueAccess.Original),
          (tx, mock) => mock.PropertyValueRead (tx, _dataContainer, _propertyValue, "value", ValueAccess.Original));
    }

    [Test]
    public void PropertyValueChanging ()
    {
      CheckEventDelegation (
          l => l.PropertyValueChanging (_dataContainer, _propertyValue, "oldValue", "newValue"),
          (tx, mock) => mock.PropertyValueChanging (tx, _dataContainer, _propertyValue, "oldValue", "newValue"));
    }

    [Test]
    public void PropertyValueChanged ()
    {
      CheckEventDelegation (
          l => l.PropertyValueChanged (_dataContainer, _propertyValue, "oldValue", "newValue"),
          (tx, mock) => mock.PropertyValueChanged (tx, _dataContainer, _propertyValue, "oldValue", "newValue"));
    }

    [Test]
    public void StateUpdated ()
    {
      CheckEventDelegation (
          l => l.StateUpdated (_dataContainer, StateType.New),
          (tx, mock) => mock.DataContainerStateUpdated (tx, _dataContainer, StateType.New));
    }
    
    [Test]
    public void Serializable ()
    {
      var instance = new DataContainerEventListener (new SerializableClientTransactionEventSinkFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
    }

    private void CheckEventDelegation (Action<DataContainerEventListener> action, Action<ClientTransaction, IClientTransactionListener> expectedEvent)
    {
      _eventSinkWithMock.ExpectMock (mock => expectedEvent (_eventSinkWithMock.ClientTransaction, mock));
      _eventSinkWithMock.ReplayMock ();

      action (_eventListener);

      _eventSinkWithMock.VerifyMock ();
    }
  }
}