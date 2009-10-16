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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions.Building
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
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods(requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsInterfaceImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirements));
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
        TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingNoMemberRequirements));
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
        TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingNoMemberRequirements));
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
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsDuckImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency 'IRequirement' (required by mixin(s) "
                          + "'Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleMethod' applied to class "
                          + "'Remotion.UnitTests.Mixins.SampleTypes.NullTarget') is not fulfilled - public or protected method 'Void Method()' could not be found on "
                          + "the target class.")]
    public void ThrowsIfMethodRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleMethod)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency 'IRequirement' (required by mixin(s) "
                          + "'Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleProperty' applied to class "
                          + "'Remotion.UnitTests.Mixins.SampleTypes.NullTarget') is not fulfilled - public or protected method 'Int32 get_Property()' could not be "
                          + "found on the target class.")]
    public void ThrowsIfPropertyRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleProperty)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency 'IRequirement' (required by mixin(s) "
                          + "'Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleEvent' applied to class "
                          + "'Remotion.UnitTests.Mixins.SampleTypes.NullTarget') is not fulfilled - public or protected method 'Void add_Event(System.EventHandler)' "
                          + "could not be found on the target class.")]
    public void ThrowsIfEventRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleEvent)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency 'IMixinRequiringAllMembersRequirements' (required by "
                                                                           + "mixin(s) 'Remotion.UnitTests.Mixins.SampleTypes.MixinRequiringAllMembersFace' applied to class "
                                                                           + "'Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding.ClassFulfillingPrivately') is not fulfilled - public "
                                                                           + "or protected method 'Void Method()' could not be found on the target class.")]
    public void ThrowsIfRequiredMethodIsPrivate ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingPrivately> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingPrivately));
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency 'IMixinRequiringAllMembersRequirements' (required by "
                                                                           + "mixin(s) 'Remotion.UnitTests.Mixins.SampleTypes.MixinRequiringAllMembersFace' applied to class "
                                                                           + "'Remotion.UnitTests.Mixins.Definitions.TestDomain.RequiredMethodDefinitionBuilding.ClassFulfillingInternally') is not fulfilled - public "
                                                                           + "or protected method 'Void Method()' could not be found on the target class.")]
    public void ThrowsIfRequiredMethodIsInternal ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingInternally> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingInternally));
      }
    }

    [Test]
    public void WorksIfRequiredMethodIsProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingProtectedly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingProtectedly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, "");
      }
    }

    [Test]
    public void WorksIfExplicitlyImplemented ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersFace)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsExplicitly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, typeof (IMixinRequiringAllMembersRequirements).FullName + ".");
      }
    }

    [Test]
    public void NoRequiredMethodsWhenFaceRequirementIsClass ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithStaticMethod));
      RequiredFaceTypeDefinition requirement = targetClass.RequiredFaceTypes[typeof (ClassWithStaticMethod)];
      Assert.AreEqual (0, requirement.Methods.Count);
    }
  }
}