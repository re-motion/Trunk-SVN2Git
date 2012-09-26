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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class DependencyDefinitionBuilderTest
  {
    [Test]
    public void FaceInterfaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IBaseType33)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IBaseType2)));

      CheckAllRequiringEntities (
          targetClass.RequiredTargetCallTypes[typeof (IBaseType31)], 
          targetClass.Mixins[typeof (BT3Mixin1)], targetClass.GetMixinByConfiguredType (typeof (BT3Mixin6<,>)));

      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IBaseType31)].IsEmptyInterface);
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IBaseType31)].IsAggregatorInterface);

      targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (Bt3Mixin7TargetCall));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));

      CheckAllRequiringEntities (
          targetClass.RequiredTargetCallTypes[typeof (ICBaseType3BT3Mixin4)],
          targetClass.Mixins[typeof (Bt3Mixin7TargetCall)]);
      CheckAllRequiringEntities (
          targetClass.RequiredTargetCallTypes[typeof (BaseType3)],
          targetClass.Mixins[typeof (BT3Mixin4)]);
    }

    [Test]
    public void FaceInterfacesWithOpenGenericTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

      Assert.IsFalse (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).TargetCallDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsFalse (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).TargetCallDependencies.ContainsKey (typeof (IBaseType33)));
      Assert.IsTrue (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).TargetCallDependencies.ContainsKey (typeof (BaseType3)));
    }

    [Test]
    public void FaceInterfacesAddedViaContext ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType6));

      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ICBT6Mixin1)), "This is added via a dependency of BT6Mixin3.");
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ICBT6Mixin2)), "This is added via a dependency of BT6Mixin3.");
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ICBT6Mixin3)), "This is added because of the CompleteInterfaceAttribute.");
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = 
        "The dependency 'IBT3Mixin4' (required by mixin 'Remotion.Mixins.UnitTests.Core.TestDomain.Bt3Mixin7TargetCall' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
        + "could not be found on the target class.")]
    public void ThrowsIfAggregateTargetCallDependencyIsNotFullyImplemented ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (Bt3Mixin7TargetCall));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IBT3Mixin4' (required by mixin 'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin7Base' on class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
        + "could not be found on the target class.")]
    public void ThrowsIfAggregateNextCallDependencyIsNotFullyImplemented ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    public void BaseInterfaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

      List<Type> requiredNextCallTypes = new List<RequiredNextCallTypeDefinition> (targetClass.RequiredNextCallTypes).ConvertAll<Type>
          (delegate (RequiredNextCallTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredNextCallTypes);
      Assert.Contains (typeof (IBaseType33), requiredNextCallTypes);
      Assert.Contains (typeof (IBaseType34), requiredNextCallTypes);
      Assert.IsFalse (requiredNextCallTypes.Contains (typeof (IBaseType35)));

      CheckSomeRequiringMixin (
          targetClass.RequiredNextCallTypes[typeof (IBaseType33)],
          targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)));
    }

    [Test]
    public void BaseMethods ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

      RequiredNextCallTypeDefinition req1 = targetClass.RequiredNextCallTypes[typeof (IBaseType31)];
      Assert.AreEqual (typeof (IBaseType31).GetMembers().Length, req1.Methods.Count);

      RequiredMethodDefinition member1 = req1.Methods[typeof (IBaseType31).GetMethod ("IfcMethod")];
      Assert.AreEqual ("Remotion.Mixins.UnitTests.Core.TestDomain.IBaseType31.IfcMethod", member1.FullName);
      Assert.AreSame (req1, member1.DeclaringRequirement);
      Assert.AreSame (req1, member1.Parent);

      Assert.AreEqual (typeof (IBaseType31).GetMethod ("IfcMethod"), member1.InterfaceMethod);
      Assert.AreEqual (targetClass.Methods[typeof (BaseType3).GetMethod ("IfcMethod")], member1.ImplementingMethod);

      RequiredNextCallTypeDefinition req2 = targetClass.RequiredNextCallTypes[typeof (IBT3Mixin4)];
      Assert.AreEqual (typeof (IBT3Mixin4).GetMembers().Length, req2.Methods.Count);

      RequiredMethodDefinition member2 = req2.Methods[typeof (IBT3Mixin4).GetMethod ("Foo")];
      Assert.AreEqual ("Remotion.Mixins.UnitTests.Core.TestDomain.IBT3Mixin4.Foo", member2.FullName);
      Assert.AreSame (req2, member2.DeclaringRequirement);
      Assert.AreSame (req2, member2.Parent);

      Assert.AreEqual (typeof (IBT3Mixin4).GetMethod ("Foo"), member2.InterfaceMethod);
      Assert.AreEqual (targetClass.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member2.ImplementingMethod);

      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        TargetClassDefinition targetClass2 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

        RequiredNextCallTypeDefinition req3 = targetClass2.RequiredNextCallTypes[typeof (ICBaseType3BT3Mixin4)];
        Assert.AreEqual (0, req3.Methods.Count);

        req3 = targetClass2.RequiredNextCallTypes[typeof (ICBaseType3)];
        Assert.AreEqual (0, req3.Methods.Count);

        req3 = targetClass2.RequiredNextCallTypes[typeof (IBaseType31)];
        Assert.AreEqual (1, req3.Methods.Count);

        req3 = targetClass2.RequiredNextCallTypes[typeof (IBT3Mixin4)];
        Assert.AreEqual (1, req3.Methods.Count);

        RequiredMethodDefinition member3 = req3.Methods[typeof (IBT3Mixin4).GetMethod ("Foo")];
        Assert.IsNotNull (member3);
        Assert.AreEqual (targetClass2.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member3.ImplementingMethod);
      }
    }

    [Test]
    public void DuckTypingFaceInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckTargetCallMixin> ().Clear().AddMixins (typeof (DuckTargetCallMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseTypeWithDuckTargetCallMixin));
        Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (DuckTargetCallMixin)));
        MixinDefinition mixin = targetClass.Mixins[typeof (DuckTargetCallMixin)];
        Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IDuckTargetCallRequirements)));
        CheckAllRequiringEntities (
            targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)],
            mixin);

        Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IDuckTargetCallRequirements)));
        Assert.AreSame (targetClass, mixin.TargetCallDependencies[typeof (IDuckTargetCallRequirements)].GetImplementer ());

        Assert.AreSame (mixin, mixin.TargetCallDependencies[typeof (IDuckTargetCallRequirements)].Depender);
        Assert.IsNull (mixin.TargetCallDependencies[typeof (IDuckTargetCallRequirements)].Aggregator);
        Assert.AreEqual (0, mixin.TargetCallDependencies[typeof (IDuckTargetCallRequirements)].AggregatedDependencies.Count);

        Assert.AreSame (targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)],
                        mixin.TargetCallDependencies[typeof (IDuckTargetCallRequirements)].RequiredType);

        Assert.AreEqual (2, targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)].Methods.Count);
        Assert.AreSame (typeof (IDuckTargetCallRequirements).GetMethod ("MethodImplementedOnBase"),
                        targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)].Methods[0].InterfaceMethod);
        Assert.AreSame (targetClass.Methods[typeof (BaseTypeWithDuckTargetCallMixin).GetMethod ("MethodImplementedOnBase")],
                        targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)].Methods[0].ImplementingMethod);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "is not fulfilled - public or protected method 'System.String MethodImplementedOnBase()' could not be found", 
        MatchType = MessageMatch.Contains)]
    public void ThrowsWhenUnfulfilledDuckTargetCall()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (DuckTargetCallMixinWithoutOverrides)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
      }
    }

    [Test]
    public void DuckTypingBaseInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckBaseMixin> ().Clear().AddMixins (typeof (DuckBaseMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseTypeWithDuckBaseMixin));
        Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (DuckBaseMixin)));
        MixinDefinition mixin = targetClass.Mixins[typeof (DuckBaseMixin)];
        Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IDuckBaseRequirements)));
        CheckAllRequiringEntities (
            targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)],
            mixin);

        Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IDuckBaseRequirements)));
        Assert.AreSame (targetClass, mixin.NextCallDependencies[typeof (IDuckBaseRequirements)].GetImplementer());

        Assert.AreSame (mixin, mixin.NextCallDependencies[typeof (IDuckBaseRequirements)].Depender);
        Assert.IsNull (mixin.NextCallDependencies[typeof (IDuckBaseRequirements)].Aggregator);
        Assert.AreEqual (0, mixin.NextCallDependencies[typeof (IDuckBaseRequirements)].AggregatedDependencies.Count);

        Assert.AreSame (targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)],
                        mixin.NextCallDependencies[typeof (IDuckBaseRequirements)].RequiredType);

        Assert.AreEqual (2, targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)].Methods.Count);
        Assert.AreSame (typeof (IDuckBaseRequirements).GetMethod ("MethodImplementedOnBase"),
                        targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)].Methods[0].InterfaceMethod);
        Assert.AreSame (targetClass.Methods[typeof (BaseTypeWithDuckBaseMixin).GetMethod ("MethodImplementedOnBase")],
                        targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)].Methods[0].ImplementingMethod);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "is not fulfilled - public or protected method 'System.String MethodImplementedOnBase()' could not be found", 
        MatchType = MessageMatch.Contains)]
    public void ThrowsWhenUnfulfilledDuckBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (DuckBaseMixinWithoutOverrides)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget));
      }
    }

    [Test]
    public void Dependencies ()
    {
      MixinDefinition bt3Mixin1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];

      Assert.IsTrue (bt3Mixin1.TargetCallDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.TargetCallDependencies.Count);

      Assert.IsTrue (bt3Mixin1.NextCallDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.NextCallDependencies.Count);

      MixinDefinition bt3Mixin2 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (bt3Mixin2.TargetCallDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.AreEqual (1, bt3Mixin2.TargetCallDependencies.Count);

      Assert.AreEqual (0, bt3Mixin2.NextCallDependencies.Count);

      MixinDefinition bt3Mixin6 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin6<,>));

      Assert.IsTrue (bt3Mixin6.TargetCallDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3Mixin6.TargetCallDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.TargetCallDependencies.ContainsKey (typeof (IBaseType33)));
      Assert.IsTrue (bt3Mixin6.TargetCallDependencies.ContainsKey (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.TargetCallDependencies.ContainsKey (typeof (IBaseType34)));

      Assert.IsFalse (bt3Mixin6.TargetCallDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.TargetCallDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.TargetCallDependencies[typeof (IBaseType31)].AggregatedDependencies.Count);

      Assert.IsTrue (
          bt3Mixin6.TargetCallDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
              bt3Mixin6.TargetCallDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.TargetCallDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.AreSame (
          bt3Mixin6.TargetClass.RequiredTargetCallTypes[typeof (IBaseType31)],
          bt3Mixin6.TargetCallDependencies[typeof (IBaseType31)].RequiredType);

      Assert.AreSame (bt3Mixin6.TargetClass, bt3Mixin6.TargetCallDependencies[typeof (IBaseType32)].GetImplementer ());
      Assert.AreSame (bt3Mixin6.TargetClass.Mixins[typeof (BT3Mixin4)], bt3Mixin6.TargetCallDependencies[typeof (IBT3Mixin4)].GetImplementer());

      Assert.IsTrue (bt3Mixin6.NextCallDependencies.ContainsKey (typeof (IBaseType34)));
      Assert.IsTrue (bt3Mixin6.NextCallDependencies.ContainsKey (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.NextCallDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsFalse (bt3Mixin6.NextCallDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.NextCallDependencies.ContainsKey (typeof (IBaseType33)), "indirect dependency");

      Assert.AreSame (bt3Mixin6.TargetClass.RequiredNextCallTypes[typeof (IBaseType34)], bt3Mixin6.NextCallDependencies[typeof (IBaseType34)].RequiredType);

      Assert.AreSame (bt3Mixin6.TargetClass, bt3Mixin6.NextCallDependencies[typeof (IBaseType34)].GetImplementer());
      Assert.AreSame (bt3Mixin6.TargetClass.Mixins[typeof (BT3Mixin4)], bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].GetImplementer());

      Assert.IsFalse (bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].AggregatedDependencies.Count);

      Assert.IsTrue (
          bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
              bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.NextCallDependencies[typeof (IBT3Mixin4)].Aggregator);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency .* is not fulfilled",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfNextCallDependencyNotFulfilled ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Next call dependencies must be interfaces.*MixinWithClassBase",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfRequiredBaseIsNotInterface ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithClassBase));
    }

    [Test]
    public void WorksIfRequiredBaseIsSystemObject ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithObjectBase));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForFace ()
    {
      TargetClassDefinition bt3 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (Bt3Mixin7TargetCall));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (Bt3Mixin7TargetCall)];

      TargetCallDependencyDefinition d1 = m7.TargetCallDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d1.GetImplementer ());
      Assert.AreEqual ("Remotion.Mixins.UnitTests.Core.TestDomain.ICBaseType3BT3Mixin4", d1.FullName);
      Assert.AreSame (m7, d1.Parent);

      Assert.IsTrue (d1.IsAggregate);
      Assert.IsTrue (d1.AggregatedDependencies[typeof (ICBaseType3)].IsAggregate);
      Assert.IsFalse (d1.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d1.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer ());

      Assert.IsFalse (d1.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d1.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.AreSame (d1, d1.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (bt3.RequiredTargetCallTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredTargetCallTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredTargetCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredTargetCallTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredTargetCallTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredTargetCallTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForBase ()
    {
      TargetClassDefinition bt3 =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7Base)];

      NextCallDependencyDefinition d2 = m7.NextCallDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d2.GetImplementer ());

      Assert.IsTrue (d2.IsAggregate);

      Assert.IsTrue (d2.AggregatedDependencies[typeof (ICBaseType3)].IsAggregate);
      Assert.AreSame (d2, d2.AggregatedDependencies[typeof (ICBaseType3)].Parent);

      Assert.IsFalse (d2.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d2.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer ());

      Assert.IsFalse (d2.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d2.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.AreSame (d2, d2.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (bt3.RequiredNextCallTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredNextCallTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredNextCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredNextCallTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredNextCallTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredNextCallTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    [Test]
    public void CompleteInterfaces ()
    {
      TargetClassDefinition bt3 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), Type.EmptyTypes, new[] { typeof (ICBaseType3) });

      var dependency = bt3.CompleteInterfaceDependencies[typeof (ICBaseType3)];
      Assert.That (dependency, Is.Not.Null);

      var requirement = dependency.RequiredType;
      Assert.That (requirement, Is.Not.Null);
      Assert.That (requirement, Is.SameAs (bt3.RequiredTargetCallTypes[typeof (ICBaseType3)]));
      Assert.That (requirement.RequiringDependencies, Is.EqualTo (new[] { dependency }));

      Assert.That (dependency, Is.TypeOf<CompleteInterfaceDependencyDefinition> ());
      Assert.That (dependency.RequiredType, Is.SameAs (requirement));
      Assert.That (dependency.TargetClass, Is.SameAs (bt3));
      Assert.That (dependency.Depender, Is.SameAs (bt3));
      Assert.That (dependency.FullName, Is.EqualTo (typeof (ICBaseType3).FullName));
      Assert.That (dependency.Parent, Is.SameAs (dependency.Depender));
      Assert.That (dependency.CompleteInterface, Is.SameAs (typeof (ICBaseType3)));

      CheckSomeRequiringCompleteInterface (requirement, typeof (ICBaseType3));
      
      Assert.That (dependency.IsAggregate, Is.True);
      Assert.That (dependency.AggregatedDependencies[typeof (IBaseType31)], Is.Not.Null);
      Assert.That (dependency.AggregatedDependencies[typeof (IBaseType31)], Is.TypeOf<CompleteInterfaceDependencyDefinition> ());
      Assert.That (
          ((CompleteInterfaceDependencyDefinition) dependency.AggregatedDependencies[typeof (IBaseType31)]).CompleteInterface, 
          Is.SameAs (typeof (ICBaseType3)));
      Assert.That (bt3.RequiredTargetCallTypes[typeof (IBaseType31)], Is.Not.Null);
      Assert.That (bt3.RequiredTargetCallTypes[typeof (IBaseType31)], Is.SameAs (dependency.AggregatedDependencies[typeof (IBaseType31)].RequiredType));
    }

    [Test]
    public void EmptyInterface()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinWithEmptyInterface), typeof (MixinRequiringEmptyInterface)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithEmptyInterface)];
        MixinDefinition m2 = bt1.Mixins[typeof (MixinRequiringEmptyInterface)];
        NextCallDependencyDefinition dependency = m2.NextCallDependencies[0];
        RequiredNextCallTypeDefinition requirement = dependency.RequiredType;
        Assert.IsTrue (requirement.IsEmptyInterface);
        Assert.IsFalse (requirement.IsAggregatorInterface);
        Assert.AreSame (m1, dependency.GetImplementer());
      }
    }

    [Test]
    public void IndirectTargetCallDependencies ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithIndirectRequirements)];
      Assert.IsNotNull (mixin);

      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectTargetAggregator)));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes[typeof (IIndirectTargetAggregator)].IsAggregatorInterface);
      
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirement1)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirementBase1)].IsAggregatorInterface);

      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirement2)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirementBase2)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirementBase2)].IsEmptyInterface);

      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirement3)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IIndirectRequirementBase3)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirementBase3)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IIndirectRequirementBase3)].IsEmptyInterface);

      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectTargetAggregator)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (IIndirectRequirementBase3)));
    }

    [Test]
    public void IndirectNextCallDependencies ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithIndirectRequirements)];
      Assert.IsNotNull (mixin);

      Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectBaseAggregator)));
      Assert.IsTrue (targetClass.RequiredNextCallTypes[typeof (IIndirectBaseAggregator)].IsAggregatorInterface);

      Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (targetClass.RequiredNextCallTypes[typeof (IIndirectRequirement1)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (targetClass.RequiredNextCallTypes[typeof (IIndirectRequirementBase1)].IsAggregatorInterface);

      Assert.IsFalse (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsFalse (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirementBase2)));

      Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (targetClass.RequiredNextCallTypes[typeof (IIndirectRequirement3)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredNextCallTypes.ContainsKey (typeof (IIndirectRequirementBase3)));
      Assert.IsFalse (targetClass.RequiredNextCallTypes[typeof (IIndirectRequirementBase3)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredNextCallTypes[typeof (IIndirectRequirementBase3)].IsEmptyInterface);

      Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectBaseAggregator)));
      Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsTrue (mixin.NextCallDependencies.ContainsKey (typeof (IIndirectRequirementBase3)));
    }

    [Test]
    public void NoIndirectDependenciesForClassFaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassImplementingInternalInterface));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithClassTargetCallImplementingInternalInterface)];
      Assert.IsNotNull (mixin);
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ClassImplementingInternalInterface)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (ClassImplementingInternalInterface)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IInternalInterface1)));
      Assert.IsFalse (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (IInternalInterface2)));

      Assert.IsTrue (mixin.TargetCallDependencies.ContainsKey (typeof (ClassImplementingInternalInterface)));
      Assert.IsFalse (mixin.TargetCallDependencies.ContainsKey (typeof (IInternalInterface1)));
      Assert.IsFalse (mixin.TargetCallDependencies.ContainsKey (typeof (IInternalInterface2)));
    }

    [Test]
    public void ExplicitDependenciesToInterfaceTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithAdditionalInterfaceDependency)];
      Assert.IsTrue (mixin.MixinDependencies.ContainsKey (typeof (IMixinWithAdditionalClassDependency)));
      Assert.AreSame (targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)],
                      mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].GetImplementer ());
      
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].Depender);
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].Parent);

      Assert.IsTrue (targetClass.RequiredMixinTypes.ContainsKey (typeof (IMixinWithAdditionalClassDependency)));
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof (IMixinWithAdditionalClassDependency)];
      CheckAllRequiringEntities (requirement, mixin);

      Assert.IsTrue (requirement.RequiringDependencies.ContainsKey (mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)]));
      
      Assert.AreEqual(0, requirement.Methods.Count, "mixin type requirements do not contain method requirements");

      Assert.AreSame (requirement, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].RequiredType);
    }

    [Test]
    public void ExplicitDependenciesToClassTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)];
      Assert.IsTrue (mixin.MixinDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
      Assert.AreSame (targetClass.Mixins[typeof (MixinWithNoAdditionalDependency)], mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].GetImplementer ());

      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].Depender);
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].Parent);

      Assert.IsTrue (targetClass.RequiredMixinTypes.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof (MixinWithNoAdditionalDependency)];
      CheckAllRequiringEntities (requirement, mixin);

      Assert.IsTrue (requirement.RequiringDependencies.ContainsKey (mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)]));

      Assert.AreEqual (0, requirement.Methods.Count, "mixin type requirements do not contain method requirements");

      Assert.AreSame (requirement, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].RequiredType);
    }

    private void CheckAllRequiringEntities (RequirementDefinitionBase requirement, params MixinDefinition[] expectedRequiringMixins)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("expectedRequiringMixins", expectedRequiringMixins);

      var requiringEntityDescription = requirement.GetRequiringEntityDescription ();
      var requiringEntityDescriptionItems = requiringEntityDescription.Split (new[] { ", "}, StringSplitOptions.None);

      foreach (var mixinDefinition in expectedRequiringMixins)
        CheckSomeRequiringMixin (requirement, mixinDefinition);

      Assert.That (requiringEntityDescriptionItems, Has.Length.EqualTo (expectedRequiringMixins.Length), requiringEntityDescription);
    }

    private void CheckSomeRequiringMixin (RequirementDefinitionBase requirement, MixinDefinition expectedRequiringMixin)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("expectedRequiringMixin", expectedRequiringMixin);

      var requirers = requirement.GetRequiringEntityDescription ().Split (new[] { ", " }, StringSplitOptions.None);

      Assert.That (requirers, Has.Member ("mixin '" + expectedRequiringMixin.FullName + "'"));
    }

    private void CheckSomeRequiringCompleteInterface (RequirementDefinitionBase requirement, Type expectedRequiringCompleteInterface)
    {
      ArgumentUtility.CheckNotNull ("requirement", requirement);
      ArgumentUtility.CheckNotNull ("expectedRequiringCompleteInterface", expectedRequiringCompleteInterface);

      var requirers = requirement.GetRequiringEntityDescription ().Split (new[] { ", " }, StringSplitOptions.None);

      Assert.That (requirers, Has.Member ("complete interface '" + expectedRequiringCompleteInterface.FullName + "'"));
    }
  }
}
