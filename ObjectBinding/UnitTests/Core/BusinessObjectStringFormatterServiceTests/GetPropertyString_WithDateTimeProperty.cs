/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithDateTimeProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectDateTimeProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockRepository = new MockRepository();
      _mockBusinessObject = _mockRepository.StrictMock<IBusinessObject>();
      _mockProperty = _mockRepository.StrictMock<IBusinessObjectDateTimeProperty>();
    }

    [Test]
    public void Scalar_WithValue ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (new DateTime (2000, 1, 1));
      _mockRepository.ReplayAll();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

        _mockRepository.VerifyAll();
        Assert.That (actual, Is.EqualTo ("01.01.2000 00:00:00"));
      }
    }

    [Test]
    public void Scalar_WithFormattableValue ()
    {
      IFormattable mockValue = _mockRepository.StrictMock<IFormattable>();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (mockValue);
      Expect.Call (mockValue.ToString ("FormatString", null)).Return ("ExpectedStringValue");
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "FormatString");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "FormatString");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.Empty);
    }
  }
}
