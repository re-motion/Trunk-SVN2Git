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
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Globalization;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums.Globalization
{
  [TestFixture]
  public class ExtensibleEnumerationGlobalizationServiceExtensionsTest
  {
    private IExtensibleEnumerationGlobalizationService _serviceStub;
    private IExtensibleEnum _valueStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceStub = MockRepository.GenerateStub<IExtensibleEnumerationGlobalizationService>();
      _valueStub = MockRepository.GenerateStub<IExtensibleEnum>();
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.GetExtensibleEnumerationValueDisplayName (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.GetExtensibleEnumerationValueDisplayNameOrDefault (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.ContainsExtensibleEnumerationValueDisplayName (_valueStub), Is.True);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_ReturnsValueName ()
    {
      _valueStub.Stub (_ => _.ValueName).Return ("expected");
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.GetExtensibleEnumerationValueDisplayName (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithoutResourceManager_ReturnsNull ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.GetExtensibleEnumerationValueDisplayNameOrDefault (_valueStub), Is.Null);
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumerationValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.ContainsExtensibleEnumerationValueDisplayName (_valueStub), Is.False);
    }
  }
}