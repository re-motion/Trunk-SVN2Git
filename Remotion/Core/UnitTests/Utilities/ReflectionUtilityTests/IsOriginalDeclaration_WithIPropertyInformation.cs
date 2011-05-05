using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class IsOriginalDeclaration_WithIPropertyInformation
  {
    [Test]
    public void IsOriginalDeclaration_ForPropertyOnBaseClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithDifferentProperties), "String");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "OtherString");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForNewPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "String");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForOverriddenPropertyOnBaseClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (ClassWithDifferentProperties), "Int32");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_ForOverriddenPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClassWithDifferentProperties), "Int32");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnBaseClass_Open ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnBaseClass_Closed ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedOpenClass_Open ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedOpenClass_Closed ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForPropertyOnDerivedClosedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "OtherVirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedOpenClass_Open ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedOpenClass_Closed ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForNewPropertyOnDerivedClosedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "VirtualT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnBaseClass_Open ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnBaseClass_Closed ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.True);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedOpenClass_Open ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedOpenClass_Closed ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<int>), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    [Test]
    public void IsOriginalDeclaration_Generic_ForOverriddenPropertyOnDerivedClosedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo (typeof (DerivedClosedGenericClassWithDifferentProperties), "AbstractT");
      Assert.That (ReflectionUtility.IsOriginalDeclaration (propertyInfo), Is.False);
    }

    private IPropertyInformation GetPropertyInfo (Type type, string property)
    {
      return new PropertyInfoAdapter (type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
    }
  }
}