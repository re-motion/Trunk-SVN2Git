// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumValueDiscoveryServiceTest
  {
    private ExtensibleEnumValueDiscoveryService _service;

    [SetUp]
    public void SetUp ()
    {
      _service = new ExtensibleEnumValueDiscoveryService ();
    }

    [Test]
    public void GetValues ()
    {
      var types = new[] { typeof (ColorExtensions), typeof (MetallicColorExtensions), typeof (object) };

      var result = _service.GetValues (new ExtensibleEnumValues<Color> (), types).ToArray();

      Assert.That (result, Is.EquivalentTo (new[] { Color.Values.Red (), Color.Values.Green (), Color.Values.RedMetallic () }));
    }

    [Test]
    public void GetStaticTypes ()
    {
      var types = new[] { 
          typeof (object), 
          typeof (Color), 
          typeof (ColorExtensions), 
          typeof (DateTime), 
          typeof (CollectionBase),
          typeof (WrongColorValuesGeneric<>)
      };

      var result = _service.GetStaticTypes (types).ToArray();

      Assert.That (result, Is.EqualTo (new[] { typeof (ColorExtensions) }));
    }

    [Test]
    public void GetValuesForType ()
    {
      var result = _service.GetValuesForType (new ExtensibleEnumValues<Color>(), typeof (ColorExtensions)).ToArray();

      var expectedValues = new[] {
        ColorExtensions.Red (null),
        ColorExtensions.Green (null)
      };
      
      Assert.That (result, Is.EquivalentTo (expectedValues));
    }

    [Test]
    public void GetValuesForType_PassesEnumValuesToMethod ()
    {
      var values = new ExtensibleEnumValues<Color>();
      _service.GetValuesForType (values, typeof (ColorExtensions)).ToArray ();

      Assert.That (ColorExtensions.LastValues, Is.EqualTo (values));
    }

    [Test]
    public void GetValueExtensionMethods_ReturnType_MustBeExtensibleEnum ()
    {
      CheckFilteredMethods ("WrongReturnType");
    }

    [Test]
    public void GetValueExtensionMethods_ReturnType_CanBeAssignable ()
    {
      var methods = new[] { typeof (MetallicColorExtensions).GetMethod ("RedMetallic") };
      var result = _service.GetValueExtensionMethods (typeof (Color), methods).ToArray ();

      var expectedMethods = new[] {
        typeof (MetallicColorExtensions).GetMethod ("RedMetallic")
      };
      
      Assert.That (result, Is.EqualTo (expectedMethods));
    }

    [Test]
    public void GetValueExtensionMethods_Visibility_MustBePublic ()
    {
      CheckFilteredMethods ("WrongVisibility1", "WrongVisibility2");
    }

    [Test]
    public void GetValueExtensionMethods_ParameterCount_MustBeOne ()
    {
      CheckFilteredMethods ("WrongParameterCount");
    }

    [Test]
    public void GetValueExtensionMethods_Parameter_MustBeEnumValues ()
    {
      CheckFilteredMethods ("NotDerivedFromValuesClass");
    }

    [Test]
    public void GetValueExtensionMethods_Parameter_MustNotBeDerivedEnumValues ()
    {
      CheckFilteredMethods ("DerivedFromDerivedValuesClass");
    }

    [Test]
    public void GetValueExtensionMethods_MustBeExtensionMethod ()
    {
      CheckFilteredMethods ("NonExtensionMethod");
    }

    [Test]
    public void GetValueExtensionMethods_MustNotBeGeneric ()
    {
      CheckFilteredMethods ("Generic");
    }

    private void CheckFilteredMethods (params string[] methodNames)
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
      var methods = methodNames.Select (n => typeof (WrongColorValues).GetMethod (n, bindingFlags));
      var result = _service.GetValueExtensionMethods (typeof (Color), methods).ToArray ();

      Assert.That (result, Is.Empty);
    }

  }
}