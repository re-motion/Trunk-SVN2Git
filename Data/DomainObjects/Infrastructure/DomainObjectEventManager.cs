/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class DomainObjectEventManager
  {
    private readonly DomainObject _domainObject;
    private readonly bool _isConstructedDomainObject;

    private bool _needsInitialLoadEvent;

    public DomainObjectEventManager (DomainObject domainObject, bool isConstructedDomainObject)
    {
      _domainObject = domainObject;
      _isConstructedDomainObject = isConstructedDomainObject;
      _needsInitialLoadEvent = !isConstructedDomainObject;
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
      DomainObject.OnLoaded (loadMode);
    }

    public void BeginPropertyValueChange (PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      var args = new PropertyChangeEventArgs (propertyValue, oldValue, newValue);
      DomainObject.OnPropertyChanging (args);
    }

    public void EndPropertyValueChange (PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      var args = new PropertyChangeEventArgs (propertyValue, oldValue, newValue);
      DomainObject.OnPropertyChanged (args);
    }
 
    public void BeginRelationChange (string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
      DomainObject.OnRelationChanging (args);
    }

    public void EndRelationChange (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      DomainObject.OnRelationChanged (new RelationChangedEventArgs (propertyName));
    }

    public void BeginDelete ()
    {
      DomainObject.OnDeleting (EventArgs.Empty);
    }

    public void EndDelete ()
    {
      DomainObject.OnDeleted (EventArgs.Empty);
    }

    public void BeginCommit ()
    {
      DomainObject.OnCommitting (EventArgs.Empty);
    }

    public void EndCommit ()
    {
      DomainObject.OnCommitted (EventArgs.Empty);
    }

    public void BeginRollback ()
    {
      DomainObject.OnRollingBack (EventArgs.Empty);
    }

    public void EndRollback ()
    {
      DomainObject.OnRolledBack (EventArgs.Empty);
    }
  }
}