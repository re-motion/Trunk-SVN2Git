using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedNameResolverTest : StandardMappingTest
  {
    private PropertyInfo _orderNumberProperty;
    private PropertyInfo _overriddenProperty;
    private PropertyInfo _overriddenPropertyOnBase;
    private PropertyInfo _propertyInGenericClass;
    private PropertyInfo _propertyInOpenGenericClass;
    private ReflectionBasedNameResolver _resolver;

    public override void SetUp ()
    {
      base.SetUp();
      _resolver = new ReflectionBasedNameResolver();
      _orderNumberProperty = typeof (Order).GetProperty ("OrderNumber");
      _overriddenProperty = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");
      _overriddenPropertyOnBase = typeof (ClassWithMixedProperties).GetProperty ("Int32");
      _propertyInGenericClass = typeof (ClosedGenericClassWithManySideRelationProperties).GetProperty ("BaseUnidirectional");
      _propertyInOpenGenericClass = typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).GetProperty ("BaseUnidirectional");
    }

    [Test]
    public void GetPropertyName ()
    {
      string name = _resolver.GetPropertyName (_orderNumberProperty);
      Assert.That (name, Is.EqualTo (typeof (Order).FullName + ".OrderNumber"));
    }

    [Test]
    public void GetPropertyName_ForOverriddenProperty ()
    {
      string name = _resolver.GetPropertyName (_overriddenProperty);
      Assert.That (name, Is.EqualTo (typeof (ClassWithMixedProperties).FullName + ".Int32"));
    }

    [Test]
    public void GetPropertyName_ForPropertyInGenericType ()
    {
      string name = _resolver.GetPropertyName (_propertyInGenericClass);
      Assert.That (name, Is.EqualTo (typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName + ".BaseUnidirectional"));
    }

    [Test]
    public void GetProperty ()
    {
      string name = typeof (Order).FullName + ".OrderNumber";
      PropertyInfo resolvedProperty = _resolver.GetProperty (typeof (Order), name);
      Assert.That (resolvedProperty, Is.EqualTo (_orderNumberProperty));
    }

    [Test]
    public void GetProperty_ShortName ()
    {
      string name = "OrderNumber";
      PropertyInfo resolvedProperty = _resolver.GetProperty (typeof (Order), name);
      Assert.That (resolvedProperty, Is.EqualTo (_orderNumberProperty));
    }

    [Test]
    public void GetProperty_ForOverriddenProperty ()
    {
      string name = typeof (ClassWithMixedProperties).FullName + ".Int32";
      PropertyInfo resolvedProperty1 = _resolver.GetProperty (typeof (ClassWithMixedProperties), name);
      PropertyInfo resolvedProperty2 = _resolver.GetProperty (typeof (DerivedClassWithMixedProperties), name);
      Assert.That (resolvedProperty1, Is.EqualTo (_overriddenPropertyOnBase));
      Assert.That (resolvedProperty2, Is.EqualTo (_overriddenProperty));
    }

    [Test]
    public void GetProperty_ForPropertyInGenericType ()
    {
      string name = typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>).FullName + ".BaseUnidirectional";
      PropertyInfo resolvedProperty1 = _resolver.GetProperty (typeof (GenericClassWithManySideRelationPropertiesNotInMapping<>), name);
      PropertyInfo resolvedProperty2 = _resolver.GetProperty (typeof (ClosedGenericClassWithManySideRelationProperties), name);
      Assert.That (resolvedProperty1, Is.EqualTo (_propertyInOpenGenericClass));
      Assert.That (resolvedProperty2, Is.EqualTo (_propertyInGenericClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "'bla.' is not a valid mapping property name.\r\nParameter name: propertyName")]
    public void GetProperty_ForMalformedPropertyName_PeriodAtEnd ()
    {
      _resolver.GetProperty (typeof (Order), "bla.");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not contain a property named 'Bla'.\r\nParameter name: propertyName")]
    public void GetProperty_ForNonExistingProperty ()
    {
      _resolver.GetProperty (typeof (Order), typeof (Order).FullName + ".Bla");
    }
  }
}