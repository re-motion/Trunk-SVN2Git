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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.RelationReflector.RelatedPropertyTypeIsNotInMapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class CheckForTypeNotFoundClassDefinitionValidationRuleTest : ValidationRuleTestBase
  {
    private CheckForTypeNotFoundClassDefinitionValidationRule _validationRule;
    
    [SetUp]
    public void SetUp ()
    {
      _validationRule = new CheckForTypeNotFoundClassDefinitionValidationRule();
    }

    [Test]
    public void RelationDefinitionWithNoTypeNotFoundClassDefinition ()
    {
      var classDefinition = new ReflectionBasedClassDefinition (
          "Test",
          "Test",
          "DefaultStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          null,
          new PersistentMixinFinder (typeof (DerivedValidationDomainObjectClass)));
      var endPoint = new AnonymousRelationEndPointDefinition (classDefinition);
      var relationDefinition = new RelationDefinition ("ID", endPoint, endPoint);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void RelationDefinitionWithTypeNotFoundClassDefinition ()
    {
      var classDefinition = new TypeNotFoundClassDefinition ("Test", "Test", "DefaultStorageProviderID", typeof (ClassOutOfInheritanceHierarchy));
      var endPoint = new AnonymousRelationEndPointDefinition (classDefinition);
      var relationDefinition = new RelationDefinition ("ID", endPoint, endPoint);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = "Mapping does not contain class 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.ClassOutOfInheritanceHierarchy'.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}