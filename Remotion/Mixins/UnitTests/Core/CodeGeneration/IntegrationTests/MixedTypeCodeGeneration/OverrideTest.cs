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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideClassMethods ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1));

      Assert.AreEqual ("BT1Mixin1.VirtualMethod", bt1.VirtualMethod ());
      Assert.IsNotNull (bt1.GetType ().GetMethod ("VirtualMethod", Type.EmptyTypes), "overridden member is public and has the same name");
      Assert.AreEqual (typeof (BaseType1), bt1.GetType ().GetMethod ("VirtualMethod", Type.EmptyTypes).GetBaseDefinition ().DeclaringType);
    }

    [Test]
    [Ignore ("TODO: This does not work on the build server, check why.")]
    public void OverrideClassProperties ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1));

      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreNotEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      bt1.VirtualProperty = "FooBar";
      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      Assert.IsNotNull (bt1.GetType ().GetProperty ("VirtualProperty"), "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin2));

      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
      bt1.VirtualProperty = "Foobar";
      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
    }

    [Test]
    public void OverrideClassPropertiesTemp ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1));

      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreNotEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      bt1.VirtualProperty = "FooBar";
      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      Assert.IsNotNull (bt1.GetType ().GetProperty ("VirtualProperty",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin2));

      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
      bt1.VirtualProperty = "Foobar";
      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
    }

    [Test]
    public void OverrideClassEvents ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1));

      EventHandler eventHandler = delegate { };

      Assert.IsFalse (Mixin.Get<BT1Mixin1> (bt1).VirtualEventAddCalled);
      bt1.VirtualEvent += eventHandler;
      Assert.IsTrue (Mixin.Get<BT1Mixin1> (bt1).VirtualEventAddCalled);

      Assert.IsFalse (Mixin.Get<BT1Mixin1> (bt1).VirtualEventRemoveCalled);
      bt1.VirtualEvent -= eventHandler;
      Assert.IsTrue (Mixin.Get<BT1Mixin1> (bt1).VirtualEventRemoveCalled);

      Assert.IsNotNull (bt1.GetType ().GetEvent ("VirtualEvent"), "overridden member is public and has the same name");
    }

    class Foo1
    { }

    class Foo2
    { }

    [CompleteInterface (typeof (Foo1))]
    [CompleteInterface (typeof (Foo2))]
    [IgnoreForMixinConfiguration]
    interface IMultiFace
    {
    }

    [Test]
    public void OverrideWithCompleteBaseInterface ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin7Base), typeof (BT3Mixin4));
      Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod ());
    }

    [Test]
    public void MixinOverridingInheritedClassMethod ()
    {
      ClassWithInheritedMethod cwim = ObjectFactory.Create<ClassWithInheritedMethod> (ParamList.Empty);
      Assert.AreEqual ("MixinOverridingInheritedMethod.ProtectedInheritedMethod-BaseClassWithInheritedMethod.ProtectedInheritedMethod-"
          + "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod-"
          + "MixinOverridingInheritedMethod.PublicInheritedMethod-BaseClassWithInheritedMethod.PublicInheritedMethod",
          cwim.InvokeInheritedMethods ());
    }

    [Test]
    public void MixinWithProtectedOverrider ()
    {
      BaseType1 obj = CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider));
      Assert.AreEqual ("MixinWithProtectedOverrider.VirtualMethod-BaseType1.VirtualMethod", obj.VirtualMethod ());
      Assert.AreEqual ("MixinWithProtectedOverrider.VirtualProperty-BaseType1.BackingField", obj.VirtualProperty);

      Assert.AreEqual (null, obj.GetVirtualEventInvocationList());
      obj.VirtualEvent += delegate { };
      Assert.AreEqual (2, obj.GetVirtualEventInvocationList ().Length);
    }

    [Test]
    public void ValueTypeMixin ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof (ValueTypeMixin));
      Assert.AreEqual ("ValueTypeMixin.VirtualMethod", bt1.VirtualMethod());
    }

    [Test]
    public void AlphabeticOrdering ()
    {
      ClassWithMixinsAcceptingAlphabeticOrdering instance = ObjectFactory.Create<ClassWithMixinsAcceptingAlphabeticOrdering> (ParamList.Empty);
      Assert.AreEqual (
          "MixinAcceptingAlphabeticOrdering1.ToString-MixinAcceptingAlphabeticOrdering2.ToString-ClassWithMixinsAcceptingAlphabeticOrdering.ToString",
          instance.ToString ());
    }
  }
}
