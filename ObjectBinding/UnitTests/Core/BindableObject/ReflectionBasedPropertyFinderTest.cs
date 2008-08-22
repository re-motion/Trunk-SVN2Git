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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReflectionBasedPropertyFinderTestDomain;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ReflectionBasedPropertyFinderTest
  {
    [Test]
    public void ReturnsPublicInstancePropertiesFromThisAndBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestType));
      var properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestType).GetProperty ("PublicInstanceProperty"),
                      typeof (BaseTestType).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    [Test]
    public void IgnoresBasePropertiesWithSameName ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeHidingProperties));
      var properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestTypeHidingProperties).GetProperty ("PublicInstanceProperty"),
                      typeof (TestTypeHidingProperties).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    [Test]
    public void FindsPropertiesFromImplicitInterfaceImplementations ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      var properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties, 
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty ("InterfaceProperty", 
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }
    
    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementations ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      var properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (typeof (IExplicitTestInterface).FullName + ".InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementationsOnBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (DerivedTypeWithInterfaces));
      var properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (typeof (IExplicitTestInterface).FullName + ".InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void NoPropertiesFromBindableObjectMixins ()
    {
      Type targetType = typeof (ClassWithIdentity);
      Type concreteType = Mixins.TypeUtility.GetConcreteMixedType (targetType);

      var targetTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (targetType).GetPropertyInfos ());
      var concreteTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (concreteType).GetPropertyInfos ());

      Assert.That (concreteTypeProperties, Is.EquivalentTo (targetTypeProperties));
    }

    [Test]
    public void PropertyWithDoubleInterfaceMethod ()
    {
      var propertyInfos = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (typeof (ClassWithDoubleInterfaceProperty)).GetPropertyInfos ());
      Assert.AreEqual (1, propertyInfos.Count);
      Assert.AreEqual ("DisplayName", propertyInfos[0].Name);
    }

    [Test]
    public void ImplicitInterfaceProperties_GetInterfaceBasedPropertyInfo()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceProperty = (PropertyInfoAdapter) (from p in propertyInfos
                               where p.Name == "InterfaceProperty"
                               select p).Single ();
      Assert.That (interfaceProperty.ValuePropertyInfo, Is.SameAs (typeof (ITestInterface).GetProperty ("InterfaceProperty")));
    }

    [Test]
    public void ExplicitInterfaceProperties_GetInterfaceBasedPropertyInfo ()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceProperty = (PropertyInfoAdapter) (from p in propertyInfos
                               where p.Name == typeof (IExplicitTestInterface).FullName + ".InterfaceProperty"
                               select p).Single ();
      Assert.That (interfaceProperty.ValuePropertyInfo, Is.SameAs (typeof (IExplicitTestInterface).GetProperty ("InterfaceProperty")));
    }
  }
}