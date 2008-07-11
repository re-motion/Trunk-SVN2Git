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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class AttributeUsageTest
  {
    [Test]
    public void GetAttributeUsage ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (MultipleAttribute));
      Assert.AreEqual (typeof (MultipleAttribute).GetCustomAttributes (typeof (AttributeUsageAttribute), true)[0], attribute);
    }

    [Test]
    public void GetAttributeUsageNeverNull ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (ImplicitUsageAttribute));
      Assert.IsNotNull (attribute);
      Assert.AreEqual (new AttributeUsageAttribute(AttributeTargets.All), attribute);
    }

    [Test]
    public void GetAttributeUsageWithNoAttribute ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (object));
      Assert.IsNotNull (attribute);
      Assert.AreEqual (new AttributeUsageAttribute(AttributeTargets.All), attribute);
    }

    [Test]
    public void AllowMultipleTrue ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeAllowMultiple (typeof (MultipleAttribute)));
    }

    [Test]
    public void AllowMultipleFalse ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeAllowMultiple (typeof (NotInheritedNotMultipleAttribute)));
    }

    [Test]
    public void DefaultAllowMultiple ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeAllowMultiple (typeof (ImplicitUsageAttribute)));
    }

    [Test]
    public void InheritedTrue ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeInherited(typeof (InheritedAttribute)));
    }

    [Test]
    public void InheritedFalse ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeInherited (typeof (NotInheritedNotMultipleAttribute)));
    }

    [Test]
    public void DefaultInherited ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeInherited (typeof (ImplicitUsageAttribute)));
    }
  }
}
