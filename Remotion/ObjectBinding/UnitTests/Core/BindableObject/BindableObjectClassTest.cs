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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectClassTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      BindableObjectClass bindableObjectClass =
          new BindableObjectClass (MixinTypeUtility.GetConcreteMixedType (typeof (SimpleBusinessObjectClass)), _bindableObjectProvider);

      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (bindableObjectClass.ConcreteType, Is.Not.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (bindableObjectClass.ConcreteType, Is.SameAs (Mixins.MixinTypeUtility.GetConcreteMixedType (typeof (SimpleBusinessObjectClass))));
      Assert.That (
          bindableObjectClass.Identifier,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
      Assert.That (bindableObjectClass.BusinessObjectProviderAttribute, Is.InstanceOfType (typeof (BindableObjectProviderAttribute)));
    }

    [Test]
    public void Initialize_WithGeneric ()
    {
      BindableObjectClass bindableObjectClass =
          new BindableObjectClass (MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithReferenceType<SimpleReferenceType>)), _bindableObjectProvider);

      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithReferenceType<SimpleReferenceType>)));
    }

    public void Initialize_WithTypeNotUsingBindableObjectMixin ()
    {
      var bindableObjectClass = new BindableObjectClass (MixinTypeUtility.GetConcreteMixedType (typeof (SimpleReferenceType)), _bindableObjectProvider);
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (SimpleReferenceType)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (SimpleReferenceType)));
    }

    [Test]
    public void Initialize_WithUnmixedType ()
    {
      var bindableObjectClass = new BindableObjectClass (typeof (ManualBusinessObject), _bindableObjectProvider);
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ManualBusinessObject)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ManualBusinessObject)));
    }

    [Test]
    public void Initialize_WithTypeDerivedFromBindableObjectBase ()
    {
      var bindableObjectClass = new BindableObjectClass (typeof (ClassDerivedFromBindableObjectBase), _bindableObjectProvider);
      Assert.That (bindableObjectClass.TargetType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
      Assert.That (bindableObjectClass.ConcreteType, Is.EqualTo (typeof (ClassDerivedFromBindableObjectBase)));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      PropertyReflector propertyReflector =
          PropertyReflector.Create(GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"), _bindableObjectProvider);
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider, BindableObjectMetadataFactory.Create());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase (propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition ("String"));
    }

    [Test]
    public void GetPropertyDefinition_ForMixedProperty ()
    {
      PropertyReflector propertyReflector =
          PropertyReflector.Create(GetPropertyInfo (
                     MixinTypeUtility.GetConcreteMixedType (typeof (ClassWithMixedProperty)),
                     typeof (IMixinAddingProperty).FullName + ".MixedProperty"), _bindableObjectProvider);
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperty), _bindableObjectProvider, BindableObjectMetadataFactory.Create());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase (propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition ("MixedProperty"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException),
        ExpectedMessage =
        "The property 'Invalid' was not found on business object class "
        + "'Remotion.ObjectBinding.UnitTests.Core.TestDomain.ClassWithAllDataTypes, Remotion.ObjectBinding.UnitTests'.")]
    public void GetPropertyDefinition_WithInvalidPropertyName ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider, BindableObjectMetadataFactory.Create());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      bindableObjectClass.GetPropertyDefinition ("Invalid");
    }

    [Test]
    public void HasPropertyDefinition ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider, BindableObjectMetadataFactory.Create());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass.HasPropertyDefinition ("String"), Is.True);
      Assert.That (bindableObjectClass.HasPropertyDefinition ("Invalid"), Is.False);
    }

    [Test]
    public void HasPropertyDefinition_ForMixedProperty ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedProperty), _bindableObjectProvider, BindableObjectMetadataFactory.Create());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass.HasPropertyDefinition ("MixedProperty"), Is.True);
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      Type type = typeof (ClassWithReferenceType<SimpleReferenceType>);
      PropertyBase[] expectedProperties = new PropertyBase[]
          {
              CreateProperty (type, "Scalar"),
              CreateProperty (type, "ReadOnlyScalar"),
              CreateProperty (type, "ReadOnlyAttributeScalar"),
              CreateProperty (type, "ReadOnlyNonPublicSetterScalar"),
              CreateProperty (type, "Array"),
              CreateProperty (type, "ImplicitInterfaceScalar"),
              CreateProperty (type, "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar"),
              CreateProperty (type, "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar")
          };

      ClassReflector classReflector = new ClassReflector (type, _bindableObjectProvider, BindableObjectMetadataFactory.Create ());
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();
      IBusinessObjectProperty[] actualProperties = bindableObjectClass.GetPropertyDefinitions();

      Assert.That (actualProperties.Length, Is.EqualTo (expectedProperties.Length));
      foreach (PropertyBase expectedProperty in expectedProperties)
      {
        bool isFound = false;
        foreach (IBusinessObjectProperty actualProperty in actualProperties)
        {
          if (actualProperty.Identifier == expectedProperty.Identifier)
          {
            Assert.That (isFound, Is.False, "Multiple properties '{0}' found", expectedProperty.Identifier, BindableObjectMetadataFactory.Create ());
            CheckPropertyBase (expectedProperty, actualProperty);
            isFound = true;
          }
        }
        Assert.That (isFound, Is.True, "Property '{0}' was not found", expectedProperty.Identifier);
      }
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      Type type = typeof (ClassWithReferenceType<SimpleReferenceType>);
      PropertyBase[] expectedProperties = new PropertyBase[]
          {
              CreateProperty (type, "Scalar"),
              CreateProperty (type, "ReadOnlyScalar"),
          };

      BindableObjectClass bindableObjectClass = new BindableObjectClass (MixinTypeUtility.GetConcreteMixedType (type), _bindableObjectProvider);
      bindableObjectClass.SetPropertyDefinitions (expectedProperties);
      IBusinessObjectProperty[] actualProperties = bindableObjectClass.GetPropertyDefinitions();

      Assert.That (actualProperties, Is.EqualTo (expectedProperties));
      foreach (IBusinessObjectProperty actualProperty in actualProperties)
        Assert.That (((PropertyBase) actualProperty).ReflectedClass, Is.SameAs (bindableObjectClass));
    }

    private void CheckPropertyBase (IBusinessObjectProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      ArgumentUtility.CheckNotNull ("expectedProperty", expectedProperty);

      Assert.That (actualProperty, Is.Not.Null);
      Assert.That (actualProperty.GetType(), Is.SameAs (expectedProperty.GetType()), "BusinessObjectPropertyType");
      Assert.That (expectedProperty.PropertyType, Is.EqualTo (actualProperty.PropertyType), "PropertyType");
      Assert.That (expectedProperty.IsList, Is.EqualTo (actualProperty.IsList), "IsList");
      if (expectedProperty.IsList)
        Assert.That (expectedProperty.ListInfo.ItemType, Is.EqualTo (actualProperty.ListInfo.ItemType), "ListInfo.ItemType");
      Assert.That (expectedProperty.IsRequired, Is.EqualTo (actualProperty.IsRequired), "IsRequired");
      Assert.That (((PropertyBase) actualProperty).ReflectedClass, Is.Not.Null);

      if (typeof (IBusinessObjectStringProperty).IsAssignableFrom (actualProperty.GetType()))
        CheckStringProperty ((IBusinessObjectStringProperty) actualProperty, expectedProperty);
    }

    private void CheckStringProperty (IBusinessObjectStringProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      Assert.That (
          expectedProperty.MaxLength,
          Is.EqualTo (((IBusinessObjectStringProperty) actualProperty).MaxLength),
          "MaxLength");
    }

    private PropertyBase CreateProperty (Type type, string propertyName)
    {
      PropertyReflector propertyReflector = PropertyReflector.Create(GetPropertyInfo (type, propertyName), _bindableObjectProvider);
      return propertyReflector.GetMetadata();
    }
  }
}
