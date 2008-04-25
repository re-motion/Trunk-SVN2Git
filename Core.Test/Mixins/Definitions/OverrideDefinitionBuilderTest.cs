using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class OverrideDefinitionBuilderTest
  {
    [Test]
    public void MethodOverrides ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      Assert.IsFalse (mixin1.HasOverriddenMembers());
      Assert.IsFalse (mixin2.HasOverriddenMembers ());
      Assert.IsTrue (targetClass.HasOverriddenMembers ());

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] {typeof (string)});
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MethodDefinition overridden = targetClass.Methods[baseMethod1];

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));
      MethodDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Methods[mixinMethod1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);

      MethodDefinition notOverridden = targetClass.Methods[baseMethod2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void PropertyOverrides ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      PropertyInfo baseProperty1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo baseProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (string)});
      PropertyInfo mixinProperty1 = typeof (BT1Mixin1).GetProperty ("VirtualProperty");

      PropertyDefinition overridden = targetClass.Properties[baseProperty1];

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));

      PropertyDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Properties[mixinProperty1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.SetMethod, overrider.SetMethod.Base);

      PropertyDefinition notOverridden = targetClass.Properties[baseProperty2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.GetMethod, overrider.GetMethod.Base);
    }

    [Test]
    public void EventOverrides ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent1 = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      EventDefinition overridden = targetClass.Events[baseEvent1];

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));

      EventDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Events[mixinEvent1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);

      EventDefinition notOverridden = targetClass.Events[baseEvent2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
    }

    [Test]
    public void OverrideNonVirtualMethod ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      MethodDefinition overrider = mixin.Methods[typeof (BT4Mixin1).GetMethod ("NonVirtualMethod")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (targetClass, overrider.Base.DeclaringClass);

      List<MethodDefinition> overrides = new List<MethodDefinition> (targetClass.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualProperty ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      PropertyDefinition overrider = mixin.Properties[typeof (BT4Mixin1).GetProperty ("NonVirtualProperty")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (targetClass, overrider.Base.DeclaringClass);

      List<PropertyDefinition> overrides = new List<PropertyDefinition> (targetClass.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualEvent ()
    {
      TargetClassDefinition targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      EventDefinition overrider = mixin.Events[typeof (BT4Mixin1).GetEvent ("NonVirtualEvent")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (targetClass, overrider.Base.DeclaringClass);

      List<EventDefinition> overrides = new List<EventDefinition> (targetClass.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by '.*BT5Mixin1.Method' could not be found.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseMethod ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by '.*BT5Mixin4.Property' could not be found.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseProperty ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin4));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by '.*BT5Mixin5.Event' could not be found.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseEvent ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin5));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Ambiguous override", MatchType = MessageMatch.Contains)]
    public void ThrowsOnTargetClassOverridingMultipleMixinMethods()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMembers> ().Clear().AddMixins (typeof (MixinWithAbstractMembers), typeof(MixinWithSingleAbstractMethod2)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers));
      }
    }

    [Test]
    public void TargetClassOverridingSpecificMixinMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificMixinMember> ().Clear().AddMixins (typeof (MixinWithVirtualMethod), typeof (MixinWithVirtualMethod2)).EnterScope())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSpecificMixinMember));
        MethodDefinition method = definition.Methods[typeof (ClassOverridingSpecificMixinMember).GetMethod ("VirtualMethod")];
        Assert.AreSame (method.Base.DeclaringClass, definition.Mixins[typeof (MixinWithVirtualMethod)]);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by "
                                                                           + "'Remotion.UnitTests.Mixins.SampleTypes.ClassOverridingSpecificMixinMember.VirtualMethod' could not be found.")]
    public void TargetClassOverridingSpecificUnconfiguredMixinMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificMixinMember> ().Clear().AddMixins (typeof (MixinWithVirtualMethod2)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSpecificMixinMember));
      }
    }

    [Test]
    public void TargetClassOverridingSpecificGenericMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificGenericMixinMember> ().Clear().AddMixins (typeof (GenericMixinWithVirtualMethod<>), typeof (GenericMixinWithVirtualMethod2<>)).EnterScope())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSpecificGenericMixinMember));
        MethodDefinition method = definition.Methods[typeof (ClassOverridingSpecificGenericMixinMember).GetMethod ("VirtualMethod")];
        Assert.AreSame (method.Base.DeclaringClass, definition.GetMixinByConfiguredType (typeof (GenericMixinWithVirtualMethod<>)));
      }
    }

    [Test]
    public void OverridingProtectedInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInheritedMethod",
          BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingProtectedInternalInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
          BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingPublicInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("PublicInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("PublicInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingProtectedInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
              typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)],
          inheritedMethod.Base);
    }

    [Test]
    public void OverridingProtectedInternelInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInternalInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
              typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)],
          inheritedMethod.Base);
    }

    [Test]
    public void OverridingPublicInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("PublicInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[typeof (BaseMixinWithInheritedMethod).GetMethod ("PublicInheritedMethod")],
          inheritedMethod.Base);
    }
  }
}