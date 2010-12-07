// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation
{
  [TestFixture]
  public class SortExpressionValidatorTest
  {
    private RelationDefinition _relationDefinition1;
    private RelationDefinition _relationDefinition2;
    private RelationDefinition _relationDefinition3;
    private IRelationDefinitionValidatorRule _validationRuleMock;
    private MappingValidationResult _fakeValidMappingValidationResult;
    private MappingValidationResult _fakeInvalidMappingValidationResult;

    [SetUp]
    public void SetUp ()
    {
      _relationDefinition1 = FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer"];
      _relationDefinition2 = FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket"];
      _relationDefinition3 = FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official"];

      _validationRuleMock = MockRepository.GenerateStrictMock<IRelationDefinitionValidatorRule> ();
      _fakeValidMappingValidationResult = MappingValidationResult.CreateValidResult();
      _fakeInvalidMappingValidationResult = MappingValidationResult.CreateInvalidResult("Test");
    }

    [Test]
    public void ValidateWithOneRelationDefinition_ValidResult ()
    {
      var validator = new SortExpressionValidator (_validationRuleMock);

      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _relationDefinition1 }).ToArray ();

      _validationRuleMock.VerifyAllExpectations ();
      Assert.That (mappingValidationResults.Length, Is.EqualTo (0));
    }

    [Test]
    public void ValidateWithOneRelationDefinition_InvalidValidResult ()
    {
      var validator = new SortExpressionValidator (_validationRuleMock);

      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _relationDefinition1 }).ToArray ();

      _validationRuleMock.VerifyAllExpectations ();
      Assert.That (mappingValidationResults.Length, Is.EqualTo (1));
    }

    [Test]
    public void ValidateWithSeveralRelationDefinitions_ValidResult ()
    {
      var validator = new SortExpressionValidator (_validationRuleMock);

      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition1)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition2)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition3)).Return (_fakeValidMappingValidationResult);
      _validationRuleMock.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _relationDefinition1, _relationDefinition2, _relationDefinition3 }).ToArray ();

      _validationRuleMock.VerifyAllExpectations ();
      Assert.That (mappingValidationResults.Length, Is.EqualTo (0));
    }

    [Test]
    public void ValidateWithSeveralRelationDefinitions_InvalidValidResult ()
    {
      var validator = new SortExpressionValidator (_validationRuleMock);

      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition1)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition2)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock.Expect (mock => mock.Validate (_relationDefinition3)).Return (_fakeInvalidMappingValidationResult);
      _validationRuleMock.Replay ();

      var mappingValidationResults = validator.Validate (new[] { _relationDefinition1, _relationDefinition2, _relationDefinition3 }).ToArray ();

      _validationRuleMock.VerifyAllExpectations ();
      Assert.That (mappingValidationResults.Length, Is.EqualTo (3));
    }

  }
}