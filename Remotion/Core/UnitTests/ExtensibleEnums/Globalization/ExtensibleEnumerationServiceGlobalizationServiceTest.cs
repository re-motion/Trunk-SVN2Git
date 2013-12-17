// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.ExtensibleEnums.Globalization;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums.Globalization
{
  [TestFixture]
  public class ExtensibleEnumerationServiceGlobalizationServiceTest
  {
    private ExtensibleEnumerationServiceGlobalizationService _service;
    private ICompoundGlobalizationService _globalizationServiceStub;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = MockRepository.GenerateStub<ICompoundGlobalizationService>();
      _service = new ExtensibleEnumerationServiceGlobalizationService (_globalizationServiceStub);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      _globalizationServiceStub
          .Stub (_ => _.GetResourceManager (TypeAdapter.Create (typeof (ExtensibleEnumWithResourcesExtensions))))
          .Return (resourceManagerStub);
      resourceManagerStub
          .Stub (
              _ => _.TryGetString (
                  Arg.Is ("Remotion.UnitTests.ExtensibleEnums.TestDomain.ExtensibleEnumWithResourcesExtensions.Value1"),
                  out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1()), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_ReturnsValueName ()
    {
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (NullResourceManager.Instance);

      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1()), Is.EqualTo ("Value1"));
    }

    [Test]
    public void GetExtensibleEnumerationValueDisplayName_IntegrationTest ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IExtensibleEnumerationGlobalizationService>();
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1()), Is.EqualTo ("Wert1"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2()), Is.EqualTo ("Wert2"));
      Assert.That (
          service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource()),
          Is.EqualTo ("ValueWithoutResource"));

      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.Red()), Is.EqualTo ("Rot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.Green()), Is.EqualTo ("Grün"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.RedMetallic()), Is.EqualTo ("RedMetallic"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.LightRed()), Is.EqualTo ("Hellrot"));
      Assert.That (service.GetExtensibleEnumerationValueDisplayName (Color.Values.LightBlue()), Is.EqualTo ("LightBlue"));
    }
  }
}