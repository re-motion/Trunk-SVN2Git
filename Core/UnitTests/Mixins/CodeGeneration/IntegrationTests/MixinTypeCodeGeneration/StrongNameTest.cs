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
using System.Collections;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Samples;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTest : CodeGenerationBaseTest
  {
    [Test]
    public void SignedMixinWithSignedTargetClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ArrayList> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        MixinDefinition mixinDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ArrayList)).GetMixinByConfiguredType (typeof (EquatableMixin<>));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void UnsignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        MixinDefinition mixinDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).GetMixinByConfiguredType (typeof (UnsignedMixin));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void SignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<UnsignedClass> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        MixinDefinition mixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (UnsignedClass)).GetMixinByConfiguredType (typeof (EquatableMixin<>));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));

        Assert.AreEqual ("Overridden", Mixin.Get<EquatableMixin<UnsignedClass>> (ObjectFactory.Create<UnsignedClass>().With()).ToString());
      }
    }

    [Test]
    public void UnsignedMixinWithSignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        MixinDefinition mixinDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (UnsignedMixin)];
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));

        Assert.AreEqual ("Overridden", ObjectFactory.Create<NullTarget>().With().ToString());
      }
    }
  }

  public class UnsignedMixin : Mixin<object>
  {
    [OverrideTarget]
    protected new string ToString ()
    {
      return "Overridden";
    }
  }

  public class UnsignedClass
  {
    [OverrideMixin]
    public new string ToString ()
    {
      return "Overridden";
    }
  }
}
