// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class DomainObjectEventManager
  {
    private readonly DomainObject _domainObject;
    private readonly bool _isConstructedDomainObject;

    private readonly Action<LoadMode> _onLoadedRaiser = delegate { };
    private readonly Action<PropertyChangeEventArgs> _onPropertyChangingRaiser = delegate { };
    private readonly Action<PropertyChangeEventArgs> _onPropertyChangedRaiser = delegate { };
    private readonly Action<RelationChangingEventArgs> _onRelationChangingRaiser = delegate { };
    private readonly Action<RelationChangedEventArgs> _onRelationChangedRaiser = delegate { };
    private readonly Action<EventArgs> _onDeletingRaiser = delegate { };
    private readonly Action<EventArgs> _onDeletedRaiser = delegate { };
    private readonly Action<EventArgs> _onCommittingRaiser = delegate { };
    private readonly Action<EventArgs> _onCommittedRaiser = delegate { };
    private readonly Action<EventArgs> _onRollingBackRaiser = delegate { };
    private readonly Action<EventArgs> _onRolledBackRaiser = delegate { };

    private bool _needsInitialLoadEvent;

    public DomainObjectEventManager (
        DomainObject domainObject,
        bool isConstructedDomainObject,
        Action<LoadMode> onLoadedRaiser,
        Action<PropertyChangeEventArgs> onPropertyChangingRaiser,
        Action<PropertyChangeEventArgs> onPropertyChangedRaiser,
        Action<RelationChangingEventArgs> onRelationChangingRaiser,
        Action<RelationChangedEventArgs> onRelationChangedRaiser,
        Action<EventArgs> onDeletingRaiser,
        Action<EventArgs> onDeletedRaiser,
        Action<EventArgs> onCommittingRaiser,
        Action<EventArgs> onCommittedRaiser,
        Action<EventArgs> onRollingBackRaiser,
        Action<EventArgs> onRolledBackRaiser)
    {
      _domainObject = domainObject;
      _isConstructedDomainObject = isConstructedDomainObject;
      _needsInitialLoadEvent = !isConstructedDomainObject;

      _onLoadedRaiser = onLoadedRaiser;
      _onPropertyChangingRaiser = onPropertyChangingRaiser;
      _onPropertyChangedRaiser = onPropertyChangedRaiser;
      _onRelationChangingRaiser = onRelationChangingRaiser;
      _onRelationChangedRaiser = onRelationChangedRaiser;
      _onDeletingRaiser = onDeletingRaiser;
      _onDeletedRaiser = onDeletedRaiser;
      _onCommittingRaiser = onCommittingRaiser;
      _onCommittedRaiser = onCommittedRaiser;
      _onRollingBackRaiser = onRollingBackRaiser;
      _onRolledBackRaiser = onRolledBackRaiser;
    }

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public bool IsConstructedDomainObject
    {
      get { return _isConstructedDomainObject; }
    }

    public void EndObjectLoading ()
    {
      LoadMode loadMode = _needsInitialLoadEvent ? LoadMode.WholeDomainObjectInitialized : LoadMode.DataContainerLoadedOnly;

      _needsInitialLoadEvent = false;

      DomainObjectMixinCodeGenerationBridge.OnDomainObjectLoaded (DomainObject, loadMode);
      _onLoadedRaiser (loadMode);
    }

    public void BeginPropertyValueChange (PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      var args = new PropertyChangeEventArgs (propertyValue, oldValue, newValue);
      _onPropertyChangingRaiser (args);
    }

    public void EndPropertyValueChange (PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      var args = new PropertyChangeEventArgs (propertyValue, oldValue, newValue);
      _onPropertyChangedRaiser (args);
    }

    public void BeginRelationChange (string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
      _onRelationChangingRaiser (args);
    }

    public void EndRelationChange (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _onRelationChangedRaiser (new RelationChangedEventArgs (propertyName));
    }

    public void BeginDelete ()
    {
      _onDeletingRaiser (EventArgs.Empty);
    }

    public void EndDelete ()
    {
      _onDeletedRaiser (EventArgs.Empty);
    }

    public void BeginCommit ()
    {
      _onCommittingRaiser (EventArgs.Empty);
    }

    public void EndCommit ()
    {
      _onCommittedRaiser (EventArgs.Empty);
    }

    public void BeginRollback ()
    {
      _onRollingBackRaiser (EventArgs.Empty);
    }

    public void EndRollback ()
    {
      _onRolledBackRaiser (EventArgs.Empty);
    }
  }
}