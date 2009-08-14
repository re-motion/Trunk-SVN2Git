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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class ObjectFactoryTest
  {
    public interface IEmptyInterface
    {
    }

    public class MixinThrowingInOnInitialized : Mixin<object>
    {
      protected override void OnInitialized ()
      {
        throw new NotSupportedException();
      }
    }

    public class MixinThrowingInCtor : Mixin<object>
    {
      public MixinThrowingInCtor ()
      {
        throw new NotSupportedException();
      }
    }

    public class TargetClassWithProtectedCtors
    {
      protected TargetClassWithProtectedCtors ()
      {
      }

      protected TargetClassWithProtectedCtors (int i)
      {
        Dev.Null = i;
      }
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType_OverriddenMixinMembers ()
    {
      Type generatedMixinType = GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      object mixinInstance = Activator.CreateInstance (generatedMixinType);

      var classInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty, mixinInstance);
      Assert.That (Mixin.Get<MixinWithAbstractMembers> (classInstance), Is.SameAs (mixinInstance));
    }

    [Test]
    public void AcceptsInstanceOfGeneratedMixinType_WrappedProtectedMixinMembers ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins (typeof (MixinWithProtectedOverrider)).EnterScope())
      {
        Type generatedMixinType = GetGeneratedMixinType (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
        object mixinInstance = Activator.CreateInstance (generatedMixinType);
        var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, mixinInstance);
        bt1.VirtualMethod();
        Assert.That (Mixin.Get<MixinWithProtectedOverrider> (bt1), Is.SameAs (mixinInstance));
      }
    }

    [Test]
    public void CompleteFaceInterfaceAddedImperativelyAsTypeArgument ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass (typeof (BaseType6)).AddCompleteInterface (typeof (IEmptyInterface))
          .EnterScope())
      {
        var complete = ObjectFactory.Create<IEmptyInterface> (ParamList.Empty);

        Assert.That (complete, Is.Not.Null);
        Assert.That (complete is BaseType6, Is.True);
      }
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgument ()
    {
      var complete = ObjectFactory.Create<ICBT6Mixin1> (ParamList.Empty);

      Assert.That (complete, Is.Not.Null);
      Assert.That (complete is BaseType6, Is.True);
      Assert.That (complete is ICBT6Mixin2, Is.True);
      Assert.That (complete is ICBT6Mixin3, Is.True);
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgumentWithMixins ()
    {
      var complete = ObjectFactory.Create<ICBT6Mixin1> (ParamList.Empty, new BT6Mixin1());

      Assert.That (complete, Is.Not.Null);
      Assert.That (complete is BaseType6, Is.True);
      Assert.That (complete is ICBT6Mixin2, Is.True);
      Assert.That (complete is ICBT6Mixin3, Is.True);
    }

    [Test]
    public void CompleteFaceInterfacesAddedByMixins ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (BT3Mixin7Face), typeof (BT3Mixin4)).EnterScope())
      {
        var complete = (ICBaseType3BT3Mixin4) ObjectFactory.Create<BaseType3> (ParamList.Empty);

        Assert.That (((IBaseType33) complete).IfcMethod(), Is.EqualTo ("BaseType3.IfcMethod"));
        Assert.That (((IBaseType34) complete).IfcMethod(), Is.EqualTo ("BaseType3.IfcMethod"));
        Assert.That (complete.IfcMethod2(), Is.EqualTo ("BaseType3.IfcMethod2"));
        Assert.That (Mixin.Get<BT3Mixin7Face> (complete).InvokeThisMethods(), Is.EqualTo ("BaseType3.IfcMethod-BT3Mixin4.Foo"));
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedExplicitly ()
    {
      object complete = ObjectFactory.Create<BaseType6> (ParamList.Empty);

      Assert.That (complete, Is.Not.Null);
      Assert.That (complete is BaseType6, Is.True);
      Assert.That (complete is ICBT6Mixin1, Is.True);
      Assert.That (complete is ICBT6Mixin2, Is.True);
      Assert.That (complete is ICBT6Mixin3, Is.True);
    }

    [Test]
    public void DefaultPolicyIsOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));

      o = ObjectFactory.Create<object> (ParamList.Empty);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ExceptionPropagated_WhenMixinOnInitializedThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinThrowingInOnInitialized)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    [Test]
    public void GenerationPolicyForce ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty, GenerationPolicy.ForceGeneration);
      Assert.That (o.GetType(), Is.Not.EqualTo (typeof (object)));
      Assert.That (o.GetType().BaseType, Is.EqualTo (typeof (object)));

      o = ObjectFactory.Create<object> (ParamList.Empty, GenerationPolicy.ForceGeneration);
      Assert.That (o.GetType(), Is.Not.EqualTo (typeof (object)));
      Assert.That (o.GetType().BaseType, Is.EqualTo (typeof (object)));
    }

    [Test]
    public void GenerationPolicyOnlyIfNecessary ()
    {
      object o = ObjectFactory.Create (typeof (object), ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));

      o = ObjectFactory.Create<object> (ParamList.Empty, GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.That (o.GetType(), Is.EqualTo (typeof (object)));
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
    public void MixedObjectsCanBeCreated ()
    {
      object o = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      Assert.That (o, Is.Not.Null);
      Assert.That (o is BaseType3, Is.True);
      Assert.That (o is IMixinTarget, Is.True);

      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3), ParamList.Empty);
      Assert.That (o, Is.Not.Null);
      Assert.That (o is IMixinTarget, Is.True);
      Assert.That (((IMixinTarget) o).Mixins[0], Is.Not.Null);
    }

    [Test]
    public void MixedObjectsCanBeCreatedWithMixinInstances ()
    {
      var m1 = new BT1Mixin1();
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty, m1);

      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.SameAs (m1));
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.SameAs (m1));
    }

    [Test]
    public void MixedObjectsWithMixinInstancesCanBeCreatedFromType ()
    {
      var m1 = new BT1Mixin1();
      var bt1 = (BaseType1) ObjectFactory.Create (typeof (BaseType1), ParamList.Empty, m1);

      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin1> (bt1), Is.SameAs (m1));
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.Null);
      Assert.That (Mixin.Get<BT1Mixin2> (bt1), Is.Not.SameAs (m1));
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (BT3Mixin1)).EnterScope())
      {
        var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        var mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.That (mixin, Is.Not.Null);
        Assert.That (mixin.This, Is.SameAs (bt3));
        Assert.That (mixin.Base, Is.Not.Null);
        Assert.That (mixin.Base, Is.Not.SameAs (bt3));
      }
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins (typeof (BT3Mixin2)).EnterScope())
      {
        var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
        var mixin = Mixin.Get<BT3Mixin2> (bt3);
        Assert.That (mixin, Is.Not.Null);
        Assert.That (mixin.This, Is.SameAs (bt3));
      }
    }

    [Test]
    public void MixinWithoutPublicCtor ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        MixinWithPrivateCtorAndVirtualMethod mixin = MixinWithPrivateCtorAndVirtualMethod.Create();
        object o = ObjectFactory.Create<NullTarget> (ParamList.Empty, mixin);
        Assert.That (o, Is.Not.Null);
        Assert.That (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o), Is.Not.Null);
        Assert.That (Mixin.Get<MixinWithPrivateCtorAndVirtualMethod> (o), Is.SameAs (mixin));
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Empty);
      }
    }

    [Test]
    public void ProtectedDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Empty), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedNonDefaultConstructor_Mixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_Mixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassWithProtectedCtors>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.ObjectFactoryTest+"
       + "TargetClassWithProtectedCtors contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is "
       + "not set).")]
    public void ProtectedNonDefaultConstructor_NonMixed ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        ObjectFactory.Create<TargetClassWithProtectedCtors> (ParamList.Create (1));
      }
    }

    [Test]
    public void ProtectedNonDefaultConstructor_NonMixed_AllowProtected ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.That (ObjectFactory.Create<TargetClassWithProtectedCtors> (true, ParamList.Create (1)), Is.Not.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void TargetInvocationExceptionWhenMixinCtorThrows ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinThrowingInCtor)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no mixin configuration for type System.Object, so no mixin instances "
                                                                      + "must be specified.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnMixinInstancesWhenNoGeneration ()
    {
      ObjectFactory.Create (typeof (object), ParamList.Empty, new object());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The supplied mixin of type "
        + "'Remotion.UnitTests.Mixins.SampleTypes.BT2Mixin1' is not valid for target type 'Remotion.UnitTests.Mixins.SampleTypes.BaseType3' in the "
        + "current configuration.")]
    public void ThrowsOnWrongMixinInstances ()
    {
      var m1 = new BT2Mixin1();
      ObjectFactory.Create<BaseType3> (ParamList.Empty, m1);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Cannot instantiate mixin "
       + "'Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes.MixinWithPrivateCtorAndVirtualMethod' applied to class "
       + "'Remotion.UnitTests.Mixins.SampleTypes.NullTarget', there is no visible default constructor.")]
    public void ThrowsWhenMixinWithoutPublicDefaultCtorShouldBeInstantiated ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinWithPrivateCtorAndVirtualMethod)).EnterScope())
      {
        ObjectFactory.Create<NullTarget> (ParamList.Empty);
      }
    }

    private Type GetGeneratedMixinType (Type targetType, Type mixinType)
    {
      ClassContext requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (
          targetType,
          GenerationPolicy.GenerateOnlyIfConfigured);

      MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[mixinType];
      Assert.That (mixinDefinition, Is.Not.Null);

      return ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier ()).GeneratedType;
    }

  }
}