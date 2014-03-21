﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using FluentValidation.Resources;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class ErrorMessageStringSourceTest
  {
    private IErrorMessageGlobalizationService _validatorGlobalizationServiceMock;
    private ErrorMessageStringSource _stringSource;
    private NotNullValidator _propertyValidator;

    [SetUp]
    public void SetUp ()
    {
      _validatorGlobalizationServiceMock = MockRepository.GenerateStrictMock<IErrorMessageGlobalizationService>();
      _propertyValidator = new NotNullValidator();

      _stringSource = new ErrorMessageStringSource (_propertyValidator, _validatorGlobalizationServiceMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_stringSource.ResourceName, Is.EqualTo (_propertyValidator.GetType().FullName));
      Assert.That (_stringSource.ResourceType, Is.EqualTo (_validatorGlobalizationServiceMock.GetType()));
    }

    [Test]
    public void GetString_ErrorMessageServiceReturnsNotNull_ReturnsErrorMessageFromService ()
    {
      _validatorGlobalizationServiceMock.Expect (mock => mock.GetErrorMessage (_propertyValidator)).Return ("FakeMessage");

      var result = _stringSource.GetString();

      _validatorGlobalizationServiceMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("FakeMessage"));
    }

    [Test]
    public void GetString_ErrorMessageServiceReturnsNull_ReturnsOriginalErrorMessage ()
    {
      _validatorGlobalizationServiceMock.Expect (mock => mock.GetErrorMessage (_propertyValidator)).Return (null);

      var result = _stringSource.GetString();

      _validatorGlobalizationServiceMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (Messages.notnull_error));
    }

    [Test]
    public void GetString_DoesNotCacheOriginalErrorMessage ()
    {
      _validatorGlobalizationServiceMock.Stub (mock => mock.GetErrorMessage (_propertyValidator)).Return (null);

      using (CultureScope.CreateInvariantCultureScope())
      {
        var resultInvariant = _stringSource.GetString();
        Assert.That (resultInvariant, Is.EqualTo (Messages.notnull_error));
        using (new CultureScope ("de-At"))
        {
          var resultDE = _stringSource.GetString();
          Assert.That (resultDE, Is.EqualTo (Messages.notnull_error));
          Assert.That (resultDE, Is.Not.EqualTo (resultInvariant), "DE resources appear to be missing");
        }
      }
    }

    [Test]
    public void GetString_ErrorMessageServiceChangesErrorMessageSource_ThrowsInvalidOperationException()
    {
      _validatorGlobalizationServiceMock.Expect (mock => mock.GetErrorMessage (_propertyValidator))
          .WhenCalled (mi => ((IPropertyValidator) mi.Arguments[0]).ErrorMessageSource = new StaticStringSource ("Wrong Message"))
          .Return (null);

      Assert.That (
          () => _stringSource.GetString(),
          Throws.InvalidOperationException.And.Message.EqualTo (
              string.Format ("ErrorMessageSource of PropertyValidator has been changed by invocation of '{0}'.GetErrorMessage (...).",
              _validatorGlobalizationServiceMock.GetType().FullName)));
    }
  }
}