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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.BaseCallProxyCodeGeneration
{
  [TestFixture]
  public class BaseCallTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces1 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (MixinWithThisAsBase)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        foreach (RequiredBaseCallTypeDefinition req in TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
      }
    }

    [Test]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces2 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        RequiredBaseCallTypeDefinition bt3Mixin4Req =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.IsNotNull (bt3Mixin4Req);
        Assert.IsTrue (bt3Mixin4Req.Type.IsAssignableFrom (proxyType));

        foreach (RequiredBaseCallTypeDefinition req in TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));

        MethodInfo methodImplementdByMixin =
            proxyType.GetMethod ("Remotion.UnitTests.Mixins.SampleTypes.IBT3Mixin4.Foo", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByMixin);

        MethodInfo methodImplementdByBCOverridden =
            proxyType.GetMethod ("Remotion.UnitTests.Mixins.SampleTypes.IBaseType31.IfcMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCOverridden);

        MethodInfo methodImplementdByBCNotOverridden =
            proxyType.GetMethod ("Remotion.UnitTests.Mixins.SampleTypes.IBaseType35.IfcMethod2", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCNotOverridden);
      }
    }

    [Test]
    public void BaseCallMethodToThis ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (MixinWithThisAsBase)).With ();
      Assert.AreEqual ("MixinWithThisAsBase.IfcMethod-BaseType3.IfcMethod", bt3.IfcMethod ());
    }

    [Test]
    public void BaseCallMethodToDuckInterface ()
    {
      BaseTypeWithDuckBaseMixin duckBase = ObjectFactory.Create<BaseTypeWithDuckBaseMixin> ().With ();
      Assert.AreEqual ("DuckBaseMixin.MethodImplementedOnBase-BaseTypeWithDuckBaseMixin.MethodImplementedOnBase-"
                       + "DuckBaseMixin.ProtectedMethodImplementedOnBase-BaseTypeWithDuckBaseMixin.ProtectedMethodImplementedOnBase",
          duckBase.MethodImplementedOnBase ());
    }

    [Test]
    public void BaseCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> ().With ();
      MixinWithIndirectRequirements mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.AreEqual ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
                       + "ClassImplementingIndirectRequirements.Method3", mixin.GetStuffViaBase ());
    }

    [Test]
    public void OverriddenMemberCalls ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod ());
      }
    }

    [Test]
    public void BaseCallToString()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingToString>().Clear().AddMixins(typeof(MixinOverridingToString)).EnterScope())
      {
        object instance = ObjectFactory.Create<ClassOverridingToString>().With();
        Assert.AreEqual("Overridden: ClassOverridingToString", instance.ToString());
      }
    }
  }
}
