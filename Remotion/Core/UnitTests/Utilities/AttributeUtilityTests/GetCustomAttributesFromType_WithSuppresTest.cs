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
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseWithAttributesForSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseWithAttributesForSuppressed"), 
        new DerivedInheritedAttribute ("BaseWithAttributesForSuppressed")}));
    }

    [Test]
    public void DerivedClass_NothingSuppressed_InheritedFalse ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedClass_BaseAttributesAreSuppressed_InheritedTrue ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedDerivedClass_BaseAttributesAndOwnAreSuppressed_InheritedTrue ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedDerivedWithAttributesForSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void DerivedDerivedClass_NothingSuppressed_InheritedFalse ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedDerivedWithAttributesForSuppressed), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedDerivedWithAttributesForSuppressed"), 
        new DerivedInheritedAttribute ("DerivedDerivedWithAttributesForSuppressed")}));
    }

  }
}
