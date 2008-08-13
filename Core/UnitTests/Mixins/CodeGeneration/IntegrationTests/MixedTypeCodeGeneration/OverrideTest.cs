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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideClassMethods ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1)).With ();

      Assert.AreEqual ("BT1Mixin1.VirtualMethod", bt1.VirtualMethod ());
      Assert.IsNotNull (bt1.GetType ().GetMethod ("VirtualMethod", Type.EmptyTypes), "overridden member is public and has the same name");
      Assert.AreEqual (typeof (BaseType1), bt1.GetType ().GetMethod ("VirtualMethod", Type.EmptyTypes).GetBaseDefinition ().DeclaringType);
    }

    [Test]
    public void OverrideClassProperties ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1)).With ();

      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreNotEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      bt1.VirtualProperty = "FooBar";
      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      Assert.IsNotNull (bt1.GetType ().GetProperty ("VirtualProperty"), "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin2)).With ();

      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
      bt1.VirtualProperty = "Foobar";
      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
    }

    [Test]
    public void OverrideClassPropertiesTemp ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1)).With ();

      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreNotEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      bt1.VirtualProperty = "FooBar";
      Assert.AreEqual ("BaseType1.BackingField", bt1.VirtualProperty);
      Assert.AreEqual ("FooBar", Mixin.Get<BT1Mixin1> (bt1).BackingField);

      Assert.IsNotNull (bt1.GetType ().GetProperty ("VirtualProperty",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), "overridden member is public and has the same name");

      bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin2)).With ();

      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
      bt1.VirtualProperty = "Foobar";
      Assert.AreEqual ("Mixin2ForBT1.VirtualProperty", bt1.VirtualProperty);
    }

    [Test]
    public void OverrideClassEvents ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (BT1Mixin1)).With ();

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
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).With ();
      Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod ());
    }

    [Test]
    public void TestMultipleOverridesSmall ()
    {
      BaseType7 bt7 = ObjectFactory.Create<BaseType7> ().With ();
      Assert.AreEqual ("BT7Mixin0.One(5)-BT7Mixin2.One(5)"
          + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
          + "-BT7Mixin3.One(5)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(5)-BaseType7.One(5)"
          + "-BaseType7.Two-BT7Mixin2.Two",
          bt7.One (5));

      Assert.AreEqual ("BT7Mixin0.One(foo)-BT7Mixin2.One(foo)"
          + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
          + "-BT7Mixin3.One(foo)-BT7Mixin1.BT7Mixin1Specific"
              + "-BaseType7.Three"
              + "-BT7Mixin2.Three"
            + "-BaseType7.Three-BT7Mixin1.One(foo)-BaseType7.One(foo)"
          + "-BaseType7.Two-BT7Mixin2.Two",
          bt7.One ("foo"));

      Assert.AreEqual ("BT7Mixin2.Two", bt7.Two ());
      Assert.AreEqual ("BT7Mixin2.Three-BaseType7.Three", bt7.Three ());
      Assert.AreEqual ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BaseType7.Five-BaseType7.NotOverridden", bt7.Four ());
      Assert.AreEqual ("BT7Mixin9.Five-BaseType7.Five", bt7.Five ());
    }

    [Test]
    public void TestMultipleOverridesGrand ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType7> ().Clear()
          .AddMixin (typeof (BT7Mixin0)).WithDependency (typeof (IBT7Mixin7))
          .AddMixin (typeof (BT7Mixin7)).WithDependency (typeof (IBT7Mixin4))
          .AddMixin (typeof (BT7Mixin4)).WithDependency (typeof (IBT7Mixin6))
          .AddMixin (typeof (BT7Mixin6)).WithDependency (typeof (IBT7Mixin2))
          .AddMixin (typeof (BT7Mixin9)).WithDependency (typeof (IBT7Mixin8))
          .AddMixins (typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3), typeof (BT7Mixin5), typeof (BT7Mixin8), typeof (BT7Mixin10))
          .EnterScope())
      {
        BaseType7 bt7 = ObjectFactory.Create<BaseType7> ().With ();
        Assert.AreEqual ("BT7Mixin0.One(7)-BT7Mixin4.One(7)-BT7Mixin6.One(7)-BT7Mixin2.One(7)"
            + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(7)-BaseType7.One(7)"
            + "-BT7Mixin3.One(7)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(7)-BaseType7.One(7)"
            + "-BaseType7.Two"
            + "-BT7Mixin2.Two",
            bt7.One (7));

        Assert.AreEqual ("BT7Mixin0.One(bar)-BT7Mixin4.One(bar)-BT7Mixin6.One(bar)-BT7Mixin2.One(bar)"
            + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
            + "-BT7Mixin3.One(bar)-BT7Mixin1.BT7Mixin1Specific"
                + "-BaseType7.Three"
                + "-BT7Mixin2.Three-BaseType7.Three"
              + "-BT7Mixin1.One(bar)-BaseType7.One(bar)"
            + "-BaseType7.Two"
            + "-BT7Mixin2.Two",
            bt7.One ("bar"));

        Assert.AreEqual ("BT7Mixin2.Two", bt7.Two ());
        Assert.AreEqual ("BT7Mixin2.Three-BaseType7.Three", bt7.Three ());
        Assert.AreEqual ("BT7Mixin2.Four-BaseType7.Four-BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five-BaseType7.NotOverridden", bt7.Four ());
        Assert.AreEqual ("BT7Mixin9.Five-BT7Mixin8.Five-BaseType7.Five", bt7.Five ());
      }
    }

    [Test]
    public void MixinOverridingInheritedClassMethod ()
    {
      ClassWithInheritedMethod cwim = ObjectFactory.Create<ClassWithInheritedMethod> ().With ();
      Assert.AreEqual ("MixinOverridingInheritedMethod.ProtectedInheritedMethod-BaseClassWithInheritedMethod.ProtectedInheritedMethod-"
          + "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod-"
          + "MixinOverridingInheritedMethod.PublicInheritedMethod-BaseClassWithInheritedMethod.PublicInheritedMethod",
          cwim.InvokeInheritedMethods ());
    }

    [Test]
    public void MixinWithProtectedOverrider ()
    {
      BaseType1 obj = CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider)).With();
      Assert.AreEqual ("MixinWithProtectedOverrider.VirtualMethod-BaseType1.VirtualMethod", obj.VirtualMethod ());
      Assert.AreEqual ("MixinWithProtectedOverrider.VirtualProperty-BaseType1.BackingField", obj.VirtualProperty);

      Assert.AreEqual (null, obj.GetVirtualEventInvocationList());
      obj.VirtualEvent += delegate { };
      Assert.AreEqual (2, obj.GetVirtualEventInvocationList ().Length);
    }

    [Test]
    public void ValueTypeMixin ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof (ValueTypeMixin)).With();
      Assert.AreEqual ("ValueTypeMixin.VirtualMethod", bt1.VirtualMethod());
    }

    [Test]
    public void AlphabeticOrdering ()
    {
      ClassWithMixinsAcceptingAlphabeticOrdering instance = ObjectFactory.Create<ClassWithMixinsAcceptingAlphabeticOrdering> ().With ();
      Assert.AreEqual (
          "MixinAcceptingAlphabeticOrdering1.ToString-MixinAcceptingAlphabeticOrdering2.ToString-ClassWithMixinsAcceptingAlphabeticOrdering.ToString",
          instance.ToString ());
    }
  }
}
