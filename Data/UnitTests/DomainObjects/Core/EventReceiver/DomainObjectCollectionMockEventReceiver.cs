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
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public abstract class DomainObjectCollectionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectCollectionMockEventReceiver (DomainObjectCollection domainObjectCollection)
    {
      ArgumentUtility.CheckNotNull ("domainObjectCollection", domainObjectCollection);

      domainObjectCollection.Added += new DomainObjectCollectionChangeEventHandler (Added);
      domainObjectCollection.Adding += new DomainObjectCollectionChangeEventHandler (Adding);
      domainObjectCollection.Removed += new DomainObjectCollectionChangeEventHandler (Removed);
      domainObjectCollection.Removing += new DomainObjectCollectionChangeEventHandler (Removing);
    }

    // abstract methods and properties

    protected abstract void Added (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Adding (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removed (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removing (object sender, DomainObjectCollectionChangeEventArgs args);

    public void Adding (object sender, DomainObject domainObject)
    {
      Adding (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Added (object sender, DomainObject domainObject)
    {
      Added (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removing (object sender, DomainObject domainObject)
    {
      Removing (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removed (object sender, DomainObject domainObject)
    {
      Removed (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }
  }
}
