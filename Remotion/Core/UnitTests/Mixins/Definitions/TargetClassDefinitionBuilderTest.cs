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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionBuilderTest
  {
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGenericTargetClass ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BT3Mixin3<,>)).Clear().EnterScope())
      {
        var builder = new TargetClassDefinitionBuilder();
        builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
      }
    }

    [Test]
    public void TargetClassDefinitionKnowsItsContext ()
    {
      var classContext = new ClassContext (typeof (BaseType1));
      TargetClassDefinition def = DefinitionObjectMother.GetTargetClassDefinition (classContext);
      Assert.That (def.ConfigurationContext, Is.Not.Null);
      Assert.That (def.ConfigurationContext, Is.EqualTo (classContext));
    }

    public interface IInterfaceWithAllMembers
    {
      void Method ();
      string Property { get; set; }
      event EventHandler Event;
    }

    public class ClassWithExplicitInterfaceImplementation : IInterfaceWithAllMembers
    {
      void IInterfaceWithAllMembers.Method ()
      {
        throw new Exception ("The method or operation is not implemented.");
      }

      string IInterfaceWithAllMembers.Property
      {
        get { throw new Exception ("The method or operation is not implemented."); }
        set { throw new Exception ("The method or operation is not implemented."); }
      }

      event EventHandler IInterfaceWithAllMembers.Event
      {
        add { throw new Exception ("The method or operation is not implemented."); }
        remove { throw new Exception ("The method or operation is not implemented."); }
      }
    }

    [Test]
    public void TargetClassHasExplicitInterfaceMembers ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (
          typeof (ClassWithExplicitInterfaceImplementation));

      Assert.That (definition.Methods.Count, Is.EqualTo (7));
      Assert.That (definition.Properties.Count, Is.EqualTo (1));
      Assert.That (definition.Events.Count, Is.EqualTo (1));

      BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      Assert.That (
          definition.Methods.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Method", bf)),
          Is.True);

      Assert.That (
          definition.Properties.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)),
          Is.True);
      Assert.That (
          definition.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)].GetMethod.
              MemberInfo,
          Is.EqualTo (
              typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.get_Property", bf)));
      Assert.That (
          definition.Properties[
              typeof (ClassWithExplicitInterfaceImplementation).GetProperty (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Property", bf)].SetMethod.
              MemberInfo,
          Is.EqualTo (
              typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.set_Property", bf)));

      Assert.That (
          definition.Events.ContainsKey (
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)),
          Is.True);
      Assert.That (
          definition.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)].AddMethod.MemberInfo,
          Is.EqualTo (
              typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.add_Event", bf)));
      Assert.That (
          definition.Events[
              typeof (ClassWithExplicitInterfaceImplementation).GetEvent (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.Event", bf)].RemoveMethod.
              MemberInfo,
          Is.EqualTo (
              typeof (ClassWithExplicitInterfaceImplementation).GetMethod (
                  "Remotion.UnitTests.Mixins.Definitions.TargetClassDefinitionBuilderTest.IInterfaceWithAllMembers.remove_Event", bf)));
    }

    [Test]
    public void HasOverriddenMembersTrue ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (bt1.HasOverriddenMembers(), Is.True);
    }

    [Test]
    public void HasOverriddenMembersFalse ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (ClassOverridingMixinMembersProtected),
          typeof (MixinWithAbstractMembers));
      Assert.That (bt1.HasOverriddenMembers(), Is.False);
    }

    [Test]
    public void GetProtectedOverriders ()
    {
      const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;

      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (ClassOverridingMixinMembersProtected),
          typeof (MixinWithAbstractMembers));

      var overriders = bt1.GetProtectedOverriders();
      Assert.That (
          overriders.Select (m => m.MethodInfo).ToArray(),
          Is.EquivalentTo (
              new[]
              {
                  typeof (ClassOverridingMixinMembersProtected).GetMethod ("AbstractMethod", bf),
                  typeof (ClassOverridingMixinMembersProtected).GetMethod ("RaiseEvent", bf),
                  typeof (ClassOverridingMixinMembersProtected).GetMethod ("get_AbstractProperty", bf),
                  typeof (ClassOverridingMixinMembersProtected).GetMethod ("add_AbstractEvent", bf),
                  typeof (ClassOverridingMixinMembersProtected).GetMethod ("remove_AbstractEvent", bf),
              }));
    }

    [Test]
    public void HasProtectedOverridersTrue ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (
          typeof (ClassOverridingMixinMembersProtected),
          typeof (MixinWithAbstractMembers));
      Assert.That (bt1.HasProtectedOverriders(), Is.True);
    }

    [Test]
    public void HasProtectedOverridersFalse ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (bt1.HasProtectedOverriders(), Is.False);
    }

    [Test]
    public void IsAbstractTrue ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition_Force (typeof (AbstractBaseType));
      Assert.That (bt1.IsAbstract, Is.True);
    }

    [Test]
    public void IsAbstractFalse ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (bt1.IsAbstract, Is.False);
    }
  }
}