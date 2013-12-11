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
using System.Globalization;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.UnitTests.Globalization.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class EnumerationGlobalizationServiceTest
  {
    private EnumerationGlobalizationService _service;
    private ICompoundGlobalizationService _globalizationServiceStub;
    private IMemberInformationNameResolver _memberInformationNameResolverStub;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = MockRepository.GenerateStub<ICompoundGlobalizationService>();
      _memberInformationNameResolverStub = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      _service = new EnumerationGlobalizationService (_globalizationServiceStub, _memberInformationNameResolverStub);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (resourceManagerStub);
      _memberInformationNameResolverStub.Stub (_ => _.GetEnumName (EnumWithResources.Value1)).Return ("enumName");
      resourceManagerStub.Stub (
          _ => _.TryGetString (
              Arg.Is ("enumName"),
              out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_ResourceIdIsUnknown_FallsBackToEnumValueName ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (resourceManagerStub);
      _memberInformationNameResolverStub.Stub (_ => _.GetEnumName (EnumWithResources.Value1)).Return ("enumName");
      resourceManagerStub.Stub (
          _ => _.TryGetString (
              Arg.Is ("enumName"),
              out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value1"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_CachesResourceManager ()
    {
      var resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();
      resourceManagerStub.Stub (_ => _.IsNull).Return (false);
      _memberInformationNameResolverStub.Stub (_ => _.GetEnumName (EnumWithResources.Value1)).Return ("enumName");
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull))
          .Return (resourceManagerStub)
          .Repeat.Once();

      resourceManagerStub.Stub (
          _ => _.TryGetString (
              Arg.Is ("enumName"),
              out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("expected"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_UsesEnumDescriptions ()
    {
      SetupNullResourceManager();

      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescriptions.Value1), Is.EqualTo ("Value One"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescriptions.Value2), Is.EqualTo ("Value 2"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescriptions.Value3), Is.EqualTo ("Value III"));
      Assert.That (_service.GetEnumerationValueDisplayName (EnumWithDescriptions.Value4), Is.EqualTo ("Value4"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_NoEnumDescription_FallsBackToValueName ()
    {
      SetupNullResourceManager();

      Assert.That (_service.GetEnumerationValueDisplayName ((EnumWithDescriptions) 100), Is.EqualTo ("100"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_UsingCultureScope ()
    {
      var service = new EnumerationGlobalizationService (
          SafeServiceLocator.Current.GetInstance<ICompoundGlobalizationService>(),
          SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>());

      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value1), Is.EqualTo ("Wert Eins"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value2), Is.EqualTo ("Wert 2"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value3), Is.EqualTo ("Wert III"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value4), Is.EqualTo ("Value4"));
        Assert.That (service.GetEnumerationValueDisplayName ((EnumFromResource) 100), Is.EqualTo ("100"));
      }

      var culture = new CultureInfo ("en-US");
      using (new CultureScope (culture, culture))
      {
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value1), Is.EqualTo ("Val1"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value2), Is.EqualTo ("Val2"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value3), Is.EqualTo ("Val3"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumFromResource.Value4), Is.EqualTo ("Value4"));
        Assert.That (service.GetEnumerationValueDisplayName ((EnumFromResource) 100), Is.EqualTo ("100"));
      }
    }

    private void SetupNullResourceManager ()
    {
      _service = new EnumerationGlobalizationService (
          _globalizationServiceStub,
          SafeServiceLocator.Current.GetInstance<IMemberInformationNameResolver>());
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (NullResourceManager.Instance);
    }
  }
}