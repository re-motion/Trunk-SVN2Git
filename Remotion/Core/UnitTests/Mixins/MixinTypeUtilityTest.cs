// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinTypeUtilityTest
  {
    [Test]
    public void IsGeneratedConcreteMixedType ()
    {
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (object)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (string)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (int)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (typeof (BaseType1)), Is.False);

      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (object))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (string))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (int))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))), Is.True);

      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)), Is.True);
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)), Is.True);
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnNextCallProxy ()
    {
      Type NextCallProxy = MixinReflector.GetNextCallProxyType (ObjectFactory.Create<BaseType1>(ParamList.Empty));
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (NextCallProxy), Is.False);
    }

    [Test]
    public void IsGeneratedConcreteMixedType_OnGeneratedMixinType ()
    {
      var mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType();
      Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (mixinType), Is.False);
    }

    [Test]
    public void IsGeneratedByMixinEngine ()
    {
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (object)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (string)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (int)), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (typeof (BaseType1)), Is.False);

      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (object))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (string))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (int))), Is.False);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))), Is.True);

      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)), Is.True);
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration)), Is.True);
    }

    [Test]
    public void IsGeneratedByMixinEngine_OnNextCallProxy ()
    {
      Type NextCallProxy = MixinReflector.GetNextCallProxyType (ObjectFactory.Create<BaseType1> (ParamList.Empty));
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (NextCallProxy), Is.True);
    }

    [Test]
    public void IsIsGeneratedByMixinEngine_OnGeneratedMixinType ()
    {
      var mixedInstance = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);
      Type mixinType = Mixin.Get<MixinWithAbstractMembers> (mixedInstance).GetType ();
      Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (mixinType), Is.True);
    }

    [Test]
    public void GetConcreteTypeStandardType ()
    {
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (object)), Is.SameAs (typeof (object)));
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (int)), Is.SameAs (typeof (int)));
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (List<int>)), Is.SameAs (typeof (List<int>)));
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (List<>)), Is.SameAs (typeof (List<>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    [Ignore ("TODO 1370: Throw.")]
    public void GetConcreteTypeOpenGeneric ()
    {
      MixinTypeUtility.GetConcreteMixedType (typeof (List<>));
    }

    [Test]
    public void GetConcreteTypeMixedTypes ()
    {
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), Is.SameAs (TypeFactory.GetConcreteType (typeof (BaseType1))));
      Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType2)), Is.SameAs (TypeFactory.GetConcreteType (typeof (BaseType2))));
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (NullTarget)), Is.Not.SameAs (typeof (NullTarget)));
        Assert.That (MixinTypeUtility.GetConcreteMixedType (typeof (NullTarget)), Is.SameAs (TypeFactory.GetConcreteType (typeof (NullTarget))));
      }
    }

    [Test]
    public void GetConcreteTypeConcreteType ()
    {
      Assert.That (MixinTypeUtility.GetConcreteMixedType (TypeFactory.GetConcreteType (typeof (BaseType1))), Is.SameAs (TypeFactory.GetConcreteType (typeof (BaseType1))));
    }

    [Test]
    public void IsAssignableStandardTypeToObject ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (object)), Is.True);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (List<int>)), Is.True);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (int)), Is.True);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (string)), Is.True);
    }

    [Test]
    public void IsAssignableStandardTypeToInterface ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (object)), Is.False);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<int>)), Is.True);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (int)), Is.False);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (string)), Is.False);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<int>), typeof (List<>)), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    [Ignore ("TODO 1370: Throw.")]
    public void IsAssignableFromOpenGenericInterface ()
    {
      MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (List<>));
    }

    [Test]
    public void IsAssignableStandardTypeToOpenGenericInterface ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IList<>), typeof (object)), Is.False);
    }

    [Test]
    public void IsAssignableStandardTypeToBaseClass ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (object)), Is.False);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (List<int>)), Is.False);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (int)), Is.True);
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (ValueType), typeof (string)), Is.False);
    }

    [Test]
    public void IsAssignableMixedTypeToObject ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (object), typeof (BaseType1)), Is.True);
    }

    [Test]
    public void IsAssignableObjectToMixedType ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (BaseType1), typeof (object)), Is.False);
    }

    [Test]
    public void IsAssignableTypeToMixedType ()
    {
      Type mixedType = CreateMixedType (typeof (NullTarget), typeof (NullMixin));
      Assert.That (MixinTypeUtility.IsAssignableFrom (mixedType, typeof (NullTarget)), Is.False);
    }

    [Test]
    public void IsAssignableMixedTypeToType ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (NullTarget), CreateMixedType (typeof (NullTarget), typeof (NullMixin))), Is.True);
    }

    [Test]
    public void IsAssignableRightParameterIsMadeConcrete ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (NullTarget), typeof (NullTarget)), Is.True);
        Assert.That (MixinTypeUtility.IsAssignableFrom (CreateMixedType (typeof (NullTarget), typeof (NullMixin)), typeof (NullTarget)), Is.True);
      }
    }

    [Test]
    public void IsAssignableInterfaceImplementedViaMixin ()
    {
      Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)), Is.False);
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinIntroducingInheritedInterface)).EnterScope())
      {
        Assert.That (MixinTypeUtility.IsAssignableFrom (typeof (IMixinIII4), typeof (NullTarget)), Is.True);
      }
    }

    [Test]
    public void HasMixinsOnSimpleTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixins (typeof (object)), Is.False);
      using (MixinConfiguration.BuildNew().EnterScope ())
      {
        Assert.That (MixinTypeUtility.HasMixins (typeof (BaseType1)), Is.False);
      }
    }

    [Test]
    public void HasMixinsOnMixedTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixins (typeof (BaseType1)), Is.True);
    }

    [Test]
    public void HasMixinsOnMixedTypesWithoutMixins ()
    {
      using (MixinConfiguration.BuildNew().ForClass<object>().EnterScope())
      {
        Assert.That (MixinTypeUtility.HasMixins (typeof (object)), Is.False);
      }
    }

    [Test]
    public void HasMixinsOnGeneratedTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixins (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))), Is.True);
      Assert.That (MixinTypeUtility.HasMixins (MixinTypeUtility.GetConcreteMixedType (typeof (object))), Is.False);
    }

    [Test]
    public void HasMixinsOnGeneratedTypesWithoutMixins ()
    {
      Assert.That (MixinTypeUtility.HasMixins (TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration)), Is.False);
    }

    [Test]
    public void HasMixinOnUnmixedTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (object), typeof (NullMixin)), Is.False);
      Assert.That (MixinTypeUtility.HasMixin (typeof (int), typeof (object)), Is.False);
    }

    [Test]
    public void HasMixinOnMixedTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin1)), Is.True);
      Assert.That (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (BT1Mixin2)), Is.True);
      Assert.That (MixinTypeUtility.HasMixin (typeof (BaseType1), typeof (object)), Is.False);
    }

    [Test]
    public void HasMixinOnGeneratedTypes ()
    {
      Assert.That (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)), Is.True);
      Assert.That (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)), Is.True);
      Assert.That (MixinTypeUtility.HasMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)), Is.False);
    }

    [Test]
    public void GetAscribableMixinOnUnmixedTypes ()
    {
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (object), typeof (NullMixin)), Is.Null);
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (object)), Is.Null);
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (List<>)), Is.Null);
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (int), typeof (List<int>)), Is.Null);
    }

    [Test]
    public void GetAscribableMixinTypeOnMixedTypes ()
    {
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin1)), Is.SameAs (typeof (BT1Mixin1)));
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (BT1Mixin2)), Is.SameAs (typeof (BT1Mixin2)));
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (IBT1Mixin1)), Is.SameAs (typeof (BT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)), Is.SameAs (typeof (GenericMixin<>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)), Is.Null);
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)), Is.Null);
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<>)), Is.SameAs (typeof (GenericMixin<int>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<int>)), Is.SameAs (typeof (GenericMixin<int>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (GenericMixin<string>)), Is.Null);
      }
      Assert.That (MixinTypeUtility.GetAscribableMixinType (typeof (BaseType1), typeof (object)), Is.Not.Null);
    }

    [Test]
    public void GetAscribableMixinTypeOnGeneratedTypes ()
    {
      Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)), Is.SameAs (typeof (BT1Mixin1)));
      Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)), Is.SameAs (typeof (BT1Mixin2)));
      Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)), Is.SameAs (typeof (BT1Mixin1)));
      using (MixinConfiguration.BuildFromActive().ForClass<GenericTargetClass<string>> ().Clear().AddMixins (typeof (GenericMixin<>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<>)), Is.SameAs (typeof (GenericMixin<>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<int>)), Is.Null);
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<string>)), Is.Null);
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)), Is.SameAs (typeof (GenericMixin<int>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)), Is.SameAs (typeof (GenericMixin<int>)));
        Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)), Is.Null);
      }
      Assert.That (MixinTypeUtility.GetAscribableMixinType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)), Is.Not.Null);
    }

    [Test]
    public void HasAscribableMixinOnUnmixedTypes ()
    {
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (object), typeof (NullMixin)), Is.False);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (object)), Is.False);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (List<>)), Is.False);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (int), typeof (List<int>)), Is.False);
    }

    [Test]
    public void HasAscribableMixinOnMixedTypes ()
    {
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin1)), Is.True);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (BT1Mixin2)), Is.True);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (IBT1Mixin1)), Is.True);
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)), Is.False);
      using (MixinConfiguration.BuildFromActive()
          .ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<>))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .EnterScope())
      {
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)), Is.False);
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)), Is.False);
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (GenericTargetClass<>), typeof (NullMixin)), Is.True);
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<int>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (GenericMixin<string>)), Is.False);
      }
      Assert.That (MixinTypeUtility.HasAscribableMixin (typeof (BaseType1), typeof (object)), Is.True);
    }

    [Test]
    public void HasAscribableMixinOnGeneratedTypes ()
    {
      Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin1)), Is.True);
      Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (BT1Mixin2)), Is.True);
      Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (IBT1Mixin1)), Is.True);
      using (MixinConfiguration.BuildFromActive ().ForClass<GenericTargetClass<string>> ().Clear ().AddMixins (typeof (GenericMixin<>)).EnterScope ())
      {
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<int>)), Is.False);
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (GenericTargetClass<string>)), typeof (GenericMixin<string>)), Is.False);
      }
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (GenericMixin<int>)).EnterScope())
      {
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<int>)), Is.True);
        Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (GenericMixin<string>)), Is.False);
      }
      Assert.That (MixinTypeUtility.HasAscribableMixin (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)), typeof (object)), Is.True);
    }

    [Test]
    public void GetMixinTypes_OnUnmixedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (object))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (int))), Is.EquivalentTo (new Type[0]));
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (List<int>))), Is.EquivalentTo (new Type[0]));
    }

    [Test]
    public void GetMixinTypes_OnMixedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (typeof (BaseType1))),
          Is.EquivalentTo (new[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void GetMixinTypes_OnGeneratedTypes ()
    {
      Assert.That (new List<Type> (MixinTypeUtility.GetMixinTypes (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1)))),
          Is.EquivalentTo (new[] { typeof (BT1Mixin1), typeof (BT1Mixin2) }));
    }

    [Test]
    public void GetMixinTypesExact_OnUnmixedTypes ()
    {
      Assert.That (MixinTypeUtility.GetMixinTypesExact (typeof (object)), Is.EquivalentTo (new Type[0]));
      Assert.That (MixinTypeUtility.GetMixinTypesExact (typeof (int)), Is.EquivalentTo (new Type[0]));
      Assert.That (MixinTypeUtility.GetMixinTypesExact (typeof (List<int>)), Is.EquivalentTo (new Type[0]));
    }

    [Test]
    public void GetMixinTypesExact_OnMixedTypes ()
    {
      // see MixinDependencySortingIntegrationTest.MixinDefinitionsAreSortedCorrectlySmall

      Assert.That (MixinTypeUtility.GetMixinTypesExact (typeof (BaseType7)), Is.EqualTo (new[] { 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5) }));

      Assert.That (MixinTypeUtility.GetMixinTypesExact (typeof (BaseType3)), Has.Member(typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }

    [Test]
    public void GetMixinTypesExact_OnGeneratedTypes ()
    {
      // see MixinDependencySortingIntegrationTest.MixinDefinitionsAreSortedCorrectlySmall

      Assert.That (MixinTypeUtility.GetMixinTypesExact (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType7))), Is.EqualTo (new[] { 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5) }));

      Assert.That (MixinTypeUtility.GetMixinTypesExact (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType3))), Has.Member(typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }

    [Test]
    public void CreateInstanceUnmixedTypes ()
    {
      Assert.That (MixinTypeUtility.CreateInstance (typeof (object)).GetType (), Is.SameAs (typeof (object)));
      Assert.That (MixinTypeUtility.CreateInstance (typeof (int)).GetType (), Is.SameAs (typeof (int)));
    }

    [Test]
    public void CreateInstanceMixedTypes ()
    {
      Assert.That (MixinTypeUtility.CreateInstance (typeof (BaseType1)).GetType (), Is.Not.SameAs (typeof (BaseType1)));
      Assert.That (MixinTypeUtility.CreateInstance (typeof (BaseType1)).GetType (), Is.SameAs (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));
    }

    [Test]
    public void CreateInstanceConcreteType()
    {
      Assert.That (
                  MixinTypeUtility.CreateInstance (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))).GetType (), Is.SameAs (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))));
    }

    [Test]
    public void CreateInstanceWithCtorArgs ()
    {
      var instance = (List<int>) MixinTypeUtility.CreateInstance (typeof (List<int>), 51);
      Assert.That (instance.Capacity, Is.EqualTo (51));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnMixedType ()
    {
      Assert.That (MixinTypeUtility.GetUnderlyingTargetType (typeof (BaseType1)), Is.SameAs (typeof (BaseType1)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnUnmixedType ()
    {
      Assert.That (MixinTypeUtility.GetUnderlyingTargetType (typeof (object)), Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnConcreteType ()
    {
      Assert.That (MixinTypeUtility.GetUnderlyingTargetType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1))), Is.SameAs (typeof (BaseType1)));
    }

    [Test]
    public void GetUnderlyingTargetTypeOnDerivedConcreteType ()
    {
      Type concreteType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1));
      var customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType();
      Assert.That (MixinTypeUtility.GetUnderlyingTargetType (derivedType), Is.SameAs (typeof (BaseType1)));
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
