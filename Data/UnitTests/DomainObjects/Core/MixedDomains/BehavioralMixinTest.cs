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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class BehavioralMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void LoadedDomainObjectsCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsNotNull (Mixin.Get<NullMixin> (order));
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsTrue (order is IInterfaceAddedByMixin);
        Assert.AreEqual ("Hello, my ID is " + DomainObjectIDs.Order1, ((IInterfaceAddedByMixin) order).GetGreetings ());
      }
    }

    [Test]
    public void MixinCanOverrideVirtualPropertiesAndMethods ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (DOWithVirtualPropertiesAndMethods)).Clear().AddMixins (typeof (MixinOverridingPropertiesAndMethods)).EnterScope())
      {
        DOWithVirtualPropertiesAndMethods instance = (DOWithVirtualPropertiesAndMethods) RepositoryAccessor.NewObject (typeof (DOWithVirtualPropertiesAndMethods)).With();
        instance.Property = "Text";
        Assert.AreEqual ("Text-MixinSetter-MixinGetter", instance.Property);
        Assert.AreEqual ("Something-MixinMethod", instance.GetSomething ());
      }
    }

    [DBTable]
    [TestDomain]
    [Uses (typeof (NullMixin))]
    public class NestedDomainObject : DomainObject
    {
      public static NestedDomainObject NewObject ()
      {
        return NewObject<NestedDomainObject> ().With ();
      }
    }

    [Test]
    public void NestedDomainObjectDomainObjectsCanBeMixed ()
    {
      DomainObject domainObject = NestedDomainObject.NewObject ();
      Assert.IsNotNull (Mixin.Get<NullMixin> (domainObject));
    }
  }
}
