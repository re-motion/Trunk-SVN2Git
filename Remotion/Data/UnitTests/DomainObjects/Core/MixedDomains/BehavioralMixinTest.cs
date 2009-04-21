// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;

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
        DOWithVirtualPropertiesAndMethods instance = (DOWithVirtualPropertiesAndMethods) RepositoryAccessor.NewObject (typeof (DOWithVirtualPropertiesAndMethods), ParamList.Empty);
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
        return NewObject<NestedDomainObject> ();
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
