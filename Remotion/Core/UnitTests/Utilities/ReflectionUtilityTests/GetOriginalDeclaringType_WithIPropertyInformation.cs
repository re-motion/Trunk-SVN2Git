using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetOriginalDeclaringType_WithIPropertyInformation
  {
    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnBaseClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("String");

      Assert.AreSame (
          typeof (ClassWithDifferentProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("OtherString");

      Assert.AreSame (
          typeof (DerivedClassWithDifferentProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("String");

      Assert.AreSame (
          typeof (DerivedClassWithDifferentProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnBaseClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithDifferentProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnDerivedClass ()
    {
      IPropertyInformation propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithDifferentProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    private IPropertyInformation GetPropertyInfo<T> (string property)
    {
      return new PropertyInfoAdapter (typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
    }
  }
}