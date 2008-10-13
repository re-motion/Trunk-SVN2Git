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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MemberDefinitionBuilderTest
  {
    [Test]
    public void Methods ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new[] {typeof (string)});
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.IsTrue (targetClass.Methods.ContainsKey (baseMethod1));
      Assert.IsFalse (targetClass.Methods.ContainsKey (mixinMethod1));

      MemberDefinitionBase member = targetClass.Methods[baseMethod1];

      Assert.IsTrue (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member));
      Assert.IsFalse (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (targetClass, member.DeclaringClass);
      Assert.AreSame (targetClass, member.Parent);

      Assert.IsTrue (targetClass.Methods.ContainsKey (baseMethod2));
      Assert.AreNotSame (member, targetClass.Methods[baseMethod2]);

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Methods.ContainsKey (baseMethod1));
      Assert.IsTrue (mixin1.Methods.ContainsKey (mixinMethod1));
      member = mixin1.Methods[mixinMethod1];

      Assert.IsTrue (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualMethod", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualMethod", member.FullName);
      Assert.IsTrue (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixin1, member.DeclaringClass);
    }

    [Test]
    public void Properties ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      PropertyInfo baseProperty = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo indexedProperty1 = typeof (BaseType1).GetProperty ("Item", new[] {typeof (int)});
      PropertyInfo indexedProperty2 = typeof (BaseType1).GetProperty ("Item", new[] {typeof (string)});
      PropertyInfo mixinProperty = typeof (BT1Mixin1).GetProperty ("VirtualProperty", new Type[0]);

      Assert.IsTrue (targetClass.Properties.ContainsKey (baseProperty));
      Assert.IsTrue (targetClass.Properties.ContainsKey (indexedProperty1));
      Assert.IsTrue (targetClass.Properties.ContainsKey (indexedProperty2));
      Assert.IsFalse (targetClass.Properties.ContainsKey (mixinProperty));

      PropertyDefinition member = targetClass.Properties[baseProperty];

      Assert.IsTrue (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member));
      Assert.IsFalse (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualProperty", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualProperty", member.FullName);
      Assert.IsTrue (member.IsProperty);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (targetClass, member.DeclaringClass);
      Assert.IsNotNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);

      Assert.IsFalse (targetClass.Methods.ContainsKey (member.GetMethod.MethodInfo));
      Assert.IsFalse (targetClass.Methods.ContainsKey (member.SetMethod.MethodInfo));

      Assert.AreSame (member, member.GetMethod.Parent);
      Assert.AreSame (member, member.SetMethod.Parent);

      member = targetClass.Properties[indexedProperty1];
      Assert.AreNotSame (member, targetClass.Properties[indexedProperty2]);

      Assert.IsNotNull (member.GetMethod);
      Assert.IsNull (member.SetMethod);

      member = targetClass.Properties[indexedProperty2];

      Assert.IsNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Properties.ContainsKey (baseProperty));
      Assert.IsTrue (mixin1.Properties.ContainsKey (mixinProperty));

      member = mixin1.Properties[mixinProperty];

      Assert.IsTrue (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualProperty", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualProperty", member.FullName);
      Assert.IsTrue (member.IsProperty);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsEvent);
      Assert.AreSame (mixin1, member.DeclaringClass);

      Assert.IsNull (member.GetMethod);
      Assert.IsNotNull (member.SetMethod);
    }

    [Test]
    public void Events ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      Assert.IsTrue (targetClass.Events.ContainsKey (baseEvent1));
      Assert.IsTrue (targetClass.Events.ContainsKey (baseEvent2));
      Assert.IsFalse (targetClass.Events.ContainsKey (mixinEvent));

      EventDefinition member = targetClass.Events[baseEvent1];

      Assert.IsTrue (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member));
      Assert.IsFalse (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualEvent", member.Name);
      Assert.AreEqual (typeof (BaseType1).FullName + ".VirtualEvent", member.FullName);
      Assert.IsTrue (member.IsEvent);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.AreSame (targetClass, member.DeclaringClass);
      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);

      Assert.IsFalse (targetClass.Methods.ContainsKey (member.AddMethod.MethodInfo));
      Assert.IsFalse (targetClass.Methods.ContainsKey (member.RemoveMethod.MethodInfo));

      Assert.AreSame (member, member.AddMethod.Parent);
      Assert.AreSame (member, member.RemoveMethod.Parent);

      member = targetClass.Events[baseEvent2];
      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsFalse (mixin1.Events.ContainsKey (baseEvent1));
      Assert.IsTrue (mixin1.Events.ContainsKey (mixinEvent));

      member = mixin1.Events[mixinEvent];

      Assert.IsTrue (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member));

      Assert.AreEqual ("VirtualEvent", member.Name);
      Assert.AreEqual (typeof (BT1Mixin1).FullName + ".VirtualEvent", member.FullName);
      Assert.IsTrue (member.IsEvent);
      Assert.IsFalse (member.IsMethod);
      Assert.IsFalse (member.IsProperty);
      Assert.AreSame (mixin1, member.DeclaringClass);

      Assert.IsNotNull (member.AddMethod);
      Assert.IsNotNull (member.RemoveMethod);
    }

    public class Base<T>
    {
      public virtual void Method(T t)
      {
      }

      public virtual T Property
      {
        get { return default(T);}
        set { Dev.Null = value; }
      }

      public virtual event Func<T> Event;
    }

    public class Derived : Base<int>
    {
      public virtual new void Method (int t)
      {
      }

      public virtual new int Property
      {
        get { return default (int); }
        set { Dev.Null = value; }
      }

      public virtual new event Func<int> Event;
    }

    public class ExtraDerived : Derived
    {
      public virtual new void Method (int t)
      {
      }

      public virtual new int Property
      {
        get { return default (int); }
        set { Dev.Null = value; }
      }

      public virtual new event Func<int> Event;
    }

    public class DerivedWithOverrides : ExtraDerived
    {
      public override void Method (int t)
      {
      }

      public override int Property
      {
        get { return default (int); }
        set { Dev.Null = value; }
      }

      public override event Func<int> Event;
    }

    public class ExtraExtraDerived : DerivedWithOverrides
    {
      public new void Method (int t)
      {
      }

      public new int Property
      {
        get { return default (int); }
        set { Dev.Null = value; }
      }

      public new event Func<int> Event;
    }

    [Test]
    public void ShadowedMembersExplicitlyRetrievedButOverriddenNot()
    {
      TargetClassDefinition d = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ExtraExtraDerived));
      const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      Assert.IsTrue (d.Methods.ContainsKey (typeof (ExtraExtraDerived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (DerivedWithOverrides).GetMethod ("Method", bf)));
      Assert.IsFalse (d.Methods.ContainsKey (typeof (ExtraDerived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (Derived).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (Base<int>).GetMethod ("Method", bf)));

      Assert.IsTrue (d.Properties.ContainsKey (typeof (ExtraExtraDerived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (DerivedWithOverrides).GetProperty ("Property", bf)));
      Assert.IsFalse (d.Properties.ContainsKey (typeof (ExtraDerived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Derived).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Base<int>).GetProperty ("Property", bf)));

      Assert.IsTrue (d.Events.ContainsKey (typeof (ExtraExtraDerived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (DerivedWithOverrides).GetEvent ("Event", bf)));
      Assert.IsFalse (d.Events.ContainsKey (typeof (ExtraDerived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (Derived).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (Base<int>).GetEvent ("Event", bf)));

      Assert.AreEqual (18, new List<MemberDefinitionBase> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ShadowedMixinMembersExplicitlyRetrieved()
    {
      MixinDefinition d = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin2)).Mixins[typeof (BT3Mixin2)];

      Assert.IsTrue (d.Properties.ContainsKey (typeof (BT3Mixin2).GetProperty ("This")));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.AreEqual (11, new List<MemberDefinitionBase> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ProtectedInternalMembers ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      Assert.IsTrue (targetClass.Methods.ContainsKey (typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
          BindingFlags.Instance | BindingFlags.NonPublic)));
    }

    [Test]
    public void IsAbstractTrue ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (AbstractBaseType), GenerationPolicy.ForceGeneration);
      Assert.IsTrue (bt1.Methods[typeof (AbstractBaseType).GetMethod ("VirtualMethod")].IsAbstract);
    }

    [Test]
    public void IsAbstractFalse ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.IsFalse (bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)].IsAbstract);
    }
  }
}
