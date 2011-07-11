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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class ClassAboveTableIsAbstractValidationRuleTest : ValidationRuleTestBase
  {
    private ClassAboveTableIsAbstractValidationRule _validationRule;
    private ClassDefinition _abstractClassDefinition;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ClassDefinition _noAbstractClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new ClassAboveTableIsAbstractValidationRule();
      _abstractClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          true);
      _noAbstractClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          "EntityName",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false);
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider");
      _tableDefinition = TableDefinitionObjectMother.Create (
          storageProviderDefinition,
          new EntityNameDefinition (null, "TableName"),
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
      _unionViewDefinition = new UnionViewDefinition (
          storageProviderDefinition,
          null,
          new IEntityDefinition[]
          {
              TableDefinitionObjectMother.Create (
              storageProviderDefinition,
              new EntityNameDefinition (null, "Test"),
              ColumnDefinitionObjectMother.ObjectIDColumn,
              ColumnDefinitionObjectMother.ClassIDColumn,
              ColumnDefinitionObjectMother.TimestampColumn)
          },
          ColumnDefinitionObjectMother.ObjectIDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new ColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
    }

    [Test]
    public void ClassTypeUnresolved ()
    {
      var classDefinition = new ClassDefinitionWithUnresolvedClassType (
          "NonAbstractClassHasEntityNameDomainObject",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_NoUnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity (_tableDefinition);

      var validationResult = _validationRule.Validate (_abstractClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_abstractClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_NotAbstract ()
    {
      _noAbstractClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_noAbstractClassDefinition);

      var expectedMessage = "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
                            + "Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of its base classes.\r\n\r\n"
                            +
                            "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}