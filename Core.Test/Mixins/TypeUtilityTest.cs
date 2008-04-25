using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class TypeUtilityTest
  {
    [Test]
    public void IsGeneratedConcreteMixedType ()
    {
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (typeof (object)));
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (typeof (string)));
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (typeof (int)));
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (typeof (BaseType1)));

      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (TypeUtility.GetConcreteMixedType (typeof (object))));
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (TypeUtility.GetConcreteMixedType (typeof (string))));
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (TypeUtility.GetConcreteMixedType (typeof (int))));
      Assert.IsTrue (TypeUtility.IsGeneratedConcreteMixedType (TypeUtility.GetConcreteMixedType (typeof (BaseType1))));

      Assert.IsTrue (TypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
      Assert.IsTrue (TypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnBaseCallProxy ()
    {
      Type baseCallProxy = MixinReflector.GetBaseCallProxyType (ObjectFactory.Create<BaseType1>().With());
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (baseCallProxy));
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnGeneratedMixinType ()
    {
      ClassOverridingMixinMembers mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType();
      Assert.IsFalse (TypeUtility.IsGeneratedConcreteMixedType (mixinType));
    }

    [Test]
    public void IsGeneratedByMixinEngine ()
    {
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (typeof (object)));
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (typeof (string)));
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (typeof (int)));
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (typeof (BaseType1)));

      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (TypeUtility.GetConcreteMixedType (typeof (object))));
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (TypeUtility.GetConcreteMixedType (typeof (string))));
      Assert.IsFalse (TypeUtility.IsGeneratedByMixinEngine (TypeUtility.GetConcreteMixedType (typeof (int))));
      Assert.IsTrue (TypeUtility.IsGeneratedByMixinEngine (TypeUtility.GetConcreteMixedType (typeof (BaseType1))));

      Assert.IsTrue (TypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
      Assert.IsTrue (TypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void IsGeneratedByMixinEngine_OnBaseCallProxy ()
    {
      Type baseCallProxy = MixinReflector.GetBaseCallProxyType (ObjectFactory.Create<BaseType1> ().With ());
      Assert.IsTrue (TypeUtility.IsGeneratedByMixinEngine (baseCallProxy));
    }

    [Test]
    public void IsIsGeneratedByMixinEngine_OnGeneratedMixinType ()
    {
      ClassOverridingMixinMembers mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType ();
      Assert.IsTrue (TypeUtility.IsGeneratedByMixinEngine (mixinType));
    }

    [Test]
    public void GetConcreteTypeStandardType ()
    {
      Assert.AreSame (typeof (object), TypeUtility.GetConcreteMixedType (typeof (object)));
      Assert.AreSame (typeof (int), TypeUtility.GetConcreteMixedType (typeof (int)));
      Assert.AreSame (typeof (List<int>), TypeUtility.GetConcreteMixedType (typeof (List<int>)));
      Assert.AreSame (typeof (List<>), TypeUtility.GetConcreteMixedType (typeof (List<>)));
    }

    [Test]
    public void GetConcreteTypeMixedTypes ()
    {
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType1)), TypeUtility.GetConcreteMixedType (typeof (BaseType1)));
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType2)), TypeUtility.GetConcreteMixedType (typeof (BaseType2)));
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.AreNotSame (typeof (NullTarget), TypeUtility.GetConcreteMixedType (typeof (NullTarget)));
        Assert.AreSame (TypeFactory.GetConcreteType (typeof (NullTarget)), TypeUtility.GetConcreteMixedType (typeof (NullTarget)));
      }
    }

    [Test]
    public void GetConcreteTypeConcreteType ()
    {
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType1)), TypeUtility.GetConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void IsAssignableStandardTypeToObject ()
    {
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (object)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (List<int>)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (int)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (string)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (List<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToInterface ()
    {
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (object)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<int>)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (int)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (string)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToOpenGenericInterface ()
    {
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (object)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (List<int>)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (int)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (string)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (List<>)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (IList<>), typeof (IList<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToBaseClass ()
    {
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (ValueType), typeof (object)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (ValueType), typeof (List<int>)));
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (ValueType), typeof (int)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (ValueType), typeof (string)));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (ValueType), typeof (List<>)));
    }

    [Test]
    public void IsAssignableMixedTypeToObject ()
    {
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (object), typeof (BaseType1)));
    }

    [Test]
    public void IsAssignableObjectToMixedType ()
    {
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void IsAssignableTypeToMixedType ()
    {
      Type mixedType = CreateMixedType (typeof (NullTarget), typeof (NullMixin));
      Assert.IsFalse (TypeUtility.IsAssignableFrom (mixedType, typeof (NullTarget)));
    }

    [Test]
    public void IsAssignableMixedTypeToType ()
    {
      Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (NullTarget), CreateMixedType (typeof (NullTarget), typeof (NullMixin))));
    }

    [Test]
    public void IsAssignableRightParameterIsMadeConcrete ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (NullTarget), typeof (NullTarget)));
        Assert.IsTrue (TypeUtility.IsAssignableFrom (CreateMixedType (typeof (NullTarget), typeof (NullMixin)), typeof (NullTarget)));
      }
    }

    [Test]
    public void IsAssignableInterfaceImplementedViaMixin ()
    {
      Assert.IsFalse (TypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)));
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIntroducingInheritedInterface)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)));
      }
    }

    [Test]
    public void HasMixinsOnSimpleTypes ()
    {
      Assert.IsFalse (TypeUtility.HasMixins (typeof (object)));
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        Assert.IsFalse (TypeUtility.HasMixins (typeof (BaseType1)));
      }
    }

    [Test]
    public void HasMixinsOnMixedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasMixins (typeof (BaseType1)));
    }

    [Test]
    public void HasMixinsOnMixedTypesWithoutMixins ()
    {
      using (MixinConfiguration.BuildNew().ForClass<object>().EnterScope())
      {
        Assert.IsFalse (TypeUtility.HasMixins (typeof (object)));
      }
    }

    [Test]
    public void HasMixinsOnGeneratedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasMixins (TypeUtility.GetConcreteMixedType (typeof (BaseType1))));
      Assert.IsFalse (TypeUtility.HasMixins (TypeUtility.GetConcreteMixedType (typeof (object))));
    }

    [Test]
    public void HasMixinsOnGeneratedTypesWithoutMixins ()
    {
      Assert.IsFalse (TypeUtility.HasMixins (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void HasMixinOnUnmixedTypes ()
    {
      Assert.IsFalse (TypeUtility.HasMixin (typeof (object), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (int), typeof (object)));
    }

    [Test]
    public void HasMixinOnMixedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.IsTrue (TypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.IsFalse (TypeUtility.HasMixin (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void HasMixinOnGeneratedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.IsTrue (TypeUtility.HasMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.IsFalse (TypeUtility.HasMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void GetAscribableMixinOnUnmixedTypes ()
    {
      Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (object), typeof (NullMixin)));
      Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (int), typeof (object)));
      Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (int), typeof (List<>)));
      Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (int), typeof (List<int>)));
    }

    public class GenericMixin<T>
    {
    }

    [Test]
    public void GetAscribableMixinTypeOnMixedTypes ()
    {
      Assert.AreSame (typeof (BT1Mixin1), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.AreSame (typeof (BT1Mixin2), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.AreSame (typeof (BT1Mixin1), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<>), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<int>), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.AreSame (typeof (GenericMixin<int>), TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      Assert.IsNotNull (TypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void GetAscribableMixinTypeOnGeneratedTypes ()
    {
      Assert.AreSame (typeof (BT1Mixin1), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.AreSame (typeof (BT1Mixin2), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.AreSame (typeof (BT1Mixin1), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<>), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<int>), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.AreSame (typeof (GenericMixin<int>), TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsNull (TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      Assert.IsNotNull (TypeUtility.GetAscribableMixinType (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void HasAscribableMixinOnUnmixedTypes ()
    {
      Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (object), typeof (NullMixin)));
      Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (int), typeof (object)));
      Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (int), typeof (List<>)));
      Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (int), typeof (List<int>)));
    }

    [Test]
    public void HasAscribableMixinOnMixedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (IBT1Mixin1)));
      Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)));
      using (MixinConfiguration.BuildFromActive()
          .ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .EnterScope())
      {
        Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)));
        Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      Assert.IsTrue (TypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void HasAscribableMixinOnGeneratedTypes ()
    {
      Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsFalse (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      Assert.IsTrue (TypeUtility.HasAscribableMixin (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void GetMixinTypesOnUnmixedTypes ()
    {
      Assert.That (new List<Type> (TypeUtility.GetMixinTypes (typeof (object))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (TypeUtility.GetMixinTypes (typeof (int))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (TypeUtility.GetMixinTypes (typeof (List<int>))), Is.EquivalentTo (new Type[0]));
    }

    [Test]
    public void GetMixinTypesOnMixedTypes ()
    {
      Assert.That (new List<Type> (TypeUtility.GetMixinTypes (typeof (BaseType1))),
          Is.EquivalentTo (new Type[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void GetMixinTypesOnGeneratedTypes ()
    {
      Assert.That (new List<Type> (TypeUtility.GetMixinTypes (TypeUtility.GetConcreteMixedType (typeof (BaseType1)))),
          Is.EquivalentTo (new Type[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void CreateInstanceUnmixedTypes ()
    {
      Assert.AreSame (typeof (object), TypeUtility.CreateInstance (typeof (object)).GetType());
      Assert.AreSame (typeof (int), TypeUtility.CreateInstance (typeof (int)).GetType ());
    }

    [Test]
    public void CreateInstanceMixedTypes ()
    {
      Assert.AreNotSame (typeof (BaseType1), TypeUtility.CreateInstance (typeof (BaseType1)).GetType());
      Assert.AreSame (TypeUtility.GetConcreteMixedType (typeof (BaseType1)), TypeUtility.CreateInstance (typeof (BaseType1)).GetType());
    }

    [Test]
    public void CreateInstanceConcreteType()
    {
      Assert.AreSame (TypeUtility.GetConcreteMixedType (typeof (BaseType1)),
          TypeUtility.CreateInstance (TypeUtility.GetConcreteMixedType (typeof (BaseType1))).GetType ());
    }

    [Test]
    public void CreateInstanceWithCtorArgs ()
    {
      List<int> instance = (List<int>) TypeUtility.CreateInstance (typeof (List<int>), 51);
      Assert.AreEqual (51, instance.Capacity);
    }

    [Test]
    public void GetUnderlyingTargetTypeOnMixedType ()
    {
      Assert.AreSame (typeof (BaseType1), TypeUtility.GetUnderlyingTargetType (typeof (BaseType1)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnUnmixedType ()
    {
      Assert.AreSame (typeof (object), TypeUtility.GetUnderlyingTargetType (typeof (object)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnConcreteType ()
    {
      Assert.AreSame (typeof (BaseType1), TypeUtility.GetUnderlyingTargetType (TypeUtility.GetConcreteMixedType (typeof (BaseType1))));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnDerivedConcreteType ()
    {
      Type concreteType = TypeUtility.GetConcreteMixedType (typeof (BaseType1));
      CustomClassEmitter customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType();
      Assert.AreSame (typeof (BaseType1), TypeUtility.GetUnderlyingTargetType (derivedType));
    }

    [Test]
    public void GetMixinConfigurationFromConcreteType ()
    {
      Type bt1Type = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreEqual (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).ConfigurationContext,
          MixinReflector.GetClassContextFromConcreteType (bt1Type));
    }

    [Test]
    public void GetMixinConfigurationFromConcreteTypeNullWhenNoMixedType ()
    {
      Assert.IsNull (MixinReflector.GetClassContextFromConcreteType (typeof (object)));
    }

    [Test]
    public void GetMixinConfigurationFromDerivedConcreteType ()
    {
      Type concreteType = TypeUtility.GetConcreteMixedType (typeof (BaseType1));
      CustomClassEmitter customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType ();
      Assert.AreEqual (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).ConfigurationContext,
          MixinReflector.GetClassContextFromConcreteType (derivedType));
    }

    private Type CreateMixedType (Type baseType, params Type[] types)
    {
      using (MixinConfiguration.BuildNew ().ForClass (baseType).AddMixins (types).EnterScope ())
      {
        return TypeFactory.GetConcreteType (baseType);
      }
    }
  }
}