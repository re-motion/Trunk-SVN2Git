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
