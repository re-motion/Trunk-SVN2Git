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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesWithMetadataTest
  {
    [Test]
    public void BaseClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes = 
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (BaseClassWithAttribute), false));
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseNonInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedNonInheritedAttribute ("BaseClass")),
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new InheritedNotMultipleAttribute ("BaseClass")),
      }));
    }

    [Test]
    public void BaseClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (BaseClassWithAttribute), true));
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseNonInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedNonInheritedAttribute ("BaseClass")),
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new InheritedNotMultipleAttribute ("BaseClass")),
      }));
    }

    [Test]
    public void DerivedClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes =
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (DerivedClassWithAttribute), false));
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseNonInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedNonInheritedAttribute ("DerivedClass")),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new SuppressAttributesAttribute (typeof (InheritedNotMultipleAttribute))),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new InheritedNotMultipleAttribute ("DerivedClass")),
      }));
    }

    [Test]
    public void DerivedClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (DerivedClassWithAttribute), true));
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseNonInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedNonInheritedAttribute ("DerivedClass")),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new SuppressAttributesAttribute (typeof (InheritedNotMultipleAttribute))),
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new InheritedNotMultipleAttribute ("BaseClass")),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new InheritedNotMultipleAttribute ("DerivedClass")),
      }));
    }

    [Test]
    public void ObjectClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes =
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (object), false));
      Assert.That (attributes.Length, Is.EqualTo (typeof (object).GetCustomAttributes (false).Length));
    }

    [Test]
    public void ObjectClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        EnumerableUtility.ToArray (AttributeUtility.GetCustomAttributesWithMetadata (typeof (object), true));
      Assert.That (attributes.Length, Is.EqualTo (typeof (object).GetCustomAttributes (false).Length));
    }
  }
}