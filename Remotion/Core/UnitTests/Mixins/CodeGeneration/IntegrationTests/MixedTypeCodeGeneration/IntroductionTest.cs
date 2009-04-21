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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class IntroductionTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntroducedInterfacesAreImplementedViaDelegation ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var bt1AsMixedIface = bt1 as IBT1Mixin1;
      Assert.IsNotNull (bt1AsMixedIface);
      Assert.AreEqual ("BT1Mixin1.IntroducedMethod", bt1AsMixedIface.IntroducedMethod ());
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithExplicitImplementation));
      var explicito = bt1 as IExplicit;
      Assert.IsNotNull (explicito);
      Assert.AreEqual ("XXX", explicito.Explicit ());
    }

    [Test]
#if NET35SP1
    [Ignore ("TODO: Due to a bug in .net 3.5 SP1 this test will cause the ExecutionEngine to crash.")]
#endif
    public void MixinCanIntroduceGenericInterface ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>));
      var generic = bt1 as IGeneric<BaseType1>;
      Assert.IsNotNull (generic);
      Assert.AreEqual ("Generic", generic.Generic (bt1));
    }

    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingInheritedInterface));
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII1) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII2) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII2) bt1).Method2 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method3", ((IMixinIII3) bt1).Method3 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method4", ((IMixinIII4) bt1).Method4 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII4) bt1).Method2 ());
    }

    [Test]
    public void MixinImplementingFullPropertiesWithPartialIntroduction ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingFullPropertiesWithPartialIntroduction)).EnterScope())
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
        MethodInfo[] allMethods = bt1.GetType ().GetMethods (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        string[] allMethodNames = Array.ConvertAll (allMethods, mi => mi.Name);
        Assert.That (allMethodNames, List.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.get_Prop1"));
        Assert.That (allMethodNames, List.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.set_Prop2"));

        Assert.That (allMethodNames, List.Not.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.set_Prop1"));
        Assert.That (allMethodNames, List.Not.Contains ("Remotion.UnitTests.Mixins.SampleTypes.InterfaceWithPartialProperties.get_Prop2"));
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedInterface ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (MixinNonIntroducingSimpleInterface));
      Assert.IsFalse (o is ISimpleInterface);
      Assert.That (Mixin.Get<MixinNonIntroducingSimpleInterface> (o), Is.InstanceOfType (typeof (ISimpleInterface)));
    }

    [Test]
    public void ImplicitlyNonIntroducedInterface ()
    {
      ClassImplementingSimpleInterface o = CreateMixedObject<ClassImplementingSimpleInterface> (typeof (MixinImplementingSimpleInterface));
      Assert.That (o, Is.InstanceOfType (typeof (ISimpleInterface)));
      Assert.AreEqual ("ClassImplementingSimpleInterface.Method", o.Method ());
    }

    [Test]
    public void MultipleSimilarInterfaces ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (List<>));
      Assert.IsTrue (o is IList<NullTarget>);
      Assert.IsTrue (o is ICollection<NullTarget>);
      Assert.IsTrue (o is IEnumerable<NullTarget>);
      Assert.IsTrue (o is IList);
      Assert.IsTrue (o is ICollection);
      Assert.IsTrue (o is IEnumerable);
    }

    [Test]
    public void IntroducedMemberVisibilites_Public ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodInfo methodInfo = t.GetMethod ("MethodWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      PropertyInfo propertyInfo = t.GetProperty ("PropertyWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      EventInfo eventInfo = t.GetEvent ("EventWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);

      Assert.That (methodInfo, Is.Not.Null);
      Assert.That (propertyInfo, Is.Not.Null);
      Assert.That (eventInfo, Is.Not.Null);
    }

    [Test]
    public void IntroducedMemberVisibilites_Private ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodInfo methodInfo = t.GetMethod (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".MethodWithDefaultVisibility", BindingFlags.NonPublic | BindingFlags.Instance);
      PropertyInfo propertyInfo = t.GetProperty (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".PropertyWithDefaultVisibility",
          BindingFlags.NonPublic | BindingFlags.Instance);
      EventInfo eventInfo = t.GetEvent (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".EventWithDefaultVisibility", BindingFlags.NonPublic | BindingFlags.Instance);

      Assert.That (methodInfo, Is.Not.Null);
      Assert.That (propertyInfo, Is.Not.Null);
      Assert.That (eventInfo, Is.Not.Null);

      Assert.That (methodInfo.IsPrivate);
      Assert.That (propertyInfo.GetGetMethod (true).IsPrivate);
      Assert.That (propertyInfo.GetSetMethod (true).IsPrivate);
      Assert.That (eventInfo.GetAddMethod (true).IsPrivate);
      Assert.That (eventInfo.GetRemoveMethod (true).IsPrivate);
    }
  }
}
