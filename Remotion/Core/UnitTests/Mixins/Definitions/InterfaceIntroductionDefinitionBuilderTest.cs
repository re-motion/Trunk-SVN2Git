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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class InterfaceIntroductionDefinitionBuilderTest
  {
    public class MixinImplementingISerializable : ISerializable, IDisposable, IDeserializationCallback
    {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        throw new NotImplementedException();
      }

      public void OnDeserialization (object sender)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void IntroducedInterface ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.IsTrue (mixin1.InterfaceIntroductions.ContainsKey (typeof (IBT1Mixin1)));
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];
      Assert.AreSame (mixin1, introducedInterface.Parent);
      Assert.AreSame (mixin1, introducedInterface.Implementer);
      Assert.IsTrue (targetClass.ReceivedInterfaces.ContainsKey (typeof (IBT1Mixin1)));
      Assert.AreSame (targetClass.ReceivedInterfaces[typeof (IBT1Mixin1)], introducedInterface);
      Assert.AreSame (targetClass, introducedInterface.TargetClass);
    }

    public interface IIntroducedBase
    {
      void Foo ();
      string FooP { get; set; }
      event EventHandler FooE;
    }

    public class BaseIntroducer : IIntroducedBase
    {
      public void Foo ()
      {
        throw new NotImplementedException();
      }

      public string FooP
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public event EventHandler FooE;
    }

    public interface IIntroducedDerived : IIntroducedBase
    {
    }

    public class DerivedIntroducer : BaseIntroducer, IIntroducedDerived
    {
    }

    [Test]
    public void IntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (DerivedIntroducer)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
        Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IIntroducedDerived)));
        Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IIntroducedBase)));

        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedMethods.Count);
        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedProperties.Count);
        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedEvents.Count);

        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods.Count);
        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties.Count);
        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents.Count);

        Assert.AreEqual (
            typeof (IIntroducedBase).GetMethod ("Foo"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].InterfaceMember);
        Assert.AreEqual (
            typeof (IIntroducedBase).GetProperty ("FooP"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].InterfaceMember);
        Assert.AreEqual (
            typeof (IIntroducedBase).GetEvent ("FooE"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].InterfaceMember);

        Assert.AreEqual (
            bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (
            bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (
            bt1.Mixins[typeof (DerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass);
      }
    }

    public class ExplicitBaseIntroducer : IIntroducedBase
    {
      void IIntroducedBase.Foo ()
      {
        throw new NotImplementedException();
      }

      string IIntroducedBase.FooP
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      event EventHandler IIntroducedBase.FooE
      {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
      }
    }

    public class ExplicitDerivedIntroducer : ExplicitBaseIntroducer, IIntroducedDerived
    {
    }

    [Test]
    public void ExplicitlyIntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (ExplicitDerivedIntroducer)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
        Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IIntroducedDerived)));
        Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IIntroducedBase)));

        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedMethods.Count);
        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedProperties.Count);
        Assert.AreEqual (0, bt1.ReceivedInterfaces[typeof (IIntroducedDerived)].IntroducedEvents.Count);

        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods.Count);
        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties.Count);
        Assert.AreEqual (1, bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents.Count);

        Assert.AreEqual (
            typeof (IIntroducedBase).GetMethod ("Foo"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].InterfaceMember);
        Assert.AreEqual (
            typeof (IIntroducedBase).GetProperty ("FooP"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].InterfaceMember);
        Assert.AreEqual (
            typeof (IIntroducedBase).GetEvent ("FooE"),
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].InterfaceMember);

        Assert.AreEqual (
            bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (
            bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass);
        Assert.AreEqual (
            bt1.Mixins[typeof (ExplicitDerivedIntroducer)],
            bt1.ReceivedInterfaces[typeof (IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass);
      }
    }

    [Test]
    public void IntroducedMembers ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof (IBT1Mixin1)];

      Assert.IsTrue (introducedInterface.IntroducedMethods.ContainsKey (typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")));
      Assert.IsTrue (introducedInterface.IntroducedProperties.ContainsKey (typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")));
      Assert.IsTrue (introducedInterface.IntroducedEvents.ContainsKey (typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")));

      MethodIntroductionDefinition method = introducedInterface.IntroducedMethods[typeof (IBT1Mixin1).GetMethod ("IntroducedMethod")];
      Assert.AreNotEqual (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method);
      Assert.AreSame (mixin1.Methods[typeof (BT1Mixin1).GetMethod ("IntroducedMethod")], method.ImplementingMember);
      Assert.AreSame (introducedInterface, method.DeclaringInterface);
      Assert.AreSame (introducedInterface, method.Parent);

      PropertyIntroductionDefinition property = introducedInterface.IntroducedProperties[typeof (IBT1Mixin1).GetProperty ("IntroducedProperty")];
      Assert.AreNotEqual (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property);
      Assert.AreSame (mixin1.Properties[typeof (BT1Mixin1).GetProperty ("IntroducedProperty")], property.ImplementingMember);
      Assert.AreSame (introducedInterface, property.DeclaringInterface);
      Assert.AreSame (introducedInterface, method.Parent);

      EventIntroductionDefinition eventDefinition = introducedInterface.IntroducedEvents[typeof (IBT1Mixin1).GetEvent ("IntroducedEvent")];
      Assert.AreNotEqual (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition);
      Assert.AreSame (mixin1.Events[typeof (BT1Mixin1).GetEvent ("IntroducedEvent")], eventDefinition.ImplementingMember);
      Assert.AreSame (introducedInterface, eventDefinition.DeclaringInterface);
      Assert.AreSame (introducedInterface, method.Parent);
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinWithExplicitImplementation)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
        Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IExplicit)));

        MethodInfo explicitMethod = typeof (MixinWithExplicitImplementation).GetMethod (
            "Remotion.UnitTests.Mixins.SampleTypes.IExplicit.Explicit", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull (explicitMethod);

        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithExplicitImplementation)];
        Assert.IsTrue (m1.Methods.ContainsKey (explicitMethod));
      }
    }

    [Test]
    public void ISerializableIsNotIntroduced ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingISerializable)).EnterScope())
      {
        Assert.IsNull (
            DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (ISerializable)]);
        Assert.IsNotNull (
            DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (IDisposable)]);
      }
    }

    [Test]
    public void IDeserializationCallbackIsNotIntroduced ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingISerializable)).EnterScope())
      {
        Assert.IsNull (
            DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (MixinImplementingISerializable)].InterfaceIntroductions[typeof (IDeserializationCallback)]);
      }
    }

    [Test]
    public void IInitializableMixinIsNotIntroduced ()
    {
      Assert.IsNull (
          DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)].InterfaceIntroductions[typeof (IInitializableMixin)]);
    }

    [Test]
    public void IntroducesGetMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingFullPropertiesWithPartialIntroduction)).EnterScope())
      {
        InterfaceIntroductionDefinition introduction = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).ReceivedInterfaces[typeof (InterfaceWithPartialProperties)];
        PropertyIntroductionDefinition prop1 = introduction.IntroducedProperties[typeof (InterfaceWithPartialProperties).GetProperty ("Prop1")];
        PropertyIntroductionDefinition prop2 = introduction.IntroducedProperties[typeof (InterfaceWithPartialProperties).GetProperty ("Prop2")];
        Assert.IsTrue (prop1.IntroducesGetMethod);
        Assert.IsFalse (prop1.IntroducesSetMethod);
        Assert.IsTrue (prop2.IntroducesSetMethod);
        Assert.IsFalse (prop2.IntroducesGetMethod);
      }
    }

    private class BT1Mixin1A : BT1Mixin1
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two mixins introduce the same interface .* to base class .*",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnDoublyIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin1A)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      }
    }

    [Test]
    public void InterfaceImplementedByTargetClassCannotBeIntroduced ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassImplementingSimpleInterface> ().Clear().AddMixins (typeof (MixinImplementingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingSimpleInterface));
        Assert.IsTrue (definition.ImplementedInterfaces.Contains (typeof (ISimpleInterface)));
        Assert.IsFalse (definition.ReceivedInterfaces.ContainsKey (typeof (ISimpleInterface)));
        Assert.IsTrue (
            definition.Mixins[typeof (MixinImplementingSimpleInterface)].NonInterfaceIntroductions.ContainsKey (
                typeof (ISimpleInterface)));
      }
    }

    [Test]
    public void NonIntroducedInterfaceIsNotImplemented ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinNonIntroducingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.IsFalse (definition.ImplementedInterfaces.Contains (typeof (ISimpleInterface)));
        Assert.IsFalse (definition.ReceivedInterfaces.ContainsKey (typeof (ISimpleInterface)));
        Assert.IsTrue (
            definition.Mixins[typeof (MixinNonIntroducingSimpleInterface)].NonInterfaceIntroductions.ContainsKey (
                typeof (ISimpleInterface)));
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinNonIntroducingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        NonInterfaceIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinNonIntroducingSimpleInterface)].NonInterfaceIntroductions[typeof (ISimpleInterface)];
        Assert.IsTrue (nonIntroductionDefinition.IsExplicitlySuppressed);
        Assert.IsFalse (nonIntroductionDefinition.IsShadowed);
        Assert.AreSame (typeof (ISimpleInterface), nonIntroductionDefinition.InterfaceType);
        Assert.AreSame (definition.Mixins[typeof (MixinNonIntroducingSimpleInterface)], nonIntroductionDefinition.Parent);
      }
    }

    [Test]
    public void ImplicitlyNonIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassImplementingSimpleInterface> ().Clear().AddMixins (typeof (MixinImplementingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingSimpleInterface));
        NonInterfaceIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof (MixinImplementingSimpleInterface)].NonInterfaceIntroductions[typeof (ISimpleInterface)];
        Assert.IsFalse (nonIntroductionDefinition.IsExplicitlySuppressed);
        Assert.IsTrue (nonIntroductionDefinition.IsShadowed);
        Assert.AreSame (typeof (ISimpleInterface), nonIntroductionDefinition.InterfaceType);
        Assert.AreSame (definition.Mixins[typeof (MixinImplementingSimpleInterface)], nonIntroductionDefinition.Parent);
      }
    }

    [Test]
    public void MultipleSimilarInterfaces ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassImplementingSimpleInterface> ().Clear().AddMixins (typeof (List<>)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingSimpleInterface));
        MixinDefinition mixinDefinition = definition.GetMixinByConfiguredType (typeof (List<>));

        Assert.IsTrue (definition.ReceivedInterfaces.ContainsKey (typeof (IList)));
        Assert.AreSame (mixinDefinition, definition.ReceivedInterfaces[typeof (IList)].Implementer);

        Assert.IsTrue (definition.ReceivedInterfaces.ContainsKey (typeof (ICollection<ClassImplementingSimpleInterface>)));
        Assert.AreSame (mixinDefinition, definition.ReceivedInterfaces[typeof (ICollection<ClassImplementingSimpleInterface>)].Implementer);

        Assert.IsTrue (definition.ReceivedInterfaces[typeof (IList)].IntroducedProperties.ContainsKey (typeof (IList).GetProperty ("IsReadOnly")));
        Assert.IsTrue (
            definition.ReceivedInterfaces[typeof (ICollection<ClassImplementingSimpleInterface>)].IntroducedProperties.ContainsKey (
                typeof (ICollection<ClassImplementingSimpleInterface>).GetProperty ("IsReadOnly")));

        Assert.AreNotEqual (
            definition.ReceivedInterfaces[typeof (IList)].IntroducedProperties[typeof (IList).GetProperty ("IsReadOnly")].ImplementingMember,
            definition.ReceivedInterfaces[typeof (ICollection<ClassImplementingSimpleInterface>)].IntroducedProperties[
                typeof (ICollection<ClassImplementingSimpleInterface>).GetProperty ("IsReadOnly")].ImplementingMember);
      }
    }

    [Test]
    public void DefaultVisibility_Private ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<NullTarget> ()
            .AddMixin<MixinIntroducingMembersWithDifferentVisibilities> ().WithIntroducedMemberVisibility (MemberVisibility.Private)
          .EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That (interfaceDefinition.IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Private));
        Assert.That (interfaceDefinition.IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Private));
        Assert.That (interfaceDefinition.IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Private));
      }
    }

    [Test]
    public void DefaultVisibility_Public ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<NullTarget> ()
            .AddMixin<MixinIntroducingMembersWithDifferentVisibilities> ().WithIntroducedMemberVisibility (MemberVisibility.Public)
          .EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That (interfaceDefinition.IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
        Assert.That (interfaceDefinition.IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
        Assert.That (interfaceDefinition.IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithDefaultVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
      }
    }

    [Test]
    public void SpecialVisibility ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<NullTarget> ()
            .AddMixin<MixinIntroducingMembersWithDifferentVisibilities> ().WithIntroducedMemberVisibility (MemberVisibility.Private)
          .EnterScope ())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof (IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That (interfaceDefinition.IntroducedMethods[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetMethod ("MethodWithPublicVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
        Assert.That (interfaceDefinition.IntroducedProperties[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetProperty ("PropertyWithPublicVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
        Assert.That (interfaceDefinition.IntroducedEvents[typeof (IMixinIntroducingMembersWithDifferentVisibilities).GetEvent ("EventWithPublicVisibility")].Visibility,
            Is.EqualTo (MemberVisibility.Public));
      }
    }
  }
}
