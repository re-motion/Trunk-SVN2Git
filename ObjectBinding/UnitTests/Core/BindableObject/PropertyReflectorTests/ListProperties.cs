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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ListProperties : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "Array");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
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
    public void GetMetadata_WithReadOnlyArray ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyArray");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithListOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ListOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ListOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyListOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyListOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyListOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyCollectionOfT ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyCollectionOfT");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyCollectionOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ReadOnlyCollection<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyCollectionOfTWithSetter ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyCollectionOfTWithSetter");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyCollectionOfTWithSetter"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ReadOnlyCollection<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArrayList ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ArrayList");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyArrayList ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithListProperties), "ReadOnlyArrayList");
      PropertyReflector propertyReflector = PropertyReflector.Create(IPropertyInformation, _businessObjectProvider);

      Assert.AreSame (typeof (SimpleReferenceType), GetUnderlyingType (propertyReflector));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (IPropertyInformation));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithMissingItemTypeAttributeOnIList ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithItemTypeAttributeOnOverride ()
    {
      // expect exception
    }
  }
}
