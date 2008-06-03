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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class PropertyInfoAdapterTest
  {
    private PropertyInfoAdapter _adapter;
    private PropertyInfo _property;

    private PropertyInfoAdapter _explicitInterfaceAdapter;

    [SetUp]
    public void SetUp ()
    {
      _property = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("NotVisibleAttributeScalar");
      _adapter = new PropertyInfoAdapter (_property);
      _explicitInterfaceAdapter = new PropertyInfoAdapter (
          typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
              "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
              BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Test]
    public void AdaptSingle ()
    {
      Assert.AreSame (_property, _adapter.PropertyInfo);
    }

    [Test]
    public void AdaptCollection ()
    {
      PropertyInfo[] properties = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperties ();
      Assert.IsTrue (properties.Length > 1);
      List<PropertyInfo> adaptedProperties =
          new List<IPropertyInformation> (PropertyInfoAdapter.AdaptCollection (properties)).ConvertAll<PropertyInfo> (
              delegate (IPropertyInformation info) { return ((PropertyInfoAdapter) info).PropertyInfo; });

      Assert.That (adaptedProperties, Is.EqualTo (properties));
    }

    [Test]
    public void UnwrapCollection ()
    {
      PropertyInfo[] properties = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperties ();
      Assert.That (new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (PropertyInfoAdapter.AdaptCollection (properties))), Is.EqualTo (properties));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.AreEqual (_property.PropertyType, _adapter.PropertyType);
    }

    [Test]
    public void Name ()
    {
      Assert.AreEqual (_property.Name, _adapter.Name);
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.AreEqual (_property.DeclaringType, _adapter.DeclaringType);
    }

    [Test]
    public void CanBeSetFromOutside ()
    {
      PropertyInfoAdapter readWriteAdapter = new PropertyInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("Scalar"));
      Assert.IsTrue (readWriteAdapter.CanBeSetFromOutside);
      PropertyInfoAdapter readOnlyAdapter =
          new PropertyInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyScalar"));
      Assert.IsFalse (readOnlyAdapter.CanBeSetFromOutside);

      PropertyInfoAdapter readOnlyNonPublicSetterAdapter =
          new PropertyInfoAdapter (typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ReadOnlyNonPublicSetterScalar"));
      Assert.IsFalse (readOnlyNonPublicSetterAdapter.CanBeSetFromOutside);

      PropertyInfoAdapter readWriteExplicitAdapter = _explicitInterfaceAdapter;
      Assert.IsTrue (readWriteExplicitAdapter.CanBeSetFromOutside);

      PropertyInfoAdapter readOnlyExplicitAdapter =
          new PropertyInfoAdapter (
              typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty (
                  "Remotion.ObjectBinding.UnitTests.Core.TestDomain.IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar", BindingFlags.NonPublic | BindingFlags.Instance));
      Assert.IsFalse (readOnlyExplicitAdapter.CanBeSetFromOutside);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.AreEqual (
          AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (_property, true), _adapter.GetCustomAttribute<ObjectBindingAttribute> (true));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.AreEqual (
          AttributeUtility.GetCustomAttributes<ObjectBindingAttribute> (_property, true), _adapter.GetCustomAttributes<ObjectBindingAttribute> (true));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.AreEqual (
          AttributeUtility.IsDefined<ObjectBindingAttribute> (_property, true), _adapter.IsDefined<ObjectBindingAttribute> (true));
    }

    [Test]
    public void GetValue ()
    {
      ClassWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      instance.NotVisibleAttributeScalar = new SimpleReferenceType ();
      Assert.AreEqual (_property.GetValue (instance, null), _adapter.GetValue (instance, null));
    }

    [Test]
    public void GetValue_ExplicitInterface ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      instance.ExplicitInterfaceScalar = new SimpleReferenceType ();
      Assert.AreEqual (instance.ExplicitInterfaceScalar, _explicitInterfaceAdapter.GetValue (instance, null));
    }
    
    [Test]
    public void SetValue ()
    {
      ClassWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      SimpleReferenceType value = new SimpleReferenceType ();
      _adapter.SetValue (instance, value, null);
      Assert.AreSame (value, instance.NotVisibleAttributeScalar);
    }

    [Test]
    public void SetValue_ExplicitInterface ()
    {
      IInterfaceWithReferenceType<SimpleReferenceType> instance = new ClassWithReferenceType<SimpleReferenceType> ();
      SimpleReferenceType value = new SimpleReferenceType ();
      _explicitInterfaceAdapter.SetValue (instance, value, null);
      Assert.AreSame (value, instance.ExplicitInterfaceScalar);
    }

    [Test]
    public void Equals_ChecksPropertyInfo ()
    {
      Assert.AreEqual (new PropertyInfoAdapter (_property), _adapter);
      Assert.AreNotEqual (_explicitInterfaceAdapter, _adapter);
    }

    [Test]
    public void GetHashCode_UsesPropertyInfo ()
    {
      Assert.AreEqual (new PropertyInfoAdapter (_property).GetHashCode (), _adapter.GetHashCode ());
      Assert.AreNotEqual (_explicitInterfaceAdapter.GetHashCode (), _adapter.GetHashCode ());
    }

    class ClassWithBaseProperty
    {
      public virtual int BaseProperty { get { return 0; } }
    }

    class ClassWithOverridingProperty : ClassWithBaseProperty
    {
      public override int BaseProperty { get { return 0; } }
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.AreEqual (_adapter.DeclaringType, _adapter.GetOriginalDeclaringType ());
      
      PropertyInfoAdapter overrideAdapter = new PropertyInfoAdapter (typeof (ClassWithOverridingProperty).GetProperty ("BaseProperty"));
      Assert.AreNotEqual (overrideAdapter.DeclaringType, overrideAdapter.GetOriginalDeclaringType ());
      Assert.AreEqual (overrideAdapter.DeclaringType.BaseType, overrideAdapter.GetOriginalDeclaringType ());
      Assert.AreEqual (typeof (ClassWithBaseProperty), overrideAdapter.GetOriginalDeclaringType ());
    }
  }
}
