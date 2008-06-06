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
      IInitializableMixinTarget instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin)).With();
      object baseCallProxy = instance.CreateBaseCallProxy (0);
      Assert.IsNotNull (baseCallProxy);
      Assert.That (baseCallProxy, Is.InstanceOfType (MixinReflector.GetBaseCallProxyType (instance)));
    }

    [Test]
    public void CreateBaseCallProxy_SetsDepthCorrectly ()
    {
      IInitializableMixinTarget instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin)).With ();
      object baseCallProxy = instance.CreateBaseCallProxy (0);
      Assert.That (PrivateInvoke.GetPublicField (baseCallProxy, "__depth"), Is.EqualTo (0));

      baseCallProxy = instance.CreateBaseCallProxy (3);
      Assert.That (PrivateInvoke.GetPublicField (baseCallProxy, "__depth"), Is.EqualTo (3));
    }

    [Test]
    public void SetFirstBaseCallProxy ()
    {
      IInitializableMixinTarget instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin)).With ();
      object baseCallProxy = instance.CreateBaseCallProxy (0);

      Assert.That (instance.FirstBaseCallProxy, Is.Not.SameAs (baseCallProxy));
      instance.SetFirstBaseCallProxy (baseCallProxy);
      Assert.That (instance.FirstBaseCallProxy, Is.SameAs (baseCallProxy));
    }

    [Test]
    public void SetExtensions ()
    {
      IInitializableMixinTarget instance = (IInitializableMixinTarget) CreateMixedObject<BaseType1> (typeof (NullMixin)).With ();
      object[] extensions = new object[1];

      Assert.That (instance.Mixins, Is.Not.SameAs (extensions));
      instance.SetExtensions (extensions);
      Assert.That (instance.Mixins, Is.SameAs (extensions));
    }
  }
}
