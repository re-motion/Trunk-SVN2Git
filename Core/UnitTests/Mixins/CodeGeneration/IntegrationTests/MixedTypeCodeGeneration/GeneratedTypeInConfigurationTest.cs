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
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWorks()
    {
      var typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
                                                "GeneratedTypeInConfigurationTest.GeneratedMixinTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (NullTarget)).With();
        Assert.IsNotNull (Mixin.Get (generatedType, instance));
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks()
    {
      var typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
                                                "GeneratedTypeInConfigurationTest.GeneratedTargetTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType).With();
        Assert.IsNotNull (Mixin.Get (typeof (NullMixin), instance));
      }
    }
  }
}