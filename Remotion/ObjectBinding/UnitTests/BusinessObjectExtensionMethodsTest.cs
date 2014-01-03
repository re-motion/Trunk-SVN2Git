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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectExtensionMethodsTest
  {
    private MockRepository _mockRepository;
    private IBusinessObject _businessObjectMock;
    private IBusinessObjectProperty _propertyMock;
    private IBusinessObjectClass _classMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _businessObjectMock = _mockRepository.StrictMock<IBusinessObject>();
      _classMock = _mockRepository.StrictMock<IBusinessObjectClass>();
      _propertyMock = _mockRepository.StrictMock<IBusinessObjectProperty>();
    }

    [Test]
    public void GetProperty ()
    {
      object exptected = new object();

      Expect.Call (_businessObjectMock.BusinessObjectClass).Return (_classMock);
      Expect.Call (_classMock.GetPropertyDefinition ("TheProperty")).Return (_propertyMock);
      Expect.Call (_businessObjectMock.GetProperty (_propertyMock)).Return (exptected);
      _mockRepository.ReplayAll();

      object actual = _businessObjectMock.GetProperty ("TheProperty");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo (exptected));
    }

    [Test]
    public void SetProperty ()
    {
      object exptected = new object ();

      Expect.Call (_businessObjectMock.BusinessObjectClass).Return (_classMock);
      Expect.Call (_classMock.GetPropertyDefinition ("TheProperty")).Return (_propertyMock);
      _businessObjectMock.SetProperty (_propertyMock, exptected);
      _mockRepository.ReplayAll ();

      _businessObjectMock.SetProperty ("TheProperty", exptected);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetPropertyString ()
    {
      string exptected = "TheValue";

      Expect.Call (_businessObjectMock.BusinessObjectClass).Return (_classMock);
      Expect.Call (_classMock.GetPropertyDefinition ("TheProperty")).Return (_propertyMock);
      Expect.Call (_businessObjectMock.GetPropertyString (_propertyMock, null)).Return (exptected);
      _mockRepository.ReplayAll ();

      string actual = _businessObjectMock.GetPropertyString ("TheProperty");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo (exptected));
    }
    
    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The business object's class ('TheClass') does not contain a property named 'InvalidProperty'.")]
    public void GetProperty_WithInvalidPropertyIdentifier ()
    {
      Expect.Call (_businessObjectMock.BusinessObjectClass).Return (_classMock);
      Expect.Call (_classMock.GetPropertyDefinition ("InvalidProperty")).Return (null);
      Expect.Call (_classMock.Identifier).Return ("TheClass");
      _mockRepository.ReplayAll();

      _businessObjectMock.GetProperty ("InvalidProperty");

      _mockRepository.VerifyAll();
      Assert.Fail();
    }
  }
}
