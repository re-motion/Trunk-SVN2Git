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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (ObjectFactory.Create<SimpleBusinessObjectClass>().With(), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectMixin mixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>().With());
      IBusinessObject businessObject = mixin;

      Assert.That (
          businessObject.DisplayName,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
    }

    [Test]
    public void GetProvider ()
    {
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)),
          Is.SameAs (BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>()));
      Assert.That (
          BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass)),
          Is.Not.SameAs (BusinessObjectProvider.GetProvider<BindableObjectWithIdentityProviderAttribute> ()));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass>().With();
      value.String = "TheString";
      SimpleBusinessObjectClass deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass> ().With ();
      byte[] serialized = Serializer.Serialize (value);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      SimpleBusinessObjectClass deserialized = (SimpleBusinessObjectClass) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }
  }
}
