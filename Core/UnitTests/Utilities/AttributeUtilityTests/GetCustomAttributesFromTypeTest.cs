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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesFromTypeTest
  {
    [Test]
    public void BaseClass_InheritedFalse ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseNonInheritedAttribute ("BaseClass"), 
        new DerivedNonInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void BaseClass_InheritedTrue ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseNonInheritedAttribute ("BaseClass"), 
        new DerivedNonInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void DerivedClass_InheritedFalse ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedClassWithAttribute), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedClass"), 
        new DerivedInheritedAttribute ("DerivedClass"), 
        new BaseNonInheritedAttribute ("DerivedClass"), 
        new DerivedNonInheritedAttribute ("DerivedClass")}));
    }

    [Test]
    public void DerivedClass_InheritedTrue ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedClassWithAttribute), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseInheritedAttribute ("DerivedClass"), 
        new DerivedInheritedAttribute ("DerivedClass"), 
        new BaseNonInheritedAttribute ("DerivedClass"), 
        new DerivedNonInheritedAttribute ("DerivedClass")}));
    }
    
    
    [Test]
    public void Filtering_WithBaseAttributeType ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (BaseInheritedAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void Filtering_WithDerivedAttributeType ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (DerivedInheritedAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new DerivedInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void Filtering_WithInterfaceType ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (ICustomAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void GetCustomAttributes_WithMemberInfo_DelegatesToTypeVersion ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes ((MemberInfo) typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }
  }
}