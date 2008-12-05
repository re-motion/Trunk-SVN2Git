// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
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
