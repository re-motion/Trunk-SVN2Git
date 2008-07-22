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
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public abstract class DomainObjectMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectMockEventReceiver (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      domainObject.Committed += new EventHandler (Committed);
      domainObject.Committing += new EventHandler (Committing);
      domainObject.RolledBack += new EventHandler (RolledBack);
      domainObject.RollingBack += new EventHandler (RollingBack);
      domainObject.Deleted += new EventHandler (Deleted);
      domainObject.Deleting += new EventHandler (Deleting);
      domainObject.PropertyChanged += new PropertyChangeEventHandler (PropertyChanged);
      domainObject.PropertyChanging += new PropertyChangeEventHandler (PropertyChanging);
      domainObject.RelationChanged += new RelationChangedEventHandler (RelationChanged);
      domainObject.RelationChanging += new RelationChangingEventHandler (RelationChanging);
    }

    // abstract methods and properties

    public abstract void RelationChanging (object sender, RelationChangingEventArgs args);
    public abstract void RelationChanged (object sender, RelationChangedEventArgs args);
    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void Deleting (object sender, EventArgs e);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void Deleted (object sender, EventArgs e);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void Committing (object sender, EventArgs e);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void Committed (object sender, EventArgs e);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void RollingBack (object sender, EventArgs e);
    //TODO ES: change all calls to this method and remove LastCall if possible
    public abstract void RolledBack (object sender, EventArgs e);

    // methods and properties

    public void RelationChanging (object sender, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      RelationChanging (null, null);

      LastCall.Constraints (
          Mocks_Is.Same (sender),
          Mocks_Property.Value ("PropertyName", propertyName)
          & Mocks_Property.Value ("OldRelatedObject", oldRelatedObject)
          & Mocks_Property.Value ("NewRelatedObject", newRelatedObject));
    }

    public void RelationChanged (object sender, string propertyName)
    {
      RelationChanged (null, (RelationChangedEventArgs) null);

      LastCall.Constraints (
          Mocks_Is.Same (sender),
          Mocks_Property.Value ("PropertyName", propertyName));
    }
  }
}
