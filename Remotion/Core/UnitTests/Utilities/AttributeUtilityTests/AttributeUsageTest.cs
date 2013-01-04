// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
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
      Assert.That (attribute, Is.EqualTo (typeof (MultipleAttribute).GetCustomAttributes (typeof (AttributeUsageAttribute), true)[0]));
    }

    [Test]
    public void GetAttributeUsageNeverNull ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (ImplicitUsageAttribute));
      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute, Is.EqualTo (new AttributeUsageAttribute(AttributeTargets.All)));
    }

    [Test]
    public void GetAttributeUsageWithNoAttribute ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (object));
      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute, Is.EqualTo (new AttributeUsageAttribute(AttributeTargets.All)));
    }

    [Test]
    public void AllowMultipleTrue ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (MultipleAttribute)), Is.True);
    }

    [Test]
    public void AllowMultipleFalse ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (NotInheritedNotMultipleAttribute)), Is.False);
    }

    [Test]
    public void DefaultAllowMultiple ()
    {
      Assert.That (AttributeUtility.IsAttributeAllowMultiple (typeof (ImplicitUsageAttribute)), Is.False);
    }

    [Test]
    public void InheritedTrue ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited(typeof (InheritedAttribute)), Is.True);
    }

    [Test]
    public void InheritedFalse ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited (typeof (NotInheritedNotMultipleAttribute)), Is.False);
    }

    [Test]
    public void DefaultInherited ()
    {
      Assert.That (AttributeUtility.IsAttributeInherited (typeof (ImplicitUsageAttribute)), Is.True);
    }
  }
}
