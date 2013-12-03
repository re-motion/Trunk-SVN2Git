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
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.UnitTests.Globalization.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class GlobalizationServiceTest
  {
    private GlobalizationService _globalizationService;
    private IResourceManagerResolver _resolverMock;
    private IResourceManager _resourceManagerStub;

    [SetUp]
    public void SetUp ()
    {
      _resourceManagerStub = MockRepository.GenerateStub<IResourceManager>();

      _resolverMock = MockRepository.GenerateStrictMock<IResourceManagerResolver>();
      _globalizationService = new GlobalizationService (_resolverMock);
    }

    [Test]
    public void GetResourceManager ()
    {
      var type = typeof (ClassWithResources);
      var typeInformation = TypeAdapter.Create (type);

      string value;
      _resourceManagerStub
        .Stub (sub => _resourceManagerStub.TryGetString ("property:Value1", out value))
        .Return (true);

      _resolverMock.Expect (mock => mock.GetResourceManager (type)).Return (_resourceManagerStub);

      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      _resolverMock.VerifyAllExpectations();
      Assert.That (resourceManager.TryGetString ("property:Value1", out value), Is.True);
    }

    [Test]
    public void GetResourceManager_WithTypeNotSupportingConversionFromITypeInformationToType ()
    {
      var typeInformation = MockRepository.GenerateStub<ITypeInformation>();

      var result = _globalizationService.GetResourceManager (typeInformation);

      _resolverMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (NullResourceManager)));
    }

    [Test]
    public void GetResourceManagerTwice_SameFromCache ()
    {
      var type = typeof (ClassWithResources);
      var typeInformation = TypeAdapter.Create (type);

      string value;
      _resourceManagerStub.Stub (sub => _resourceManagerStub.TryGetString ("property:Value1", out value)).Return (true);

      _resolverMock.Expect (mock => mock.GetResourceManager (type)).Return (_resourceManagerStub).Repeat.Once();

      var resourceManager1 = _globalizationService.GetResourceManager (typeInformation);
      var resourceManager2 = _globalizationService.GetResourceManager (typeInformation);

      Assert.That (resourceManager1, Is.SameAs (resourceManager2));
    }
  }
}