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
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums.Globalization
{
  [TestFixture]
  public class ExtensibleEnumerationServiceGlobalizationServiceTest
  {
    private ExtensibleEnumerationServiceGlobalizationService _service;

    [SetUp]
    public void SetUp ()
    {
      _service = new ExtensibleEnumerationServiceGlobalizationService();
    }

    [Test]
    public void GetExtensibleEnumerationValueDisplayName ()
    {
      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1 ()), Is.EqualTo ("Wert1"));
      Assert.That (_service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2 ()), Is.EqualTo ("Wert2"));
      Assert.That (
          _service.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource ()),
          Is.EqualTo ("Remotion.UnitTests.ExtensibleEnums.TestDomain.ExtensibleEnumWithResourcesExtensions.ValueWithoutResource"));
    }
  }
}