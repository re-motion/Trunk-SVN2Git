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
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinTypeUtilityTest
  {
    [Test]
    public void IsGeneratedConcreteMixedType ()
    {
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (object)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (string)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (int)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (BaseType1)));

      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (object))));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (string))));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (int))));
      Assert.IsTrue (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));

      Assert.IsTrue (MixinTypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
      Assert.IsTrue (MixinTypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnBaseCallProxy ()
    {
      Type baseCallProxy = MixinReflector.GetBaseCallProxyType (ObjectFactory.Create<BaseType1>(ParamList.Empty));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (baseCallProxy));
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnGeneratedMixinType ()
    {
      ClassOverridingMixinMembers mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType();
      Assert.IsFalse (MixinTypeUtility.IsGeneratedConcreteMixedType (mixinType));
    }

    [Test]
    public void IsGeneratedByMixinEngine ()
    {
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (object)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (string)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (int)));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (BaseType1)));

      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (object))));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (string))));
      Assert.IsFalse (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (int))));
      Assert.IsTrue (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));

      Assert.IsTrue (MixinTypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
      Assert.IsTrue (MixinTypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void IsGeneratedByMixinEngine_OnBaseCallProxy ()
    {
      Type baseCallProxy = MixinReflector.GetBaseCallProxyType (ObjectFactory.Create<BaseType1> (ParamList.Empty));
      Assert.IsTrue (MixinTypeUtility.IsGeneratedByMixinEngine (baseCallProxy));
    }

    [Test]
    public void IsIsGeneratedByMixinEngine_OnGeneratedMixinType ()
    {
      ClassOverridingMixinMembers mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType ();
      Assert.IsTrue (MixinTypeUtility.IsGeneratedByMixinEngine (mixinType));
    }

    [Test]
    public void GetConcreteTypeStandardType ()
    {
      Assert.AreSame (typeof (object), MixinTypeUtility.GetConcreteMixedType (typeof (object)));
      Assert.AreSame (typeof (int), MixinTypeUtility.GetConcreteMixedType (typeof (int)));
      Assert.AreSame (typeof (List<int>), MixinTypeUtility.GetConcreteMixedType (typeof (List<int>)));
      Assert.AreSame (typeof (List<>), MixinTypeUtility.GetConcreteMixedType (typeof (List<>)));
    }

    [Test]
    public void GetConcreteTypeMixedTypes ()
    {
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType1)), MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)));
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType2)), MixinTypeUtility.GetConcreteMixedType (typeof (BaseType2)));
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.AreNotSame (typeof (NullTarget), MixinTypeUtility.GetConcreteMixedType (typeof (NullTarget)));
        Assert.AreSame (TypeFactory.GetConcreteType (typeof (NullTarget)), MixinTypeUtility.GetConcreteMixedType (typeof (NullTarget)));
      }
    }

    [Test]
    public void GetConcreteTypeConcreteType ()
    {
      Assert.AreSame (TypeFactory.GetConcreteType (typeof (BaseType1)), MixinTypeUtility.GetConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void IsAssignableStandardTypeToObject ()
    {
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (object)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (List<int>)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (int)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (string)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (List<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToInterface ()
    {
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (object)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<int>)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (int)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (string)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToOpenGenericInterface ()
    {
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (object)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (List<int>)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (int)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (string)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (List<>)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (IList<>)));
    }

    [Test]
    public void IsAssignableStandardTypeToBaseClass ()
    {
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (object)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (List<int>)));
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (int)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (string)));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (List<>)));
    }

    [Test]
    public void IsAssignableMixedTypeToObject ()
    {
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (BaseType1)));
    }

    [Test]
    public void IsAssignableObjectToMixedType ()
    {
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void IsAssignableTypeToMixedType ()
    {
      Type mixedType = CreateMixedType (typeof (NullTarget), typeof (NullMixin));
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (mixedType, typeof (NullTarget)));
    }

    [Test]
    public void IsAssignableMixedTypeToType ()
    {
      Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (NullTarget), CreateMixedType (typeof (NullTarget), typeof (NullMixin))));
    }

    [Test]
    public void IsAssignableRightParameterIsMadeConcrete ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (NullTarget), typeof (NullTarget)));
        Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (CreateMixedType (typeof (NullTarget), typeof (NullMixin)), typeof (NullTarget)));
      }
    }

    [Test]
    public void IsAssignableInterfaceImplementedViaMixin ()
    {
      Assert.IsFalse (MixinTypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)));
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIntroducingInheritedInterface)).EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)));
      }
    }

    [Test]
    public void HasMixinsOnSimpleTypes ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixins (typeof (object)));
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        Assert.IsFalse (MixinTypeUtility.HasMixins (typeof (BaseType1)));
      }
    }

    [Test]
    public void HasMixinsOnMixedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixins (typeof (BaseType1)));
    }

    [Test]
    public void HasMixinsOnMixedTypesWithoutMixins ()
    {
      using (MixinConfiguration.BuildNew().ForClass<object>().EnterScope())
      {
        Assert.IsFalse (MixinTypeUtility.HasMixins (typeof (object)));
      }
    }

    [Test]
    public void HasMixinsOnGeneratedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixins (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));
      Assert.IsFalse (MixinTypeUtility.HasMixins (MixinTypeUtility.GetConcreteMixedType (typeof (object))));
    }

    [Test]
    public void HasMixinsOnGeneratedTypesWithoutMixins ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixins (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void HasMixinOnUnmixedTypes ()
    {
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (object), typeof (NullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (int), typeof (object)));
    }

    [Test]
    public void HasMixinOnMixedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void HasMixinOnGeneratedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.IsTrue (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.IsFalse (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void GetAscribableMixinOnUnmixedTypes ()
    {
      Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (object), typeof (NullMixin)));
      Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (object)));
      Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (List<>)));
      Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (List<int>)));
    }

    public class GenericMixin<T>
    {
    }

    [Test]
    public void GetAscribableMixinTypeOnMixedTypes ()
    {
      Assert.AreSame (typeof (BT1Mixin1), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.AreSame (typeof (BT1Mixin2), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.AreSame (typeof (BT1Mixin1), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<>), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<int>), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.AreSame (typeof (GenericMixin<int>), MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      Assert.IsNotNull (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void GetAscribableMixinTypeOnGeneratedTypes ()
    {
      Assert.AreSame (typeof (BT1Mixin1), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.AreSame (typeof (BT1Mixin2), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.AreSame (typeof (BT1Mixin1), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<>), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.AreSame (typeof (GenericMixin<int>), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.AreSame (typeof (GenericMixin<int>), MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsNull (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      Assert.IsNotNull (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void HasAscribableMixinOnUnmixedTypes ()
    {
      Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (object), typeof (NullMixin)));
      Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (object)));
      Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (List<>)));
      Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (List<int>)));
    }

    [Test]
    public void HasAscribableMixinOnMixedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin2)));
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (IBT1Mixin1)));
      Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)));
      using (MixinConfiguration.BuildFromActive()
          .ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)));
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)));
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)));
      }
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (object)));
    }

    [Test]
    public void HasAscribableMixinOnGeneratedTypes ()
    {
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)));
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)));
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)));
        Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)));
        Assert.IsFalse (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)));
      }
      Assert.IsTrue (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)));
    }

    [Test]
    public void GetMixinTypesOnUnmixedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (object))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (int))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (List<int>))), Is.EquivalentTo (new Type[0]));
    }

    [Test]
    public void GetMixinTypesOnMixedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (BaseType1))),
          Is.EquivalentTo (new Type[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void GetMixinTypesOnGeneratedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)))),
          Is.EquivalentTo (new Type[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void CreateInstanceUnmixedTypes ()
    {
      Assert.AreSame (typeof (object), MixinTypeUtility.CreateInstance (typeof (object)).GetType());
      Assert.AreSame (typeof (int), MixinTypeUtility.CreateInstance (typeof (int)).GetType ());
    }

    [Test]
    public void CreateInstanceMixedTypes ()
    {
      Assert.AreNotSame (typeof (BaseType1), MixinTypeUtility.CreateInstance (typeof (BaseType1)).GetType());
      Assert.AreSame (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), MixinTypeUtility.CreateInstance (typeof (BaseType1)).GetType());
    }

    [Test]
    public void CreateInstanceConcreteType()
    {
      Assert.AreSame (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)),
          MixinTypeUtility.CreateInstance (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))).GetType ());
    }

    [Test]
    public void CreateInstanceWithCtorArgs ()
    {
      List<int> instance = (List<int>) MixinTypeUtility.CreateInstance (typeof (List<int>), 51);
      Assert.AreEqual (51, instance.Capacity);
    }

    [Test]
    public void GetUnderlyingTargetTypeOnMixedType ()
    {
      Assert.AreSame (typeof (BaseType1), MixinTypeUtility.GetUnderlyingTargetType (typeof (BaseType1)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnUnmixedType ()
    {
      Assert.AreSame (typeof (object), MixinTypeUtility.GetUnderlyingTargetType (typeof (object)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnConcreteType ()
    {
      Assert.AreSame (typeof (BaseType1), MixinTypeUtility.GetUnderlyingTargetType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnDerivedConcreteType ()
    {
      Type concreteType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1));
      CustomClassEmitter customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType();
      Assert.AreSame (typeof (BaseType1), MixinTypeUtility.GetUnderlyingTargetType (derivedType));
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
      Type concreteType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1));
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
