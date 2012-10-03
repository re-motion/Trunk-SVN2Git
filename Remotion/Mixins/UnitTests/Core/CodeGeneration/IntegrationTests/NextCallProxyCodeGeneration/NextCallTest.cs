// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.NextCallProxyCodeGeneration.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.NextCallProxyCodeGeneration
{
  [TestFixture]
  public class NextCallTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsRequiredNextCallInterfaces1 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (MixinWithThisAsBase)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("NextCallProxy");

        foreach (RequiredNextCallTypeDefinition req in DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).RequiredNextCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
      }
    }

    [Test]
    public void GeneratedTypeImplementsRequiredNextCallInterfaces2 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("NextCallProxy");

        RequiredNextCallTypeDefinition bt3Mixin4Req =
            DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).RequiredNextCallTypes[typeof (IBT3Mixin4)];
        Assert.IsNotNull (bt3Mixin4Req);
        Assert.IsTrue (bt3Mixin4Req.Type.IsAssignableFrom (proxyType));

        foreach (RequiredNextCallTypeDefinition req in DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).RequiredNextCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));

        MethodInfo methodImplementdByMixin =
            proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.IBT3Mixin4.Foo", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByMixin);

        MethodInfo methodImplementdByBCOverridden =
            proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.IBaseType31.IfcMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCOverridden);

        MethodInfo methodImplementdByBCNotOverridden =
            proxyType.GetMethod ("Remotion.Mixins.UnitTests.Core.TestDomain.IBaseType35.IfcMethod2", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCNotOverridden);
      }
    }

    [Test]
    public void NextCallMethodToThis ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (MixinWithThisAsBase));
      Assert.AreEqual ("MixinWithThisAsBase.IfcMethod-BaseType3.IfcMethod", bt3.IfcMethod ());
    }

    [Test]
    public void NextCallMethodToDuckInterface ()
    {
      BaseTypeWithDuckBaseMixin duckBase = ObjectFactory.Create<BaseTypeWithDuckBaseMixin> (ParamList.Empty);
      Assert.AreEqual ("DuckBaseMixin.MethodImplementedOnBase-BaseTypeWithDuckBaseMixin.MethodImplementedOnBase-"
                       + "DuckBaseMixin.ProtectedMethodImplementedOnBase-BaseTypeWithDuckBaseMixin.ProtectedMethodImplementedOnBase",
          duckBase.MethodImplementedOnBase ());
    }

    [Test]
    public void NextCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> (ParamList.Empty);
      MixinWithIndirectRequirements mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.AreEqual ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
                       + "ClassImplementingIndirectRequirements.Method3", mixin.GetStuffViaBase ());
    }

    [Test]
    public void OverriddenMemberCalls ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod ());
      }
    }

    [Test]
    public void NextCallToString()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingToString>().Clear().AddMixins(typeof(MixinOverridingToString)).EnterScope())
      {
        object instance = ObjectFactory.Create<ClassOverridingToString>(ParamList.Empty);
        Assert.AreEqual("Overridden: ClassOverridingToString", instance.ToString());
      }
    }

    [Test]
    public void NextCall_ToMethodWithArgument ()
    {
      var instance = ObjectFactory.Create<TargetClassForMixinOverridingMethodWithArgument>();
      var result = instance.VirtualMethod ("Test");

      Assert.That (
          result,
          Is.EqualTo ("MixinOverridingMethodWithArgument.VirtualMethod(Test) - TargetClassForMixinOverridingMethodWithArgument.VirtualMethod(Test)"));
    }

    [Test]
    public void NextCall_ToMethodWithArgument_AlsoDeclaredOnObject ()
    {
      var instance = ObjectFactory.Create<TargetClassForMixinOverridingMethodWithArgument> ();
// ReSharper disable EqualExpressionComparison
      var result1 = instance.Equals (instance);
// ReSharper restore EqualExpressionComparison
      var result2 = instance.Equals (null);

      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }
  }
}
