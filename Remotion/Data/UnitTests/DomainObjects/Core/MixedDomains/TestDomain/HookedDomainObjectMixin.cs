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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  [NonIntroduced (typeof (IDomainObjectMixin))]
  public class HookedDomainObjectMixin : Mixin<Order>, IDomainObjectMixin
  {
    public event EventHandler InitializationHandler;

    public bool OnLoadedCalled;
    public int OnLoadedCount;
    public LoadMode OnLoadedLoadMode;
    public bool OnCreatedCalled;
    
    public bool OnDomainObjectReferenceInitializedCalled;
    public int OnDomainObjectReferenceInitializedCount;

    public void OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnLoadedCalled = true;
      OnLoadedLoadMode = loadMode;
      ++OnLoadedCount;
      Assert.IsNotNull (This.ID);
      ++This.OrderNumber;
      Assert.IsNotNull (This.OrderItems);
    }

    public void OnDomainObjectCreated ()
    {
      OnCreatedCalled = true;
      Assert.IsNotNull (This.ID);
      This.OrderNumber += 2;
      Assert.IsNotNull (This.OrderItems);
    }

    public void OnDomainObjectReferenceInitialized ()
    {
      OnDomainObjectReferenceInitializedCalled = true;
      ++OnDomainObjectReferenceInitializedCount;
      Assert.IsNotNull (This.ID);
      if (InitializationHandler != null)
        InitializationHandler (this, EventArgs.Empty);
    }

    public new Order This 
    { 
      get { return base.This; } 
    }
  }
}
