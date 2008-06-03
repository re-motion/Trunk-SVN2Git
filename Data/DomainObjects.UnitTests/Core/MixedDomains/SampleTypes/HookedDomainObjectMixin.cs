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
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [NonIntroduced (typeof (IDomainObjectMixin))]
  public class HookedDomainObjectMixin : Mixin<Order>, IDomainObjectMixin
  {
    public bool OnLoadedCalled = false;
    public int OnLoadedCount = 0;
    public LoadMode OnLoadedLoadMode;
    public bool OnCreatedCalled = false;

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
  }
}
