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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Listens to the events raised by a <see cref="DataContainer"/>, forwarding them to the <see cref="IClientTransactionEventSink"/>.
  /// </summary>
  [Serializable]
  public class DataContainerEventListener : IDataContainerEventListener
  {
    private readonly IClientTransactionEventSink _eventSink;

    public DataContainerEventListener (IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      _eventSink = eventSink;
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      _eventSink.RaiseEvent ((tx, l) => l.PropertyValueReading (tx, dataContainer, propertyDefinition, valueAccess));
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      _eventSink.RaiseEvent ((tx, l) => l.PropertyValueRead (tx, dataContainer, propertyDefinition, value, valueAccess));
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      _eventSink.RaiseEvent ((tx, l) => l.PropertyValueChanging (tx, dataContainer, propertyDefinition, oldValue, newValue));
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      _eventSink.RaiseEvent ((tx, l) => l.PropertyValueChanged (tx, dataContainer, propertyDefinition, oldValue, newValue));
    }

    public void StateUpdated (DataContainer dataContainer, StateType state)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      _eventSink.RaiseEvent ((tx, l) => l.DataContainerStateUpdated (tx, dataContainer, state));
    }
  }
}