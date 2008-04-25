using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class RequiredMethodDefinitionBuilderTest
  {
    private static void CheckRequiredMethods (RequirementDefinitionBase requirement, ClassDefinitionBase implementer, string memberPrefix)
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.AreEqual (5, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method", bf)];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method", bf), method.InterfaceMethod);
      Assert.AreSame (implementer.Methods[implementer.Type.GetMethod (memberPrefix + "Method", bf)],
          method.ImplementingMethod);

      RequiredMethodDefinition propertyGetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property", bf)];
      Assert.IsNotNull (propertyGetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property", bf), propertyGetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty (memberPrefix + "Property", bf)].GetMethod,
          propertyGetter.ImplementingMethod);

      RequiredMethodDefinition propertySetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property", bf)];
      Assert.IsNotNull (propertySetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property", bf), propertySetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty (memberPrefix + "Property", bf)].SetMethod,
          propertySetter.ImplementingMethod);

      RequiredMethodDefinition eventAdder = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event", bf)];
      Assert.IsNotNull (eventAdder);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event", bf), eventAdder.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent (memberPrefix + "Event", bf)].AddMethod,
          eventAdder.ImplementingMethod);

      RequiredMethodDefinition eventRemover = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event", bf)];
      Assert.IsNotNull (eventRemover);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event", bf), eventRemover.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent (memberPrefix + "Event", bf)].RemoveMethod,
          eventRemover.ImplementingMethod);
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods(requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsInterfaceImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassFulfillingNoMemberRequirements)).Clear().AddMixins (typeof (MixinRequiringAllMembersFace), typeof (MixinFulfillingAllMemberRequirements)).EnterScope())
      {
        TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = TargetClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredBaseCallMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassFulfillingNoMemberRequirements)).Clear().AddMixins (typeof (MixinRequiringAllMembersBase), typeof (MixinFulfillingAllMemberRequirements)).EnterScope())
      {
        TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = TargetClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredFaceMethodsDuckImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsDuckImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    public class MixinRequiringSingleMethod : Mixin<MixinRequiringSingleMethod.IRequirement>
    {
      public interface IRequirement
      {
        void Method ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IRequirement \\(mixins .*MixinRequiringSingleMethod applied to class .*NullTarget\\) is not fulfilled - "
        + "public or protected method Method could not be found on the base class.", MatchType = MessageMatch.Regex)]
    public void ThrowsIfMethodRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleMethod)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    public class MixinRequiringSingleProperty : Mixin<MixinRequiringSingleProperty.IRequirement>
    {
      public interface IRequirement
      {
        int Property { get; }
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IRequirement \\(mixins .*MixinRequiringSingleProperty applied to class .*NullTarget\\) is not fulfilled - "
        + "public or protected property Property could not be found on the base class.", MatchType = MessageMatch.Regex)]
    public void ThrowsIfPropertyRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleProperty)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    public class MixinRequiringSingleEvent : Mixin<MixinRequiringSingleEvent.IRequirement>
    {
      public interface IRequirement
      {
        event EventHandler Event;
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IRequirement \\(mixins .*MixinRequiringSingleEvent applied to class .*NullTarget\\) is not fulfilled - public "
        + "or protected event Event could not be found on the base class.", MatchType = MessageMatch.Regex)]
    public void ThrowsIfEventRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleEvent)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    public class ClassFulfillingPrivately
    {
      private void Method ()
      {
        throw new NotImplementedException ();
      }

      public int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<string> Event;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency IMixinRequiringAllMembersRequirements \\(mixins "
        + ".*MixinRequiringAllMembersFace applied to class .*ClassFulfillingPrivately\\) is not fulfilled - "
        + "public or protected method Method could not be found on the base class.", MatchType = MessageMatch.Regex)]
    public void ThrowsIfRequiredMethodIsPrivate ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingPrivately> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingPrivately));
      }
    }

    public class ClassFulfillingInternally
    {
      internal void Method ()
      {
        throw new NotImplementedException ();
      }

      public int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<string> Event;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency IMixinRequiringAllMembersRequirements \\(mixins "
        + ".*MixinRequiringAllMembersFace applied to class .*ClassFulfillingInternally\\) is not fulfilled - "
        + "public or protected method Method could not be found on the base class.", MatchType = MessageMatch.Regex)]
    public void ThrowsIfRequiredMethodIsInternal ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingInternally> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingInternally));
      }
    }

    public class ClassFulfillingProtectedly
    {
      protected void Method ()
      {
        throw new NotImplementedException ();
      }

      protected int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      protected event Func<string> Event;
    }

    [Test]
    public void WorksIfRequiredMethodIsProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingProtectedly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingProtectedly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, "");
      }
    }

    [Test]
    public void WorksIfExplicitlyImplemented ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsExplicitly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, typeof (IMixinRequiringAllMembersRequirements).FullName + ".");
      }
    }

    [Test]
    public void NoRequiredMethodsWhenFaceRequirementIsClass ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithStaticMethod));
      RequiredFaceTypeDefinition requirement = targetClass.RequiredFaceTypes[typeof (ClassWithStaticMethod)];
      Assert.AreEqual (0, requirement.Methods.Count);
    }
  }
}