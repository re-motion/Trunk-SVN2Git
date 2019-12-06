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
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class ExclusiveBetweenValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithPropertyValueExclusivelyBetweenBoundaries_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (3);
      var validator = new ExclusiveBetweenValidator (2, 4);

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (null);
      var validator = new ExclusiveBetweenValidator (2, 4);

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueDifferentTypeThanFrom_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (1m);
      var validator = new ExclusiveBetweenValidator (2f, 4f);

      var validationFailures = validator.Validate (propertyValidatorContext);

      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Ctor_WithFromDifferentTypeThanTo_ThrowsArgumentException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => new ExclusiveBetweenValidator (2m, 4f),
            Throws.ArgumentException.With.Message.EqualTo ($"'from' must have the same type as 'to'.{Environment.NewLine}Parameter name: to"));
      }
    }

    [Test]
    public void Ctor_WithFromGreaterThanTo_ThrowsArgumentOutOfRangeException ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => new ExclusiveBetweenValidator (2, 1),
            Throws.InstanceOf<ArgumentOutOfRangeException>()
                .With.Message.EqualTo ($"'to' should be larger than 'from'.{Environment.NewLine}Parameter name: to"));
      }
    }

    [Test]
    public void Validate_WithPropertyValueEqualsUpperBoundary_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (3);
      var validator = new ExclusiveBetweenValidator (2, 3);

      var validationFailures = validator.Validate (propertyValidatorContext).ToArray();

      Assert.That (validationFailures.Length, Is.EqualTo (1));
      Assert.That (validationFailures[0].ErrorMessage, Is.EqualTo ("Enter a value between 2 and 3 (exclusive)."));
      Assert.That (validationFailures[0].LocalizedValidationMessage, Is.EqualTo ("Enter a value between 2 and 3 (exclusive)."));
    }

    [Test]
    public void Validate_WithPropertyValueEqualsLowerBoundary_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (2);
      var validator = new ExclusiveBetweenValidator (2, 3);

      var validationFailures = validator.Validate (propertyValidatorContext).ToArray();

      Assert.That (validationFailures.Length, Is.EqualTo (1));
      Assert.That (validationFailures[0].ErrorMessage, Is.EqualTo ("Enter a value between 2 and 3 (exclusive)."));
    }

    [Test]
    public void Validate_WithPropertyValueEqualsLowerBoundaryAndCustomValidationMessage_ReturnsSingleValidationFailureWithCustomValidationMessage ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext (2);
      var validator = new ExclusiveBetweenValidator (2, 3, validationMessage: new InvariantValidationMessage ("Custom validation message: '{From}', '{To}'."));

      var validationFailures = validator.Validate (propertyValidatorContext).ToArray();

      Assert.That (validationFailures.Length, Is.EqualTo (1));
      Assert.That (validationFailures[0].ErrorMessage, Is.EqualTo ("Enter a value between 2 and 3 (exclusive)."));
      Assert.That (validationFailures[0].LocalizedValidationMessage, Is.EqualTo ("Custom validation message: '2', '3'."));
    }

    [Test]
    public void Validate_WithIComparable_CallsCompareTo ()
    {
      var propertyValueMock = MockRepository.GenerateMock<IComparable>();
      var fromStub = MockRepository.GenerateStub<IComparable>();
      var toStub = MockRepository.GenerateStub<IComparable>();
      propertyValueMock.Expect (_ => _.CompareTo (fromStub)).Return (1);
      propertyValueMock.Expect (_ => _.CompareTo (toStub)).Return (-1);
      var propertyValidatorContext = CreatePropertyValidatorContext (propertyValueMock);
      var validator = new ExclusiveBetweenValidator (fromStub, toStub);

      var validationFailures = validator.Validate (propertyValidatorContext);

      propertyValueMock.VerifyAllExpectations();
      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithIComparer_UsesComparer ()
    {
      var comparerMock = MockRepository.GenerateMock<IComparer>();
      comparerMock.Expect (_ => _.Compare (10, 1)).Return (1);
      comparerMock.Expect (_ => _.Compare (10, 3)).Return (-1);
      var propertyValidatorContext = CreatePropertyValidatorContext (10);
      var validator = new ExclusiveBetweenValidator (1, 3, comparerMock);

      var validationFailures = validator.Validate (propertyValidatorContext);

      comparerMock.VerifyAllExpectations();
      Assert.That (validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueNotImplementingIComparable_ThrowsInvalidCastException ()
    {
      var propertyValue = new object();
      var propertyValidatorContext = CreatePropertyValidatorContext (propertyValue);
      var validator = new ExclusiveBetweenValidator (2, 4);

      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That (
            () => validator.Validate (propertyValidatorContext),
            Throws.InstanceOf<InvalidCastException>()
                .With.Message.EqualTo ("Unable to cast object of type 'System.Object' to type 'System.IComparable'."));
      }
    }
  }
}