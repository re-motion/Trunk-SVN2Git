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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    public class ClassOverridingMixinMethod
    {
      [OverrideMixin]
      public new string ToString ()
      {
        return "Overridden!";
      }
    }

    public class SimpleMixin : Mixin<object>
    {
    }

    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (Mixin<object>));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMethod> ().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod)).With ();
        Assert.AreEqual ("Overridden!", Mixin.Get (generatedType, instance).ToString ());
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      typeEmitter.CreateMethod ("ToString", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturning (new ConstReference ("Generated _and_ overridden").ToExpression ())
          .AddCustomAttribute (new CustomAttributeBuilder (typeof (OverrideMixinAttribute).GetConstructor (Type.EmptyTypes), new object[0]));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (SimpleMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType).With ();
        Assert.AreEqual ("Generated _and_ overridden", Mixin.Get<SimpleMixin> (instance).ToString ());
      }
    }
  }
}
