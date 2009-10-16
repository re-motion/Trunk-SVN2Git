// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixinReflectorTest
  {
    [Test]
    public void GetTargetProperty ()
    {
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT1Mixin1)), Is.Null);

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin1)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin1)),
          Is.EqualTo (typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin2)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin2)),
          Is.EqualTo (typeof (Mixin<IBaseType32>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), 
          Is.Not.EqualTo (typeof (Mixin<,>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetBaseProperty ()
    {
      Assert.That (MixinReflector.GetBaseProperty (typeof (BT1Mixin1)), Is.Null);

      Assert.That (MixinReflector.GetBaseProperty (typeof (BT3Mixin1)), Is.Not.Null);
      Assert.That (
          MixinReflector.GetBaseProperty (typeof (BT3Mixin1)),
          Is.EqualTo (
              typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetBaseProperty (typeof (BT3Mixin2)), Is.Null);

      Assert.That (MixinReflector.GetBaseProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), Is.Not.Null);
      Assert.That (MixinReflector.GetBaseProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), 
          Is.Not.EqualTo (typeof (Mixin<,>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));
      Assert.That (MixinReflector.GetBaseProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Base", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetMixinBaseCallProxyType ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Type bcpt = MixinReflector.GetBaseCallProxyType (bt1);
      Assert.That (bcpt, Is.Not.Null);
      Assert.That (bcpt, Is.EqualTo (bt1.GetType ().GetNestedType ("BaseCallProxy")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not a mixin target", MatchType = MessageMatch.Contains)]
    public void GetMixinBaseCallProxyTypeThrowsIfWrongType1 ()
    {
      MixinReflector.GetBaseCallProxyType (new object ());
    }

    [Test]
    public void GetMixinConfigurationFromConcreteType ()
    {
      Type bt1Type = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (MixinReflector.GetClassContextFromConcreteType (bt1Type),
          Is.EqualTo (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1))));
    }

    [Test]
    public void GetMixinConfigurationFromConcreteTypeNullWhenNoMixedType ()
    {
      Assert.That (MixinReflector.GetClassContextFromConcreteType (typeof (object)), Is.Null);
    }

    [Test]
    public void GetMixinConfigurationFromDerivedConcreteType ()
    {
      Type concreteType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType1));
      var customClassEmitter = new CustomClassEmitter (new ModuleScope (false), "Test", concreteType);
      Type derivedType = customClassEmitter.BuildType ();
      Assert.That (MixinReflector.GetClassContextFromConcreteType (derivedType),
          Is.EqualTo (MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1))));
    }

    [Test]
    public void GetOrderedMixinTypes_NullWhenNoMixedType ()
    {
      Assert.That (MixinReflector.GetOrderedMixinTypesFromConcreteType (typeof (object)), Is.Null);
    }

    [Test]
    public void GetOrderedMixinTypes_OrderedMixinTypes ()
    {
      var concreteMixedType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType7));
      
      // see MixinDependencySortingIntegrationTest.MixinDefinitionsAreSortedCorrectlySmall
      Assert.That (MixinReflector.GetOrderedMixinTypesFromConcreteType (concreteMixedType), Is.EqualTo (new[] { 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5) }));
    }

    [Test]
    public void GetOrderedMixinTypes_OpenGenericMixinTypesAreClosed ()
    {
      var concreteMixedType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType3));

      Assert.That (MixinReflector.GetOrderedMixinTypesFromConcreteType (concreteMixedType), 
          List.Contains (typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }
  }
}