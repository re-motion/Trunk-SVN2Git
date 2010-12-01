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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class OnlyOneTablePerHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private OnlyOneTablePerHierarchyValidationRule _validationRule;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ReflectionBasedClassDefinition _baseClassDefinition;
    private ReflectionBasedClassDefinition _classDefinitionWithBaseClass;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new OnlyOneTablePerHierarchyValidationRule();
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));
      _tableDefinition = new TableDefinition (storageProviderDefinition, "TableName", null, new ColumnDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (storageProviderDefinition, null, new TableDefinition[0]);

      _baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          null,
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          true);
      _classDefinitionWithBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
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
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameDomainObject",
          "EntityName",
          StorageProviderDefinition,
          typeof (DerivedValidationDomainObjectClass),
          false);

      classDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasBaseClassAndHasNoTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasBaseClassAndHasTableDefinition_BaseClassHasNoTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_tableDefinition);
      _baseClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void HasBaseClassAndHasTableDefinition_BaseClassHasTableDefinition ()
    {
      _classDefinitionWithBaseClass.SetStorageEntity (_tableDefinition);
      _baseClassDefinition.SetStorageEntity (_tableDefinition);

      var validationResult = _validationRule.Validate (_classDefinitionWithBaseClass).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult.Length, Is.EqualTo (1));
      var expectedMessage = "Class 'DerivedValidationDomainObjectClass' must not define a table when its base class 'BaseValidationDomainObjectClass' also defines one.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }

    [Test]
    public void HasBaseClassesAndHasTableDefinition_BaseOfBaseClassHasTableDefinition ()
    {
      var baseOfBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          StorageProviderDefinition,
          typeof (BaseOfBaseValidationDomainObjectClass),
          true);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "EntityNameMatchesParentEntityNameBaseDomainObject",
          "BaseEntityName",
          StorageProviderDefinition,
          typeof (BaseValidationDomainObjectClass),
          true,
          baseOfBaseClassDefinition,
          null,
          new PersistentMixinFinderMock(typeof(DomainObject), new Type[0]));
      var classDefinitionWithBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
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
      
      var validationResult = _validationRule.Validate (classDefinitionWithBaseClass).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult.Length, Is.EqualTo (1));
      var expectedMessage = 
        "Class 'DerivedValidationDomainObjectClass' must not define a table when its base class 'BaseOfBaseValidationDomainObjectClass' also defines one.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }

    
  }
}