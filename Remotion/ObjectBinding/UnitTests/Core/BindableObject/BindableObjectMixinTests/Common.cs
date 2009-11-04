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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty), Is.InstanceOfType (typeof (IBusinessObject)));
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectMixin mixin = Mixin.Get<BindableObjectMixin> (ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty));
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
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass> (ParamList.Empty);
      value.String = "TheString";
      SimpleBusinessObjectClass deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      SimpleBusinessObjectClass value = ObjectFactory.Create<SimpleBusinessObjectClass> (ParamList.Empty);
      byte[] serialized = Serializer.Serialize (value);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      SimpleBusinessObjectClass deserialized = (SimpleBusinessObjectClass) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }
  }
}
