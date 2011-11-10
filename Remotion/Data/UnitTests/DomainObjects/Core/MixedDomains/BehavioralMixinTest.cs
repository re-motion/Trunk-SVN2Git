// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DomainImplementation;
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
    public void DomainObjectsCanBeMixed ()
    {
      var domainObject = TargetClassForBehavioralMixin.NewObject ();
      Assert.IsNotNull (Mixin.Get<NullMixin> (domainObject));
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      var domainObject = TargetClassForBehavioralMixin.NewObject ();
      Assert.IsTrue (domainObject is IInterfaceAddedByMixin);
      Assert.AreEqual ("Hello, my ID is " + domainObject.ID, ((IInterfaceAddedByMixin) domainObject).GetGreetings ());
    }

    [Test]
    public void MixinCanOverrideVirtualPropertiesAndMethods ()
    {
      var instance = TargetClassForBehavioralMixin.NewObject();
      instance.Property = "Text";
      Assert.AreEqual ("Text-MixinSetter-MixinGetter", instance.Property);
      Assert.AreEqual ("Something-MixinMethod", instance.GetSomething ());
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
    public void NestedDomainObjects_CanBeMixed ()
    {
      DomainObject domainObject = NestedDomainObject.NewObject ();
      Assert.IsNotNull (Mixin.Get<NullMixin> (domainObject));
    }
  }
}
