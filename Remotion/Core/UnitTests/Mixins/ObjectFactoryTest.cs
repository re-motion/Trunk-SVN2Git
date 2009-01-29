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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class ObjectFactoryTest
  {
    [Test]
    public void MixedObjectsCanBeCreated ()
    {
      object o = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      Assert.IsNotNull (o);
      Assert.IsTrue (o is BaseType3);
      Assert.IsTrue (o is IMixinTarget);

      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3), ParamList.Empty);
      Assert.IsNotNull (o);
      Assert.IsTrue (o is IMixinTarget);
      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedWithMixinInstances ()
    {
      BT1Mixin1 m1 = new BT1Mixin1 ();
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, m1);

      Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
      Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
      Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
      Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
    }

    [Test]
    public void MixedObjectsWithMixinInstancesCanBeCreatedFromType ()
    {
      BT1Mixin1 m1 = new BT1Mixin1 ();
      BaseType1 bt1 = (BaseType1) ObjectFactory.Create (typeof (BaseType1), ParamList.Empty, m1);

      Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
      Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
      Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
      Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstances ()
    {
      BT2Mixin1 m1 = new BT2Mixin1 ();
      ObjectFactory.Create<BaseType3> (ParamList.Empty, m1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstancesWithType ()
    {
      BT2Mixin1 m1 = new BT2Mixin1 ();
      ObjectFactory.Create (typeof (BaseType3), ParamList.Empty, m1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin Remotion.UnitTests.Mixins.SampleTypes.MixinWithProtectedOverrider "
        + "applied to base type Remotion.UnitTests.Mixins.SampleTypes.BaseType1 needs to have a subclass generated at runtime. It is therefore not "
            + "possible to use the given object of type MixinWithProtectedOverrider as a mixin instance.", MatchType = MessageMatch.Contains)]
    public void ThrowsOnBaseMixinInstanceWhenGeneratedTypeIsNeeded ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinWithProtectedOverrider)).EnterScope())
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, new MixinWithProtectedOverrider ());
        bt1.VirtualMethod ();
      }
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType1 ()
    {
      TargetClassDefinition configuration = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers));
      Type generatedMixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (configuration.Mixins[typeof (MixinWithAbstractMembers)]).GeneratedType;
      object mixinInstance = Activator.CreateInstance (generatedMixinType);

      ClassOverridingMixinMembers classInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty, mixinInstance);
      Assert.AreSame (mixinInstance, Mixin.Get<MixinWithAbstractMembers> (classInstance));
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType2 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinWithProtectedOverrider)).EnterScope())
      {
        TargetClassDefinition configuration = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
        Type mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (configuration.Mixins[0]).GeneratedType;
        object mixinInstance = Activator.CreateInstance (mixinType);
        BaseType1 bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, mixinInstance);
        bt1.VirtualMethod ();
        Assert.AreSame (mixinInstance, Mixin.Get<MixinWithProtectedOverrider> (bt1));
      }
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin2)).EnterScope())
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
      }
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin1)).EnterScope())
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
        Assert.IsNotNull (mixin.Base);
        Assert.AreNotSame (bt3, mixin.Base);
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedByMixins ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Face), typeof (BT3Mixin4)).EnterScope())
      {
        ICBaseType3BT3Mixin4 complete = ObjectFactory.Create<BaseType3>(ParamList.Empty) as ICBaseType3BT3Mixin4;

        Assert.IsNotNull (complete);
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType33) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType34) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod2", complete.IfcMethod2());
        Assert.AreEqual ("BaseType3.IfcMethod-BT3Mixin4.Foo", Mixin.Get<BT3Mixin7Face> (complete).InvokeThisMethods());
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedExplicitly ()
    {
      object complete = ObjectFactory.Create<BaseType6> (ParamList.Empty);

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgument ()
    {
      ICBT6Mixin1 complete = ObjectFactory.Create<ICBT6Mixin1> (ParamList.Empty);

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    public interface IEmptyInterface { }

    [Test]
    public void CompleteFaceInterfaceAddedImperativelyAsTypeArgument ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass (typeof (BaseType6)).AddCompleteInterface (typeof (IEmptyInterface))
          .EnterScope ())
      {
        IEmptyInterface complete = ObjectFactory.Create<IEmptyInterface> (ParamList.Empty);

        Assert.IsNotNull (complete);
        Assert.IsTrue (complete is BaseType6);
        Assert.IsTrue (complete is IEmptyInterface);
      }
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgumentWithMixins ()
    {
      ICBT6Mixin1 complete = ObjectFactory.Create<ICBT6Mixin1> (ParamList.Empty, new BT6Mixin1 ());

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Contains)]
    public void InterfaceAsTypeArgumentWithoutCompleteness ()
    {
      ObjectFactory.Create<IBaseType2> (ParamList.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Regex)]
    public void InterfaceAsTypeArgumentWithoutCompletenessWithMixins ()
    {
      ObjectFactory.Create<IBaseType2> (ParamList.Empty);
    }

    [Test]
    public void MixinWithoutPublicCtor ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        MixinWithPrivateCtorAndVirtualMethod mixin = MixinWithPrivateCtorAndVirtualMethod.Create ();
        object o = ObjectFactory.Create<NullTarget> (ParamList.Empty, mixin);
        Assert.IsNotNull (o);
        Assert.IsNotNull (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o));
        Assert.AreSame (mixin, Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o));
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Cannot instantiate mixin Remotion.UnitTests.Mixins."
        + "Validation.ValidationSampleTypes.MixinWithPrivateCtorAndVirtualMethod, there is no visible default constructor.")]
    public void ThrowsWhenMixinWithoutPublicDefaultCtorShouldBeInstantiated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    [Test]
    public void GenerationPolicyOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.AreEqual (typeof (object), o.GetType ());

      o = ObjectFactory.Create<object> (ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.AreEqual (typeof (object), o.GetType ());
    }

    [Test]
    public void GenerationPolicyForce ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty, GenerationPolicy.ForceGeneration);
      Assert.AreNotEqual (typeof (object), o.GetType ());
      Assert.AreEqual (typeof (object), o.GetType ().BaseType);

      o = ObjectFactory.Create<object> (ParamList.Empty, GenerationPolicy.ForceGeneration);
      Assert.AreNotEqual (typeof (object), o.GetType ());
      Assert.AreEqual (typeof (object), o.GetType ().BaseType);
    }

    [Test]
    public void DefaultPolicyIsOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty);
      Assert.AreEqual (typeof (object), o.GetType ());

      o = ObjectFactory.Create<object> (ParamList.Empty);
      Assert.AreEqual (typeof (object), o.GetType ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin instances "
        + "must be specified.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnMixinInstancesWhenNoGeneration ()
    {
      ObjectFactory.Create (typeof (object), ParamList.Empty, new object ());
    }

    public class MixinThrowingInOnInitialized : Mixin<object>
    {
      protected override void OnInitialized ()
      {
        throw new NotSupportedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ExceptionPropagated_WhenMixinOnInitializedThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinThrowingInOnInitialized)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    public class MixinThrowingInCtor : Mixin<object>
    {
      public MixinThrowingInCtor()
      {
        throw new NotSupportedException ();
      }
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void TargetInvocationExceptionWhenMixinCtorThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinThrowingInCtor)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    public class TargetClassWithProtectedCtors
    {
      protected TargetClassWithProtectedCtors ()
      {
      }

      protected TargetClassWithProtectedCtors (int i)
      {
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
        + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
        + "not set).")]
    public void ProtectedDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
        + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
        + "not set).")]
    public void ProtectedNonDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
        + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
        + "not set).")]
    public void ProtectedDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
        + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
        + "not set).")]
    public void ProtectedNonDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<TargetClassWithProtectedCtors> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsNotNull (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<TargetClassWithProtectedCtors> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsNotNull (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)));
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Assert.IsNotNull (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        Assert.IsNotNull (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)));
      }
    }
  }
}
