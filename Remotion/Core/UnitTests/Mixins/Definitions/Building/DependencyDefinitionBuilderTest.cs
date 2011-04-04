// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.Definitions.Building
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

      List<MixinDefinition> requirers = new List<MixinDefinition> (targetClass.RequiredTargetCallTypes[typeof (IBaseType31)].FindRequiringMixins());
      Assert.Contains (targetClass.Mixins[typeof (BT3Mixin1)], requirers);
      Assert.Contains (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin6<,>)), requirers);
      Assert.AreEqual (2, requirers.Count);

      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IBaseType31)].IsEmptyInterface);
      Assert.IsFalse (targetClass.RequiredTargetCallTypes[typeof (IBaseType31)].IsAggregatorInterface);

      targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (Bt3Mixin7TargetCall));
      Assert.IsTrue (targetClass.RequiredTargetCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      requirers = new List<MixinDefinition> (targetClass.RequiredTargetCallTypes[typeof (ICBaseType3BT3Mixin4)].FindRequiringMixins());
      Assert.Contains (targetClass.Mixins[typeof (Bt3Mixin7TargetCall)], requirers);

      requirers = new List<MixinDefinition> (targetClass.RequiredTargetCallTypes[typeof (BaseType3)].FindRequiringMixins ());
      Assert.Contains (targetClass.Mixins[typeof (BT3Mixin4)], requirers);
      Assert.AreEqual (1, requirers.Count);
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
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency 'IBT3Mixin4' (required by mixin(s) 'Remotion.UnitTests.Mixins.TestDomain.Bt3Mixin7TargetCall' applied to class "
                          + "'Remotion.UnitTests.Mixins.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
                          + "could not be found on the target class.")]
    public void ThrowsIfAggregateTargetCallDependencyIsNotFullyImplemented ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (Bt3Mixin7TargetCall));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency 'IBT3Mixin4' (required by mixin(s) 'Remotion.UnitTests.Mixins.TestDomain.BT3Mixin7Base' applied to class "
                          + "'Remotion.UnitTests.Mixins.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
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

      List<MixinDefinition> requirers = new List<MixinDefinition> (targetClass.RequiredNextCallTypes[typeof (IBaseType33)].FindRequiringMixins());
      Assert.Contains (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
    }

    [Test]
    public void BaseMethods ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));

      RequiredNextCallTypeDefinition req1 = targetClass.RequiredNextCallTypes[typeof (IBaseType31)];
      Assert.AreEqual (typeof (IBaseType31).GetMembers().Length, req1.Methods.Count);

      RequiredMethodDefinition member1 = req1.Methods[typeof (IBaseType31).GetMethod ("IfcMethod")];
      Assert.AreEqual ("Remotion.UnitTests.Mixins.TestDomain.IBaseType31.IfcMethod", member1.FullName);
      Assert.AreSame (req1, member1.DeclaringRequirement);
      Assert.AreSame (req1, member1.Parent);

      Assert.AreEqual (typeof (IBaseType31).GetMethod ("IfcMethod"), member1.InterfaceMethod);
      Assert.AreEqual (targetClass.Methods[typeof (BaseType3).GetMethod ("IfcMethod")], member1.ImplementingMethod);

      RequiredNextCallTypeDefinition req2 = targetClass.RequiredNextCallTypes[typeof (IBT3Mixin4)];
      Assert.AreEqual (typeof (IBT3Mixin4).GetMembers().Length, req2.Methods.Count);

      RequiredMethodDefinition member2 = req2.Methods[typeof (IBT3Mixin4).GetMethod ("Foo")];
      Assert.AreEqual ("Remotion.UnitTests.Mixins.TestDomain.IBT3Mixin4.Foo", member2.FullName);
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
        Assert.IsTrue (new List<MixinDefinition> (targetClass.RequiredTargetCallTypes[typeof (IDuckTargetCallRequirements)].FindRequiringMixins ()).Contains (mixin));

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
        Assert.IsTrue (new List<MixinDefinition> (targetClass.RequiredNextCallTypes[typeof (IDuckBaseRequirements)].FindRequiringMixins()).Contains (mixin));

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
      Assert.AreEqual ("Remotion.UnitTests.Mixins.TestDomain.ICBaseType3BT3Mixin4", d1.FullName);
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
      Assert.That (requirement.FindRequiringMixins (), Has.Member(mixin));
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
      Assert.That (requirement.FindRequiringMixins (), Has.Member(mixin));
      Assert.IsTrue (requirement.RequiringDependencies.ContainsKey (mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)]));

      Assert.AreEqual (0, requirement.Methods.Count, "mixin type requirements do not contain method requirements");

      Assert.AreSame (requirement, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].RequiredType);
    }
  }
}
