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
  public class GetCustomAttributesFromType_WithSuppresTest
  {
    [Test]
    public void BaseClass_NothingSuppressed ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseWithAttributesForSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseWithAttributesForSuppressed"), 
        new DerivedInheritedAttribute ("BaseWithAttributesForSuppressed")}));
    }

    [Test]
    public void DerivedClass_NothingSuppressed_InheritedFalse ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedClass_BaseAttributesAreSuppressed_InheritedTrue ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedDerivedClass_BaseAttributesAndOwnAreSuppressed_InheritedTrue ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedDerivedWithAttributesForSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedDerivedClass_NothingSuppressed_InheritedFalse ()
    {
      Attribute[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedDerivedWithAttributesForSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedDerivedWithAttributesForSuppressed"), 
        new DerivedInheritedAttribute ("DerivedDerivedWithAttributesForSuppressed")}));
    }

  }
}