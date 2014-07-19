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

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithReferenceProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private IBusinessObject _businessObjectStub;
    private IBusinessObjectReferenceProperty _propertyStub;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService ();
      _businessObjectStub = MockRepository.GenerateStub<IBusinessObject> ();
      _propertyStub = MockRepository.GenerateStub<IBusinessObjectReferenceProperty> ();
    }

    [Test]
    public void Scalar_WithBusinessObjectValue ()
    {
      var valueStub = MockRepository.GenerateStub<IBusinessObject>();
      // Cannot stub ToString()
      // valueStub.Stub (_=>_.ToString()).Return ("ExpectedStringValue");
      _businessObjectStub.Stub (_=>_.GetProperty (_propertyStub)).Return (valueStub);

      string actual = _stringFormatterService.GetPropertyString (_businessObjectStub, _propertyStub, null);

      Assert.That (actual, Is.EqualTo (valueStub.ToString()));
    }

    [Test]
    public void Scalar_WithBusinessObjectWithIdentityValue ()
    {
      var valueStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity> ();
      valueStub.Stub (_ => _.DisplayName).Return ("ExpectedStringValue");
      _businessObjectStub.Stub (_=>_.GetProperty (_propertyStub)).Return (valueStub);

      string actual = _stringFormatterService.GetPropertyString (_businessObjectStub, _propertyStub, null);

      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      _businessObjectStub.Stub (_=>_.GetProperty (_propertyStub)).Return (null);

      string actual = _stringFormatterService.GetPropertyString (_businessObjectStub, _propertyStub, null);

      Assert.That (actual, Is.Empty);
    }
  }
}
