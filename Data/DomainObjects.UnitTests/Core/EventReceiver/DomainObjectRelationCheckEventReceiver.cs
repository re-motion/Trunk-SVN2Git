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
using System.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver
{
  public class DomainObjectRelationCheckEventReceiver : DomainObjectEventReceiver
  {
    // types

    // static members and constants

    // member fields
    private Hashtable changingRelatedObjects;
    private Hashtable changedRelatedObjects;
    private StateType changingObjectState;
    private StateType changedObjectState;

    // construction and disposing

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject)
      : this (domainObject, false)
    {
    }

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject, bool cancel)
      : base (domainObject, cancel)
    {
      changingRelatedObjects = new Hashtable ();
      changedRelatedObjects = new Hashtable ();
    }

    // methods and properties

    public DomainObject GetChangingRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changingRelatedObjects[propertyName];
    }

    public DomainObject GetChangedRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changedRelatedObjects[propertyName];
    }

    protected override void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changingObjectState = domainObject.State;

      string changedProperty = args.PropertyName;

			if (CardinalityType.One == domainObject.InternalDataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanging (sender, args);
    }

    protected override void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changedObjectState = domainObject.State;

      string changedProperty = args.PropertyName;

			if (CardinalityType.One == domainObject.InternalDataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanged (sender, args);
    }
  }
}
