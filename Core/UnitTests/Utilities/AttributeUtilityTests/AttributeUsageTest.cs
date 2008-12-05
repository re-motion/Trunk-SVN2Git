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
