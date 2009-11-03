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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums.Infrastructure
{
  public class ExtensibleEnumValueDiscoveryServiceTest
  {
    private ExtensibleEnumDefinition<Color> _definition;

    [SetUp]
    public void SetUp ()
    {
      _definition = new ExtensibleEnumDefinition<Color> (MockRepository.GenerateStub<IExtensibleEnumValueDiscoveryService> ());
    }

    [Test]
    public void GetValuesForTypes ()
    {
      var types = new[] { typeof (ColorExtensions), typeof (MetallicColorExtensions), typeof (object) };

      var result = ExtensibleEnumValueDiscoveryService.GetValuesForTypes (_definition, types).ToArray ();

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

      var result = ExtensibleEnumValueDiscoveryService.GetStaticTypes (types).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { typeof (ColorExtensions) }));
    }

    [Test]
    public void GetValuesForType ()
    {
      var result = ExtensibleEnumValueDiscoveryService.GetValuesForType (_definition, typeof (ColorExtensions)).ToArray ();

      var expectedValues = new[] {
          ColorExtensions.Red (null),
          ColorExtensions.Green (null)
      };

      Assert.That (result, Is.EquivalentTo (expectedValues));
    }

    [Test]
    public void GetValuesForType_PassesEnumValuesToMethod ()
    {
      ExtensibleEnumValueDiscoveryService.GetValuesForType (_definition, typeof (ColorExtensions)).ToArray ();

      Assert.That (ColorExtensions.LastCallArgument, Is.EqualTo (_definition));
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
      var result = ExtensibleEnumValueDiscoveryService.GetValueExtensionMethods (typeof (Color), methods).ToArray ();

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
    public void GetValueExtensionMethods_MustBeExtensionMethod ()
    {
      CheckFilteredMethods ("NonExtensionMethod");
    }

    [Test]
    public void GetValueExtensionMethods_MustNotBeGeneric ()
    {
      CheckFilteredMethods ("Generic");
    }

    [Test]
    public void GetValues ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var values = service.GetValues (new ExtensibleEnumDefinition<Color> (service));
      Assert.That (values, 
          Is.EquivalentTo (new[] { Color.Values.Red (), Color.Values.Green (), Color.Values.RedMetallic () }));
    }

    [Test]
    public void GetValues_PassesDefinition_ToExtensionMethod ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var definition = new ExtensibleEnumDefinition<Color> (service);

      service.GetValues (definition);

      Assert.That (ColorExtensions.LastCallArgument, Is.SameAs (definition));
    }

    private static void CheckFilteredMethods (params string[] methodNames)
    {
      const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
      var methods = methodNames.Select (n => typeof (WrongColorValues).GetMethod (n, bindingFlags));
      var result = ExtensibleEnumValueDiscoveryService.GetValueExtensionMethods (typeof (Color), methods).ToArray ();

      Assert.That (result, Is.Empty);
    }
  }
}