// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class DependencyDefinitionBuilderTest
  {
    [Test]
    public void FaceInterfaces ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));

      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType33)));
      Assert.IsFalse (targetClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType2)));

      List<MixinDefinition> requirers = new List<MixinDefinition> (targetClass.RequiredFaceTypes[typeof (IBaseType31)].FindRequiringMixins());
      Assert.Contains (targetClass.Mixins[typeof (BT3Mixin1)], requirers);
      Assert.Contains (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin6<,>)), requirers);
      Assert.AreEqual (2, requirers.Count);

      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IBaseType31)].IsEmptyInterface);
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IBaseType31)].IsAggregatorInterface);

      targetClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      requirers = new List<MixinDefinition> (targetClass.RequiredFaceTypes[typeof (ICBaseType3BT3Mixin4)].FindRequiringMixins());
      Assert.Contains (targetClass.Mixins[typeof (BT3Mixin7Face)], requirers);

      requirers = new List<MixinDefinition> (targetClass.RequiredFaceTypes[typeof (BaseType3)].FindRequiringMixins ());
      Assert.Contains (targetClass.Mixins[typeof (BT3Mixin4)], requirers);
      Assert.AreEqual (1, requirers.Count);
    }

    [Test]
    public void FaceInterfacesWithOpenGenericTypes ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));

      Assert.IsFalse (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).ThisDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsFalse (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).ThisDependencies.ContainsKey (typeof (IBaseType33)));
      Assert.IsTrue (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)).ThisDependencies.ContainsKey (typeof (BaseType3)));
    }

    [Test]
    public void FaceInterfacesAddedViaContext ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType6));

      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin1)), "This is added via a dependency of BT6Mixin3.");
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin2)), "This is added via a dependency of BT6Mixin3.");
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin3)), "This is added because of the CompleteInterfaceAttribute.");
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IBT3Mixin4 (mixins Remotion.UnitTests.Mixins.SampleTypes.BT3Mixin7Face applied to class "
                          + "Remotion.UnitTests.Mixins.SampleTypes.BaseType3) is not fulfilled - public or protected method Foo could not be found on the base class.")]
    public void ThrowsIfAggregateThisDependencyIsNotFullyImplemented ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Face));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IBT3Mixin4 (mixins Remotion.UnitTests.Mixins.SampleTypes.BT3Mixin7Base applied to class "
                          + "Remotion.UnitTests.Mixins.SampleTypes.BaseType3) is not fulfilled - public or protected method Foo could not be found on the base class.")]
    public void ThrowsIfAggregateBaseDependencyIsNotFullyImplemented ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    public void BaseInterfaces ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));

      List<Type> requiredBaseCallTypes = new List<RequiredBaseCallTypeDefinition> (targetClass.RequiredBaseCallTypes).ConvertAll<Type>
          (delegate (RequiredBaseCallTypeDefinition def) { return def.Type; });
      Assert.Contains (typeof (IBaseType31), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType33), requiredBaseCallTypes);
      Assert.Contains (typeof (IBaseType34), requiredBaseCallTypes);
      Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType35)));

      List<MixinDefinition> requirers = new List<MixinDefinition> (targetClass.RequiredBaseCallTypes[typeof (IBaseType33)].FindRequiringMixins());
      Assert.Contains (targetClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
    }

    [Test]
    public void BaseMethods ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));

      RequiredBaseCallTypeDefinition req1 = targetClass.RequiredBaseCallTypes[typeof (IBaseType31)];
      Assert.AreEqual (typeof (IBaseType31).GetMembers().Length, req1.Methods.Count);

      RequiredMethodDefinition member1 = req1.Methods[typeof (IBaseType31).GetMethod ("IfcMethod")];
      Assert.AreEqual ("Remotion.UnitTests.Mixins.SampleTypes.IBaseType31.IfcMethod", member1.FullName);
      Assert.AreSame (req1, member1.DeclaringRequirement);
      Assert.AreSame (req1, member1.Parent);

      Assert.AreEqual (typeof (IBaseType31).GetMethod ("IfcMethod"), member1.InterfaceMethod);
      Assert.AreEqual (targetClass.Methods[typeof (BaseType3).GetMethod ("IfcMethod")], member1.ImplementingMethod);

      RequiredBaseCallTypeDefinition req2 = targetClass.RequiredBaseCallTypes[typeof (IBT3Mixin4)];
      Assert.AreEqual (typeof (IBT3Mixin4).GetMembers().Length, req2.Methods.Count);

      RequiredMethodDefinition member2 = req2.Methods[typeof (IBT3Mixin4).GetMethod ("Foo")];
      Assert.AreEqual ("Remotion.UnitTests.Mixins.SampleTypes.IBT3Mixin4.Foo", member2.FullName);
      Assert.AreSame (req2, member2.DeclaringRequirement);
      Assert.AreSame (req2, member2.Parent);

      Assert.AreEqual (typeof (IBT3Mixin4).GetMethod ("Foo"), member2.InterfaceMethod);
      Assert.AreEqual (targetClass.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member2.ImplementingMethod);

      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        TargetClassDefinition targetClass2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3));

        RequiredBaseCallTypeDefinition req3 = targetClass2.RequiredBaseCallTypes[typeof (ICBaseType3BT3Mixin4)];
        Assert.AreEqual (0, req3.Methods.Count);

        req3 = targetClass2.RequiredBaseCallTypes[typeof (ICBaseType3)];
        Assert.AreEqual (0, req3.Methods.Count);

        req3 = targetClass2.RequiredBaseCallTypes[typeof (IBaseType31)];
        Assert.AreEqual (1, req3.Methods.Count);

        req3 = targetClass2.RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.AreEqual (1, req3.Methods.Count);

        RequiredMethodDefinition member3 = req3.Methods[typeof (IBT3Mixin4).GetMethod ("Foo")];
        Assert.IsNotNull (member3);
        Assert.AreEqual (targetClass2.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member3.ImplementingMethod);
      }
    }

    [Test]
    public void DuckTypingFaceInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckFaceMixin> ().Clear().AddMixins (typeof (DuckFaceMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseTypeWithDuckFaceMixin));
        Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (DuckFaceMixin)));
        MixinDefinition mixin = targetClass.Mixins[typeof (DuckFaceMixin)];
        Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IDuckFaceRequirements)));
        Assert.IsTrue (new List<MixinDefinition> (targetClass.RequiredFaceTypes[typeof (IDuckFaceRequirements)].FindRequiringMixins ()).Contains (mixin));

        Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IDuckFaceRequirements)));
        Assert.AreSame (targetClass, mixin.ThisDependencies[typeof (IDuckFaceRequirements)].GetImplementer ());

        Assert.AreSame (mixin, mixin.ThisDependencies[typeof (IDuckFaceRequirements)].Depender);
        Assert.IsNull (mixin.ThisDependencies[typeof (IDuckFaceRequirements)].Aggregator);
        Assert.AreEqual (0, mixin.ThisDependencies[typeof (IDuckFaceRequirements)].AggregatedDependencies.Count);

        Assert.AreSame (targetClass.RequiredFaceTypes[typeof (IDuckFaceRequirements)],
            mixin.ThisDependencies[typeof (IDuckFaceRequirements)].RequiredType);

        Assert.AreEqual (2, targetClass.RequiredFaceTypes[typeof (IDuckFaceRequirements)].Methods.Count);
        Assert.AreSame (typeof (IDuckFaceRequirements).GetMethod ("MethodImplementedOnBase"),
            targetClass.RequiredFaceTypes[typeof (IDuckFaceRequirements)].Methods[0].InterfaceMethod);
        Assert.AreSame (targetClass.Methods[typeof (BaseTypeWithDuckFaceMixin).GetMethod ("MethodImplementedOnBase")],
            targetClass.RequiredFaceTypes[typeof (IDuckFaceRequirements)].Methods[0].ImplementingMethod);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "is not fulfilled - public or protected method MethodImplementedOnBase could not be found", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenUnfulfilledDuckFace()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (DuckFaceMixinWithoutOverrides)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [Test]
    public void DuckTypingBaseInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckBaseMixin> ().Clear().AddMixins (typeof (DuckBaseMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseTypeWithDuckBaseMixin));
        Assert.IsTrue (targetClass.Mixins.ContainsKey (typeof (DuckBaseMixin)));
        MixinDefinition mixin = targetClass.Mixins[typeof (DuckBaseMixin)];
        Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IDuckBaseRequirements)));
        Assert.IsTrue (new List<MixinDefinition> (targetClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].FindRequiringMixins()).Contains (mixin));

        Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IDuckBaseRequirements)));
        Assert.AreSame (targetClass, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].GetImplementer());

        Assert.AreSame (mixin, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].Depender);
        Assert.IsNull (mixin.BaseDependencies[typeof (IDuckBaseRequirements)].Aggregator);
        Assert.AreEqual (0, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].AggregatedDependencies.Count);

        Assert.AreSame (targetClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)],
            mixin.BaseDependencies[typeof (IDuckBaseRequirements)].RequiredType);

        Assert.AreEqual (2, targetClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].Methods.Count);
        Assert.AreSame (typeof (IDuckBaseRequirements).GetMethod ("MethodImplementedOnBase"),
            targetClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].Methods[0].InterfaceMethod);
        Assert.AreSame (targetClass.Methods[typeof (BaseTypeWithDuckBaseMixin).GetMethod ("MethodImplementedOnBase")],
            targetClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].Methods[0].ImplementingMethod);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "is not fulfilled - public or protected method MethodImplementedOnBase could not be found", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenUnfulfilledDuckBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (DuckBaseMixinWithoutOverrides)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget));
      }
    }

    [Test]
    public void Dependencies ()
    {
      MixinDefinition bt3Mixin1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];

      Assert.IsTrue (bt3Mixin1.ThisDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.ThisDependencies.Count);

      Assert.IsTrue (bt3Mixin1.BaseDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.AreEqual (1, bt3Mixin1.BaseDependencies.Count);

      MixinDefinition bt3Mixin2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
      Assert.IsTrue (bt3Mixin2.ThisDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.AreEqual (1, bt3Mixin2.ThisDependencies.Count);

      Assert.AreEqual (0, bt3Mixin2.BaseDependencies.Count);

      MixinDefinition bt3Mixin6 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin6<,>));

      Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType33)));
      Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType34)));

      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.ThisDependencies[typeof (IBaseType31)].AggregatedDependencies.Count);

      Assert.IsTrue (
          bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
              bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.AreEqual (
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).RequiredFaceTypes[typeof (IBaseType31)],
          bt3Mixin6.ThisDependencies[typeof (IBaseType31)].RequiredType);

      Assert.AreSame (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)), bt3Mixin6.ThisDependencies[typeof (IBaseType32)].GetImplementer());
      Assert.AreSame (
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin4)],
          bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].GetImplementer());

      Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType34)));
      Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBT3Mixin4)));
      Assert.IsFalse (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType31)));
      Assert.IsFalse (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType32)));
      Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType33)), "indirect dependency");

      Assert.AreEqual (
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes[typeof (IBaseType34)],
          bt3Mixin6.BaseDependencies[typeof (IBaseType34)].RequiredType);

      Assert.AreSame (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)), bt3Mixin6.BaseDependencies[typeof (IBaseType34)].GetImplementer());
      Assert.AreSame (
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin4)],
          bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].GetImplementer());

      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregate);

      Assert.AreEqual (0, bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].AggregatedDependencies.Count);

      Assert.IsTrue (
          bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
              bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)]));
      Assert.IsNull (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].Aggregator);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency .* is not fulfilled",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfBaseDependencyNotFulfilled ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Base call dependencies must be interfaces.*MixinWithClassBase",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfRequiredBaseIsNotInterface ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithClassBase));
    }

    [Test]
    public void WorksIfRequiredBaseIsSystemObject ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithObjectBase));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForFace ()
    {
      TargetClassDefinition bt3 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7Face)];

      ThisDependencyDefinition d1 = m7.ThisDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d1.GetImplementer ());
      Assert.AreEqual ("Remotion.UnitTests.Mixins.SampleTypes.ICBaseType3BT3Mixin4", d1.FullName);
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

      Assert.IsTrue (bt3.RequiredFaceTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredFaceTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForBase ()
    {
      TargetClassDefinition bt3 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7Base)];

      BaseDependencyDefinition d2 = m7.BaseDependencies[typeof (ICBaseType3BT3Mixin4)];
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

      Assert.IsTrue (bt3.RequiredBaseCallTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredBaseCallTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    [Test]
    public void EmptyInterface()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinWithEmptyInterface), typeof (MixinRequiringEmptyInterface)).EnterScope())
      {
        TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithEmptyInterface)];
        MixinDefinition m2 = bt1.Mixins[typeof (MixinRequiringEmptyInterface)];
        BaseDependencyDefinition dependency = m2.BaseDependencies[0];
        RequiredBaseCallTypeDefinition requirement = dependency.RequiredType;
        Assert.IsTrue (requirement.IsEmptyInterface);
        Assert.IsFalse (requirement.IsAggregatorInterface);
        Assert.AreSame (m1, dependency.GetImplementer());
      }
    }

    [Test]
    public void IndirectThisDependencies ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithIndirectRequirements)];
      Assert.IsNotNull (mixin);

      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectThisAggregator)));
      Assert.IsTrue (targetClass.RequiredFaceTypes[typeof (IIndirectThisAggregator)].IsAggregatorInterface);
      
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IIndirectRequirement1)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IIndirectRequirementBase1)].IsAggregatorInterface);

      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (targetClass.RequiredFaceTypes[typeof (IIndirectRequirement2)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IIndirectRequirementBase2)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredFaceTypes[typeof (IIndirectRequirementBase2)].IsEmptyInterface);

      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (targetClass.RequiredFaceTypes[typeof (IIndirectRequirement3)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (IIndirectRequirementBase3)));
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IIndirectRequirementBase3)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (IIndirectRequirementBase3)].IsEmptyInterface);

      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectThisAggregator)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (IIndirectRequirementBase3)));
    }

    [Test]
    public void IndirectBaseDependencies ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithIndirectRequirements)];
      Assert.IsNotNull (mixin);

      Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectBaseAggregator)));
      Assert.IsTrue (targetClass.RequiredBaseCallTypes[typeof (IIndirectBaseAggregator)].IsAggregatorInterface);

      Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (targetClass.RequiredBaseCallTypes[typeof (IIndirectRequirement1)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (targetClass.RequiredBaseCallTypes[typeof (IIndirectRequirementBase1)].IsAggregatorInterface);

      Assert.IsFalse (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsFalse (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirementBase2)));

      Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (targetClass.RequiredBaseCallTypes[typeof (IIndirectRequirement3)].IsAggregatorInterface);
      Assert.IsTrue (targetClass.RequiredBaseCallTypes.ContainsKey (typeof (IIndirectRequirementBase3)));
      Assert.IsFalse (targetClass.RequiredBaseCallTypes[typeof (IIndirectRequirementBase3)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredBaseCallTypes[typeof (IIndirectRequirementBase3)].IsEmptyInterface);

      Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IIndirectBaseAggregator)));
      Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirement1)));
      Assert.IsFalse (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirement2)));
      Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirement3)));
      Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirementBase1)));
      Assert.IsFalse (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirementBase2)));
      Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IIndirectRequirementBase3)));
    }

    [Test]
    public void NoIndirectDependenciesForClassFaces ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassImplementingInternalInterface));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithClassFaceImplementingInternalInterface)];
      Assert.IsNotNull (mixin);
      Assert.IsTrue (targetClass.RequiredFaceTypes.ContainsKey (typeof (ClassImplementingInternalInterface)));
      Assert.IsFalse (targetClass.RequiredFaceTypes[typeof (ClassImplementingInternalInterface)].IsAggregatorInterface);
      Assert.IsFalse (targetClass.RequiredFaceTypes.ContainsKey (typeof (IInternalInterface1)));
      Assert.IsFalse (targetClass.RequiredFaceTypes.ContainsKey (typeof (IInternalInterface2)));

      Assert.IsTrue (mixin.ThisDependencies.ContainsKey (typeof (ClassImplementingInternalInterface)));
      Assert.IsFalse (mixin.ThisDependencies.ContainsKey (typeof (IInternalInterface1)));
      Assert.IsFalse (mixin.ThisDependencies.ContainsKey (typeof (IInternalInterface2)));
    }

    [Test]
    public void ExplicitDependenciesToInterfaceTypes ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithAdditionalInterfaceDependency)];
      Assert.IsTrue (mixin.MixinDependencies.ContainsKey (typeof (IMixinWithAdditionalClassDependency)));
      Assert.AreSame (targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)],
          mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].GetImplementer ());
      
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].Depender);
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].Parent);

      Assert.IsTrue (targetClass.RequiredMixinTypes.ContainsKey (typeof (IMixinWithAdditionalClassDependency)));
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof (IMixinWithAdditionalClassDependency)];
      Assert.That (requirement.FindRequiringMixins (), List.Contains (mixin));
      Assert.IsTrue (requirement.RequiringDependencies.ContainsKey (mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)]));
      
      Assert.AreEqual(0, requirement.Methods.Count, "mixin type requirements do not contain method requirements");

      Assert.AreSame (requirement, mixin.MixinDependencies[typeof (IMixinWithAdditionalClassDependency)].RequiredType);
    }

    [Test]
    public void ExplicitDependenciesToClassTypes ()
    {
      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)];
      Assert.IsTrue (mixin.MixinDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
      Assert.AreSame (targetClass.Mixins[typeof (MixinWithNoAdditionalDependency)], mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].GetImplementer ());

      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].Depender);
      Assert.AreSame (mixin, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].Parent);

      Assert.IsTrue (targetClass.RequiredMixinTypes.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof (MixinWithNoAdditionalDependency)];
      Assert.That (requirement.FindRequiringMixins (), List.Contains (mixin));
      Assert.IsTrue (requirement.RequiringDependencies.ContainsKey (mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)]));

      Assert.AreEqual (0, requirement.Methods.Count, "mixin type requirements do not contain method requirements");

      Assert.AreSame (requirement, mixin.MixinDependencies[typeof (MixinWithNoAdditionalDependency)].RequiredType);
    }
  }
}
