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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithEnumerationProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectEnumerationProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService ();
      _mockRepository = new MockRepository ();
      _mockBusinessObject = _mockRepository.StrictMock<IBusinessObject> ();
      _mockProperty = _mockRepository.StrictMock<IBusinessObjectEnumerationProperty> ();
    }

    [Test]
    public void Scalar_WithValue ()
    {
      IEnumerationValueInfo enumValueInfo = new EnumerationValueInfo (TestEnum.Value5, "Value5", "ExpectedStringValue", true);
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (TestEnum.Value5);
      Expect.Call (_mockProperty.GetValueInfoByValue (TestEnum.Value5, _mockBusinessObject)).Return (enumValueInfo);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      Expect.Call (_mockProperty.GetValueInfoByValue (null, _mockBusinessObject)).Return (null);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.Empty);
    }

    [Test]
    public void Scalar_WithUndefinedValue ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (TestEnum.Value5);
      Expect.Call (_mockProperty.GetValueInfoByValue (TestEnum.Value5, _mockBusinessObject)).Return (null);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.Empty);
    }
  }
}
