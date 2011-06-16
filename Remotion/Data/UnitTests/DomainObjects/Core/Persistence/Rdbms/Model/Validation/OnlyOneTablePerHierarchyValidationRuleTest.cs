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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Validation
{
  [TestFixture]
  public class OnlyOneTablePerHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private OnlyOneTablePerHierarchyValidationRule _validationRule;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ClassDefinition _baseClassDefinition;
    private ClassDefinition _classDefinitionWithBaseClass;
    private SimpleColumnDefinition _objectIDColunmn;
    private SimpleColumnDefinition _classIDCOlumn;
    private SimpleColumnDefinition _timestampColumn;

    [SetUp]
    public void SetUp ()
    {
      _objectIDColunmn = new SimpleColumnDefinition ("ObjectID", typeof (int), "integer", false, true);
      _classIDCOlumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      _timestampColumn = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);

      _validationRule = new OnlyOneTablePerHierarchyValidationRule();
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider");
      _tableDefinition = TableDefinitionObjectMother.Create (
          storageProviderDefinition,
          new EntityNameDefinition (null, "TableName"),
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn);
      _unionViewDefinition = new UnionViewDefinition (
          storageProviderDefinition,
          null,
          new IEntityDefinition[]
          {
              TableDefinitionObjectMother.Create (
              storageProviderDefinition,
              new EntityNameDefinition (null, "Test"),
              _objectIDColunmn,
              _classIDCOlumn,
              _timestampColumn)
          },
          _objectIDColunmn,
          _classIDCOlumn,
          _timestampColumn,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);

      _baseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          null,
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          true);
      _classDefinitionWithBaseClass = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false,
          _baseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));
    }

    [Test]
    public void HasNoBaseClassAndHasNoTableDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false);

      classDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClassAndHasNoTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClassAndHasTableDefinition_BaseClassHasNoTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_tableDefinition);
      _baseClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClassAndHasTableDefinition_BaseClassHasTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_tableDefinition);
      _baseClassDefinition.SetStorageEntity (_tableDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass);

      var expectedMessage =
          "Class 'DerivedValidationDomainObjectClass' must not define a table when its base class 'BaseValidationDomainObjectClass' also defines one.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void HasBaseClassesAndHasTableDefinition_BaseOfBaseClassHasTableDefinition ()
    {
      var baseOfBaseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          StorageProviderDefinition,
          typeof (BaseOfBaseValidationDomainObjectClass),
          true);
      var baseClassDefinition = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          true,
          baseOfBaseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));
      var classDefinitionWithBaseClass = ClassDefinitionFactory.CreateClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false,
          baseClassDefinition,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      classDefinitionWithBaseClass.SetStorageEntity (_tableDefinition);
      baseClassDefinition.SetStorageEntity (_unionViewDefinition);
      baseOfBaseClassDefinition.SetStorageEntity (_tableDefinition);

      var validationResult = _validationRule.Validate (classDefinitionWithBaseClass);

      var expectedMessage =
          "Class 'DerivedValidationDomainObjectClass' must not define a table when its base class 'BaseOfBaseValidationDomainObjectClass' also defines one.\r\n\r\n"
          + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}