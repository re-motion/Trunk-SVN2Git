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
using System.Linq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class ObjectValidatorExtractorTest
  {
    private ObjectValidatorExtractor _extractor;
    private IObjectValidator _stubObjectValidator1;
    private IObjectValidator _stubObjectValidator2;
    private IObjectValidator _stubObjectValidator3;
    private RemovingValidatorRegistration _removingValidatorRegistration1A;
    private RemovingValidatorRegistration _removingValidatorRegistration2A;
    private RemovingValidatorRegistration _removingValidatorRegistration2B;
    private RemovingValidatorRegistration _removingValidatorRegistration2C;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration1;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration2;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration3;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration4;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration5;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub1;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub2;
    private IRemovingObjectValidationRuleCollector _removingObjectValidationRuleCollectorStub3;
    private ILogContext _logContextMock;

    [SetUp]
    public void SetUp ()
    {
      _removingValidatorRegistration1A = new RemovingValidatorRegistration (typeof (FakeCustomerValidator), null);
      _removingValidatorRegistration2A = new RemovingValidatorRegistration (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector1));
      _removingValidatorRegistration2B = new RemovingValidatorRegistration (typeof (StubObjectValidator), typeof (CustomerValidationRuleCollector2));
      _removingValidatorRegistration2C = new RemovingValidatorRegistration (typeof (StubObjectValidator), null);

      _removingObjectValidationRuleCollectorStub1 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub1.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create (typeof (Customer)));
      _removingObjectValidationRuleCollectorStub2 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub2.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Customer)));
      _removingObjectValidationRuleCollectorStub3 = MockRepository.GenerateStub<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub3.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Employee)));

      _removingObjectValidatorRegistration1 = new RemovingObjectValidatorRegistration (_removingValidatorRegistration1A, _removingObjectValidationRuleCollectorStub1);
      _removingObjectValidatorRegistration2 = new RemovingObjectValidatorRegistration (_removingValidatorRegistration2A, _removingObjectValidationRuleCollectorStub1);
      _removingObjectValidatorRegistration3 = new RemovingObjectValidatorRegistration (_removingValidatorRegistration2B, _removingObjectValidationRuleCollectorStub2);
      _removingObjectValidatorRegistration4 = new RemovingObjectValidatorRegistration (_removingValidatorRegistration1A, _removingObjectValidationRuleCollectorStub1);
      _removingObjectValidatorRegistration5 = new RemovingObjectValidatorRegistration (_removingValidatorRegistration2C, _removingObjectValidationRuleCollectorStub3);

      _stubObjectValidator1 = new FakeCustomerValidator(); //extracted
      _stubObjectValidator2 = new StubObjectValidator(); //extracted
      _stubObjectValidator3 = MockRepository.GenerateStub<IObjectValidator>(); //not extracted

      _logContextMock = MockRepository.GenerateStrictMock<ILogContext>();

      _extractor = new ObjectValidatorExtractor (
          new[]
          {
              _removingObjectValidatorRegistration1, _removingObjectValidatorRegistration2, _removingObjectValidatorRegistration3, _removingObjectValidatorRegistration4, 
              _removingObjectValidatorRegistration5
          },
          _logContextMock);
    }

    [Test]
    public void ExtractObjectValidatorsToRemove ()
    {
      var addingObjectValidationRuleCollector = MockRepository.GenerateStub<IAddingObjectValidationRuleCollector>();
      addingObjectValidationRuleCollector.Stub (stub => stub.Validators)
          .Return (new[] { _stubObjectValidator1, _stubObjectValidator3, _stubObjectValidator2 });
      addingObjectValidationRuleCollector.Stub (stub => stub.CollectorType).Return (typeof (CustomerValidationRuleCollector1));
      addingObjectValidationRuleCollector.Stub (stub => stub.ValidatedType).Return (TypeAdapter.Create(typeof (Customer)));

      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IObjectValidator>.Is.Same (_stubObjectValidator1),
                  Arg<RemovingObjectValidatorRegistration[]>.List.Equal (new[] { _removingObjectValidatorRegistration1, _removingObjectValidatorRegistration4 }),
                  Arg<IAddingObjectValidationRuleCollector>.Is.Same (addingObjectValidationRuleCollector))).Repeat.Once();
      _logContextMock.Expect (
          mock =>
              mock.ValidatorRemoved (
                  Arg<IObjectValidator>.Is.Same (_stubObjectValidator2),
                  Arg<RemovingObjectValidatorRegistration[]>.List.Equal (new[] { _removingObjectValidatorRegistration2 }),
                  Arg<IAddingObjectValidationRuleCollector>.Is.Same (addingObjectValidationRuleCollector))).Repeat.Once ();

      var result = _extractor.ExtractObjectValidatorsToRemove (addingObjectValidationRuleCollector).ToArray();

      _logContextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (new[] { _stubObjectValidator1, _stubObjectValidator2 }));
    }
  }
}