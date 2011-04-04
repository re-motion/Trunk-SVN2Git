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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityMixinTest : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (
          ObjectFactory.Create<ClassWithIdentity>(ParamList.Create ("TheUniqueIdentifier")),
          Is.InstanceOf (typeof (IBusinessObjectWithIdentity)));
    }

    [Test]
    public void GetUniqueIdentifier ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin> (ObjectFactory.Create<ClassWithIdentity>(ParamList.Create ("TheUniqueIdentifier")));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That (businessObjectWithIdentity.UniqueIdentifier, Is.SameAs ("TheUniqueIdentifier"));
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin> (ObjectFactory.Create<ClassWithIdentityAndDisplayName> (ParamList.Create ("TheUniqueIdentifier")));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That (businessObjectWithIdentity.DisplayName, Is.SameAs ("TheUniqueIdentifier"));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute> ()));
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute> ()));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> (ParamList.Empty);
      value.String = "TheString";
      ClassWithIdentity deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> (ParamList.Empty);
      byte[] serialized = Serializer.Serialize (value);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
      ClassWithIdentity deserialized = (ClassWithIdentity) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void HasMixin ()
    {
      Assert.IsTrue (Mixins.MixinTypeUtility.HasMixin (typeof (ClassWithIdentity), typeof (BindableObjectWithIdentityMixin)));
      Assert.IsFalse (Mixins.MixinTypeUtility.HasMixin (typeof (ClassWithAllDataTypes), typeof (BindableObjectWithIdentityMixin)));
      Assert.IsFalse (Mixins.MixinTypeUtility.HasMixin (typeof (object), typeof (BindableObjectWithIdentityMixin)));
    }
  }
}
