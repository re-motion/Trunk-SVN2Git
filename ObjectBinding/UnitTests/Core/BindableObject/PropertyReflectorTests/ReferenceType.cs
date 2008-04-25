using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ReferenceType : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void GetMetadata_WithScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyScalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyAttributeScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyAttributeScalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyAttributeScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyNonPublicSetterScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "ReadOnlyNonPublicSetterScalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyNonPublicSetterScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadWriteExplicitInterfaceScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
          "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ExplicitInterfaceScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadWriteExplicitInterfaceReadOnlyScalar ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>),
          "Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar");
      PropertyReflector propertyReflector = new PropertyReflector (IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ExplicitInterfaceReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }
  }
}