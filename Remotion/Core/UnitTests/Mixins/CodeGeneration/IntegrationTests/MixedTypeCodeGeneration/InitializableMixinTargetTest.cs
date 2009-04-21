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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InitializableMixinTargetTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsIInitializableMixinTarget ()
    {
      Type concreteType = CreateMixedType (typeof (BaseType1), typeof (NullMixin));
      Assert.That (typeof (IInitializableMixinTarget).IsAssignableFrom (concreteType));
    }

    [Test]
    public void CreateBaseCallProxy_InstantiatesCorrectType ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));
      object baseCallProxy = instance.CreateBaseCallProxy (0);
      Assert.IsNotNull (baseCallProxy);
      Assert.That (baseCallProxy, Is.InstanceOfType (MixinReflector.GetBaseCallProxyType (instance)));
    }

    [Test]
    public void CreateBaseCallProxy_SetsDepthCorrectly ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));
      object baseCallProxy = instance.CreateBaseCallProxy (0);
      Assert.That (PrivateInvoke.GetPublicField (baseCallProxy, "__depth"), Is.EqualTo (0));

      baseCallProxy = instance.CreateBaseCallProxy (3);
      Assert.That (PrivateInvoke.GetPublicField (baseCallProxy, "__depth"), Is.EqualTo (3));
    }

    [Test]
    public void SetFirstBaseCallProxy ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));
      object baseCallProxy = instance.CreateBaseCallProxy (0);

      Assert.That (instance.FirstBaseCallProxy, Is.Not.SameAs (baseCallProxy));
      instance.SetFirstBaseCallProxy (baseCallProxy);
      Assert.That (instance.FirstBaseCallProxy, Is.SameAs (baseCallProxy));
    }

    [Test]
    public void SetExtensions ()
    {
      var instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin));
      var extensions = new object[1];

      Assert.That (instance.Mixins, Is.Not.SameAs (extensions));
      instance.SetExtensions (extensions);
      Assert.That (instance.Mixins, Is.SameAs (extensions));
    }
  }
}
