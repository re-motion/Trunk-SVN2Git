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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class IsOriginalDeclaration
  {
    [Test]
    public void IsOriginalDeclaration_ForPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithDifferentProperties), "String");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "OtherString");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForNewPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "String");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForOverriddenPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithDifferentProperties), "Int32");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForOverriddenPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "Int32");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnBaseClass_Open ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnBaseClass_Closed ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedOpenClass_Open ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedOpenClass_Closed ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedClosedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedOpenClass_Open ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedOpenClass_Closed ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedClosedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnBaseClass_Open ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnBaseClass_Closed ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedOpenClass_Open ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedOpenClass_Closed ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedClosedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    private PropertyInfo GetPropertyInfo (Type type, string property)
    {
      return type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }
  }
}
