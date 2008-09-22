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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InstantiationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCall ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) Activator.CreateInstance (generatedType);
      Assert.IsNotNull (bt3);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
      Assert.AreSame (bt3, Mixin.Get<BT3Mixin1> (bt3).This);
    }

    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCallEvenWhenDerived ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));

      AssemblyBuilder builder =
          AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName ("Foo"), AssemblyBuilderAccess.Run);
      TypeBuilder typeBuilder = builder.DefineDynamicModule ("Foo.dll").DefineType ("Derived", TypeAttributes.Public, generatedType);
      ConstructorBuilder ctor = typeBuilder.DefineConstructor (MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);
      ILGenerator ilgen = ctor.GetILGenerator ();
      ilgen.Emit (OpCodes.Ldarg_0);
      ilgen.Emit (OpCodes.Callvirt, generatedType.GetConstructor (Type.EmptyTypes));
      ilgen.Emit (OpCodes.Ret);

      Type evenDerivedType = typeBuilder.CreateType ();

      BaseType3 bt3 = (BaseType3) Activator.CreateInstance (evenDerivedType);
      Assert.AreSame (generatedType, bt3.GetType ().BaseType);
      Assert.IsNotNull (bt3);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
      Assert.AreSame (bt3, Mixin.Get<BT3Mixin1> (bt3).This);
    }

    [Test]
    public void CtorsRespectMixedTypeInstantiationScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BT3Mixin1 suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedObjectInstantiationScope (suppliedMixinInstance))
      {
        BaseType3 bt3 = (BaseType3) Activator.CreateInstance (generatedType);
        Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
        Assert.AreSame (suppliedMixinInstance, Mixin.Get<BT3Mixin1> (bt3));
        Assert.AreSame (bt3, suppliedMixinInstance.This);
        Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The supplied mixin of type Remotion.UnitTests.Mixins.SampleTypes.BT3Mixin1 is not valid in the current configuration.",
        MatchType = MessageMatch.Contains)]
    public void ThrowsIfWrongMixinInstancesInScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      BT3Mixin1 suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedObjectInstantiationScope (suppliedMixinInstance))
      {
        try
        {
          Activator.CreateInstance (generatedType);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    [Test]
    public void OverriddenMethodCalledFromCtor ()
    {
      TargetClassCallingOverriddenMethodFromCtor instance =
          CreateMixedObject<TargetClassCallingOverriddenMethodFromCtor> (typeof (MixinOverridingMethodCalledFromCtor)).With ();
      MixinOverridingMethodCalledFromCtor mixin = Mixin.Get<MixinOverridingMethodCalledFromCtor> (instance);
      Assert.That (instance.Result, Is.SameAs (mixin));
      Assert.That (mixin.MyThis, Is.SameAs (instance));
      Assert.That (mixin.MyBase, Is.Not.Null);
    }

    [Test]
    public void IntroducedMethodCalledFromCtor ()
    {
      TargetClassCallingIntroducedMethodFromCtor instance =
          CreateMixedObject<TargetClassCallingIntroducedMethodFromCtor> (typeof (MixinIntroducingMethodCalledFromCtor)).With ();
      
      MixinIntroducingMethodCalledFromCtor mixin = Mixin.Get<MixinIntroducingMethodCalledFromCtor> (instance);
      Assert.That (instance.Result, Is.SameAs (mixin));
      Assert.That (mixin.MyThis, Is.SameAs (instance));
      Assert.That (mixin.MyBase, Is.Not.Null);
    }
  }
}
