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
  [TestFixture]
  public class ExtensibleEnumValueDiscoveryServiceTest
  {
    private readonly MethodInfo _redMethod = typeof (ColorExtensions).GetMethod ("Red");
    private readonly MethodInfo _greenMethod = typeof (ColorExtensions).GetMethod ("Green");
    private readonly MethodInfo _redMetallicMethod = typeof (MetallicColorExtensions).GetMethod ("RedMetallic");

    private ExtensibleEnumDefinition<Color> _fakeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _fakeDefinition = new ExtensibleEnumDefinition<Color> (MockRepository.GenerateStub<IExtensibleEnumValueDiscoveryService> ());
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
    public void GetValueInfosForTypes ()
    {
      var types = new[] { typeof (ColorExtensions), typeof (MetallicColorExtensions), typeof (object) };

      var result = ExtensibleEnumValueDiscoveryService.GetValueInfosForTypes (_fakeDefinition, types).ToArray ();

      var expected = new[] { 
          new { Value = Color.Values.Red (), DeclaringMethod = _redMethod }, 
          new { Value = Color.Values.Green (), DeclaringMethod = _greenMethod }, 
          new { Value = (Color) Color.Values.RedMetallic (), DeclaringMethod = _redMetallicMethod }, };

      Assert.That (result.Select (info => new { info.Value, info.DeclaringMethod }).ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetValueInfosForType ()
    {
      var result = ExtensibleEnumValueDiscoveryService.GetValueInfosForType (_fakeDefinition, typeof (ColorExtensions)).ToArray ();

      var expected = new[] { 
          new { Value = Color.Values.Red (), DeclaringMethod = _redMethod }, 
          new { Value = Color.Values.Green (), DeclaringMethod = _greenMethod }, };

      Assert.That (result.Select (info => new { info.Value, info.DeclaringMethod }).ToArray (), Is.EquivalentTo (expected));
    }

    [Test]
    public void GetValueInfosForType_PassesEnumValuesToMethod ()
    {
      ExtensibleEnumValueDiscoveryService.GetValueInfosForType (_fakeDefinition, typeof (ColorExtensions)).ToArray ();

      Assert.That (ColorExtensions.LastCallArgument, Is.EqualTo (_fakeDefinition));
    }

    [Test]
    public void GetValueExtensionMethods_ReturnType_MustBeExtensibleEnum ()
    {
      CheckFilteredMethods ("WrongReturnType");
    }

    [Test]
    public void GetValueExtensionMethods_ReturnType_CanBeAssignable ()
    {
      var methods = new[] { _redMetallicMethod };
      var result = ExtensibleEnumValueDiscoveryService.GetValueExtensionMethods (typeof (Color), methods).ToArray ();

      var expectedMethods = new[] { _redMetallicMethod };

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
    public void GetValueInfos_Value ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var valueInfos = service.GetValueInfos (new ExtensibleEnumDefinition<Color> (service));
      Assert.That (valueInfos.Select (info => info.Value).ToArray (),
          Is.EquivalentTo (new[] { Color.Values.Red (), Color.Values.Green () }));
    }

    [Test]
    public void GetValueInfos_Method ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var valueInfos = service.GetValueInfos (new ExtensibleEnumDefinition<Color> (service));

      var declaringMethodOfGreen = valueInfos.Where (info => info.Value.ID == "Green").Single ().DeclaringMethod;
      var declaringMethodOfRed = valueInfos.Where (info => info.Value.ID == "Red").Single ().DeclaringMethod;
      
      Assert.That (declaringMethodOfGreen, Is.EqualTo (_greenMethod));
      Assert.That (declaringMethodOfRed, Is.EqualTo (_redMethod));
    }

    [Test]
    public void GetValueInfos_PassesDefinition_ToExtensionMethod ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var definition = new ExtensibleEnumDefinition<Color> (service);

      service.GetValueInfos (definition).ToArray();

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
