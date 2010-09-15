// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReflectionBasedPropertyFinderTestDomain;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ReflectionBasedPropertyFinderTest
  {
    [Test]
    public void ReturnsPublicInstancePropertiesFromThisAndBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestType));
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
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
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
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
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties, 
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty ("InterfaceProperty", 
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }
    
    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementations ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (typeof (IExplicitTestInterface).FullName + ".InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementationsOnBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (DerivedTypeWithInterfaces));
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (typeof (IExplicitTestInterface).FullName + ".InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void NoPropertiesFromBindableObjectMixins ()
    {
      Type targetType = typeof (ClassWithIdentity);
      Type concreteType = Mixins.MixinTypeUtility.GetConcreteMixedType (targetType);

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
      Assert.That (interfaceProperty.Name, Is.EqualTo ("InterfaceProperty"));
    }

    [Test]
    public void ExplicitInterfaceProperties_GetInterfaceBasedPropertyInfo ()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceProperty = (PropertyInfoAdapter) (from p in propertyInfos
                               where p.Name == typeof (IExplicitTestInterface).FullName + ".InterfaceProperty"
                               select p).Single ();
      Assert.That (interfaceProperty.Name, 
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReflectionBasedPropertyFinderTestDomain.IExplicitTestInterface.InterfaceProperty"));
    }

    [Test]
    public void InterfaceProperties_PropertyWithoutGetter ()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceProperty = (BindableObjectPropertyInfoAdapter) (from p in propertyInfos
                                                     where p.Name == "NonGetterInterfaceProperty"
                                                     select p).SingleOrDefault ();
      Assert.That (interfaceProperty, Is.Null);
    }

    [Test]
    public void GetPropertyInfos_MixedProperties ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<PropertyFinderMixinAddingProperty> ()
          .EnterScope ())
      {
        var concreteType = TypeFactory.GetConcreteType (typeof (BaseBusinessObjectClass));
        var propertyFinder = new ReflectionBasedPropertyFinder (concreteType);
        var propertyInformations = propertyFinder.GetPropertyInfos().OrderBy (pi => pi.Name).ToArray();

        Assert.That (propertyInformations[0], Is.TypeOf (typeof(BindableObjectMixinIntroducedPropertyInformation)));
        Assert.That (propertyInformations[0].DeclaringType, Is.SameAs (typeof (PropertyFinderMixinAddingProperty)));
        Assert.That (((BindableObjectMixinIntroducedPropertyInformation) propertyInformations[0]).ConcreteType, Is.SameAs (concreteType));
        Assert.That (propertyInformations[0].Name, Is.EqualTo ("MixedProperty"));
        
        Assert.That (propertyInformations[1], Is.TypeOf(typeof (BindableObjectMixinIntroducedPropertyInformation)));
        Assert.That (propertyInformations[1].DeclaringType, Is.SameAs (typeof (PropertyFinderMixinAddingProperty)));
        Assert.That (((BindableObjectMixinIntroducedPropertyInformation) propertyInformations[1]).ConcreteType, Is.SameAs (concreteType));
        Assert.That (propertyInformations[1].Name, Is.EqualTo ("MixedReadOnlyProperty"));

        Assert.That (propertyInformations[2], Is.TypeOf (typeof (PropertyInfoAdapter)));
        Assert.That (propertyInformations[2].DeclaringType, Is.SameAs (typeof (BaseBusinessObjectClass)));
        Assert.That (propertyInformations[2].Name, Is.EqualTo ("Public"));
        
        Assert.That (propertyInformations[3], Is.TypeOf (typeof (BindableObjectMixinIntroducedPropertyInformation)));
        Assert.That (propertyInformations[3].DeclaringType, Is.SameAs (typeof (PropertyFinderMixinAddingProperty)));
        Assert.That (((BindableObjectMixinIntroducedPropertyInformation) propertyInformations[3]).ConcreteType, Is.SameAs (concreteType));
        Assert.That (propertyInformations[3].Name, Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.IPropertyFinderMixinAddingProperty.ExplicitMixedProperty"));
      }
    }

    [Test]
    public void GetPropertyInfos_MixedProperties_DuplicatesNotRemoved ()
    {
      var concreteType = TypeFactory.GetConcreteType (typeof (ClassWithMixedPropertyOfSameName));
      var propertyFinder = new ReflectionBasedPropertyFinder (concreteType);
      var propertyInformations = propertyFinder.GetPropertyInfos().OrderBy (pi => pi.Name).ThenBy (pi => pi.DeclaringType.FullName).ToArray();

      Assert.That (propertyInformations[0], Is.TypeOf (typeof (PropertyInfoAdapter)));
      Assert.That (propertyInformations[0].DeclaringType, Is.SameAs (typeof (ClassWithMixedPropertyOfSameName)));
      Assert.That (propertyInformations[0].Name, Is.EqualTo ("MixedProperty"));

      Assert.That (propertyInformations[1], Is.TypeOf (typeof (BindableObjectMixinIntroducedPropertyInformation)));
      Assert.That (propertyInformations[1].DeclaringType, Is.SameAs (typeof (MixinAddingProperty)));
      Assert.That (propertyInformations[1].Name, Is.EqualTo ("MixedProperty"));
    }

    private IEnumerable<PropertyInfo> UnwrapCollection (IEnumerable<IPropertyInformation> properties)
    {
      return from PropertyInfoAdapter adapter in properties 
             select adapter.PropertyInfo;
    }
  }
}
