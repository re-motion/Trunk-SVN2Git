/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesForMixedTypes_WithSuppressTest
  {
    [Test]
    public void MixinAttribute_IsAlsoSuppressed ()
    {
      using (MixinConfiguration.BuildNew().ForClass<DerivedWithAttributesAndSuppressed>().AddMixin<MixinAddingInheritedAttribute>().EnterScope()) 
      {
        Type type = TypeFactory.GetConcreteType(typeof(DerivedWithAttributesAndSuppressed));
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true), 
            List.Not.Contains (new BaseInheritedAttribute ("MixinAddingInheritedAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseWithAttributesForSuppressed> ().AddMixin<MixinAddingSuppressAttribute> ().EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (BaseWithAttributesForSuppressed));
        Assert.That (type.GetCustomAttributes (true), List.Contains (new BaseInheritedAttribute ("BaseWithAttributesForSuppressed")));
        Assert.That (type.GetCustomAttributes (true), List.Contains (new BaseInheritedAttribute ("MixinAddingSuppressAttribute")));
        
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            List.Not.Contains (new BaseInheritedAttribute ("BaseWithAttributesForSuppressed")));
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            List.Contains (new BaseInheritedAttribute ("MixinAddingSuppressAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingNonInheritedAttributeOnTargetClass ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithNonInheritedAttribute> ().AddMixin<MixinSuppressingNonInheritedAttribute> ().EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (ClassWithNonInheritedAttribute));
        
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            List.Not.Contains (new BaseNonInheritedAttribute ("ClassWithNonInheritedAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingAttributeOnOtherMixin ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutAttributes> ().AddMixin<MixinAddingInheritedAttribute> ().AddMixin<MixinAddingSuppressAttribute>()
          .EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (ClassWithoutAttributes));

        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            List.Not.Contains (new BaseInheritedAttribute ("MixinAddingInheritedAttribute")));
      }
    }

  }
}