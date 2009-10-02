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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.Definitions.TestDomain.MemberFiltering;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MemberDefinitionBuilderTest
  {
    [Test]
    public void Methods ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

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
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

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
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

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

    [Test]
    public void ShadowedMembersExplicitlyRetrievedButOverriddenNot()
    {
      TargetClassDefinition d = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (DerivedDerivedDerivedDerivedWithNewMembers));
      const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      Assert.IsTrue (d.Methods.ContainsKey (typeof (DerivedDerivedDerivedDerivedWithNewMembers).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (DerivedDerivedDerivedWithOverrides).GetMethod ("Method", bf)));
      Assert.IsFalse (d.Methods.ContainsKey (typeof (DerivedDerivedWithNewVirtualMembers).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetMethod ("Method", bf)));
      Assert.IsTrue (d.Methods.ContainsKey (typeof (BaseWithVirtualMembers<int>).GetMethod ("Method", bf)));

      Assert.IsTrue (d.Properties.ContainsKey (typeof (DerivedDerivedDerivedDerivedWithNewMembers).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (DerivedDerivedDerivedWithOverrides).GetProperty ("Property", bf)));
      Assert.IsFalse (d.Properties.ContainsKey (typeof (DerivedDerivedWithNewVirtualMembers).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetProperty ("Property", bf)));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (BaseWithVirtualMembers<int>).GetProperty ("Property", bf)));

      Assert.IsTrue (d.Events.ContainsKey (typeof (DerivedDerivedDerivedDerivedWithNewMembers).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (DerivedDerivedDerivedWithOverrides).GetEvent ("Event", bf)));
      Assert.IsFalse (d.Events.ContainsKey (typeof (DerivedDerivedWithNewVirtualMembers).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetEvent ("Event", bf)));
      Assert.IsTrue (d.Events.ContainsKey (typeof (BaseWithVirtualMembers<int>).GetEvent ("Event", bf)));

      Assert.AreEqual (18, new List<MemberDefinitionBase> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ShadowedMixinMembersExplicitlyRetrieved()
    {
      MixinDefinition d = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin2)).Mixins[typeof (BT3Mixin2)];

      Assert.IsTrue (d.Properties.ContainsKey (typeof (BT3Mixin2).GetProperty ("This")));
      Assert.IsTrue (d.Properties.ContainsKey (typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.AreEqual (11, new List<MemberDefinitionBase> (d.GetAllMembers ()).Count);
    }

    [Test]
    public void ProtectedInternalMembers ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInheritedMethod));
      Assert.IsTrue (targetClass.Methods.ContainsKey (typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
          BindingFlags.Instance | BindingFlags.NonPublic)));
    }

    [Test]
    public void IsAbstractTrue ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (typeof (AbstractBaseType));
      Assert.IsTrue (bt1.Methods[typeof (AbstractBaseType).GetMethod ("VirtualMethod")].IsAbstract);
    }

    [Test]
    public void IsAbstractFalse ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.IsFalse (bt1.Methods[typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes)].IsAbstract);
    }
  }
}
