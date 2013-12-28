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
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums.Globalization
{
  [TestFixture]
  public class ExtensibleEnumerationGlobalizationServiceTest
  {
    private ExtensibleEnumerationGlobalizationService _service;
    private ICompoundGlobalizationService _globalizationServiceStub;
    private string _resourceValue;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceStub = MockRepository.GenerateStub<ICompoundGlobalizationService>();
      _service = new ExtensibleEnumerationGlobalizationService (_globalizationServiceStub);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
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

      Assert.That (_service.TryGetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1(), out _resourceValue), Is.True);
      Assert.That (_resourceValue, Is.EqualTo ("expected"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      _globalizationServiceStub.Stub (_ => _.GetResourceManager (Arg<ITypeInformation>.Is.NotNull)).Return (NullResourceManager.Instance);

      Assert.That (_service.TryGetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1 (), out _resourceValue), Is.False);
    }
  }
}