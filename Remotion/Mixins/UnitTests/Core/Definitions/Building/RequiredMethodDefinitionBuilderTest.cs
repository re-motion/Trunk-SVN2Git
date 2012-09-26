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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
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
    public void RequiredTargetCallMethodsInterfaceImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersTargetCall)];
      Assert.IsNotNull (mixin);

      RequiredTargetCallTypeDefinition requirement = mixin.TargetCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods(requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredNextCallMethodsInterfaceImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersNextCall)];
      Assert.IsNotNull (mixin);

      RequiredNextCallTypeDefinition requirement = mixin.NextCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredTargetCallMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassFulfillingNoMemberRequirements)).Clear().AddMixins (typeof (MixinRequiringAllMembersTargetCall), typeof (MixinFulfillingAllMemberRequirements)).EnterScope())
      {
        TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersTargetCall)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = TargetClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredTargetCallTypeDefinition requirement = mixin.TargetCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredNextCallMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassFulfillingNoMemberRequirements)).Clear().AddMixins (typeof (MixinRequiringAllMembersNextCall), typeof (MixinFulfillingAllMemberRequirements)).EnterScope())
      {
        TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersNextCall)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = TargetClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredNextCallTypeDefinition requirement = mixin.NextCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredTargetCallMethodsDuckImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersTargetCall)];
      Assert.IsNotNull (mixin);

      RequiredTargetCallTypeDefinition requirement = mixin.TargetCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    public void RequiredNextCallMethodsDuckImplementedOnBase ()
    {
      TargetClassDefinition TargetClassDefinition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = TargetClassDefinition.Mixins[typeof (MixinRequiringAllMembersNextCall)];
      Assert.IsNotNull (mixin);

      RequiredNextCallTypeDefinition requirement = mixin.NextCallDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, TargetClassDefinition, "");
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IRequirement' (required by mixin "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleMethod' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget') is not fulfilled - public or protected method 'Void Method()' could not be found "
        + "on the target class.")]
    public void ThrowsIfMethodRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleMethod)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IRequirement' (required by mixin "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleProperty' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget') is not fulfilled - public or protected method 'Int32 get_Property()' could not be "
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
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = 
        "The dependency 'IRequirement' (required by mixin "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding.MixinRequiringSingleEvent' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget') is not fulfilled - public or protected method "
        + "'Void add_Event(System.EventHandler)' could not be found on the target class.")]
    public void ThrowsIfEventRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinRequiringSingleEvent)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
        Assert.Fail ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IMixinRequiringAllMembersRequirements' (required by mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.MixinRequiringAllMembersTargetCall' on class "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding.ClassFulfillingPrivately') "
        + "is not fulfilled - public or protected method 'Void Method()' could not be found on the target class.")]
    public void ThrowsIfRequiredMethodIsPrivate ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingPrivately> ().Clear().AddMixins (typeof (MixinRequiringAllMembersTargetCall)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingPrivately));
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IMixinRequiringAllMembersRequirements' (required by "
        + "mixin 'Remotion.Mixins.UnitTests.Core.TestDomain.MixinRequiringAllMembersTargetCall' on class "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.RequiredMethodDefinitionBuilding.ClassFulfillingInternally') is not fulfilled - "
        + "public or protected method 'Void Method()' could not be found on the target class.")]
    public void ThrowsIfRequiredMethodIsInternal ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingInternally> ().Clear().AddMixins (typeof (MixinRequiringAllMembersTargetCall)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingInternally));
      }
    }

    public interface IDerivedSimpleInterface : ISimpleInterface {}

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'ISimpleInterface' (required by complete interface "
        + "'Remotion.Mixins.UnitTests.Core.Definitions.Building.RequiredMethodDefinitionBuilderTest+IDerivedSimpleInterface' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget') is not fulfilled - public or protected method 'System.String Method()' "
        + "could not be found on the target class.")]
    public void ThrowsIfDerivedRequiredInterfaceIsNotFullyImplemented ()
    {
      using (MixinConfiguration
          .BuildFromActive ()
          .ForClass<NullTarget> ().Clear ().AddMixin<NullMixin> ().AddCompleteInterface<IDerivedSimpleInterface> ()
          .EnterScope ())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
      }
    }
    
    [Test]
    public void WorksIfRequiredMethodIsProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingProtectedly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersTargetCall)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingProtectedly));
        RequiredTargetCallTypeDefinition requirement = definition.RequiredTargetCallTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, "");
      }
    }

    [Test]
    public void WorksIfExplicitlyImplemented ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassFulfillingAllMemberRequirementsExplicitly> ().Clear().AddMixins (typeof (MixinRequiringAllMembersTargetCall)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassFulfillingAllMemberRequirementsExplicitly));
        RequiredTargetCallTypeDefinition requirement = definition.RequiredTargetCallTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, typeof (IMixinRequiringAllMembersRequirements).FullName + ".");
      }
    }

    [Test]
    public void NoRequiredMethodsWhenFaceRequirementIsClass ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithStaticMethod));
      RequiredTargetCallTypeDefinition requirement = targetClass.RequiredTargetCallTypes[typeof (ClassWithStaticMethod)];
      Assert.AreEqual (0, requirement.Methods.Count);
    }
  }
}
