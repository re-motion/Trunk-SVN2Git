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
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRuleTest : ValidationRuleTestBase
  {
    private EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule _validationRule;
    private ReflectionBasedClassDefinition _baseOfBaseClass;
    private ReflectionBasedClassDefinition _derivedBaseClass1;
    private ReflectionBasedClassDefinition _derivedClass;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new EntityNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule();

      _baseOfBaseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "BaseOfBase",
          null,
          typeof (BaseOfBaseValidationDomainObjectClass),
          false);
      _derivedBaseClass1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedBase1",
          null,
          typeof (BaseValidationDomainObjectClass),
          false,
          _baseOfBaseClass);
      _derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Derived",
          "Derived",
          typeof (DerivedValidationDomainObjectClass),
          false,
          _derivedBaseClass1);

      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));
      _tableDefinition1 = new TableDefinition (storageProviderDefinition, "TableName1", null, new ColumnDefinition[0]);
      _tableDefinition2 = new TableDefinition (storageProviderDefinition, "TableName2", null, new ColumnDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (storageProviderDefinition, null, new TableDefinition[0]);
    }

    [Test]
    public void NoInheritanceRoot ()
    {
      var validationResult = _validationRule.Validate (_derivedBaseClass1).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void InheritanceRoot_NoTableDefinition()
    {
      _baseOfBaseClass.SetStorageEntity (_unionViewDefinition);
      _derivedBaseClass1.SetStorageEntity (_unionViewDefinition);
      _derivedClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_baseOfBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }


    [Test]
    public void InheritanceRoot_UniqueTableDefinition ()
    {
      _baseOfBaseClass.SetStorageEntity (_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity (_unionViewDefinition);
      _derivedClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_baseOfBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void InheritanceRoot_DifferentTableNameInDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity (_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity (_tableDefinition2);
      _derivedClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_baseOfBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void InheritanceRoot_SameTableNameInDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity (_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity (_tableDefinition1);
      _derivedClass.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_baseOfBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult.Length, Is.EqualTo (1));
      var expectedMessage = "At least two classes in different inheritance branches derived from abstract class 'BaseOfBaseValidationDomainObjectClass' "
        +"specify the same entity name 'TableName1', which is not allowed.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }

    [Test]
    public void InheritanceRoot_SameTableNameInDerivedDerivedClass ()
    {
      _baseOfBaseClass.SetStorageEntity (_tableDefinition1);
      _derivedBaseClass1.SetStorageEntity (_unionViewDefinition);
      _derivedClass.SetStorageEntity (_tableDefinition1);

      var validationResult = _validationRule.Validate (_baseOfBaseClass).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult.Length, Is.EqualTo (1));
      var expectedMessage = "At least two classes in different inheritance branches derived from abstract class 'BaseOfBaseValidationDomainObjectClass' "
        + "specify the same entity name 'TableName1', which is not allowed.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.BaseOfBaseValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }
    

  }
}