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
  public class NonAbstractClassHasEntityNameValidationRuleTest : ValidationRuleTestBase
  {
    private NonAbstractClassHasEntityNameValidationRule _validationRule;
    private ReflectionBasedClassDefinition _abstractClassDefinition;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ReflectionBasedClassDefinition _noAbstractClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new NonAbstractClassHasEntityNameValidationRule();
      _abstractClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          null,
          typeof (DerivedValidationDomainObjectClass),
          true);
      _noAbstractClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameDomainObject",
          "EntityName",
          typeof (DerivedValidationDomainObjectClass),
          false);
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));
      _tableDefinition = new TableDefinition (storageProviderDefinition, "TableName", null, new ColumnDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (storageProviderDefinition, null, new TableDefinition[0]);
    }

    [Test]
    public void ClassTypeUnresolved ()
    {
      var classDefinition = new ReflectionBasedClassDefinitionWithUnresolvedClassType (
          "NonAbstractClassHasEntityNameDomainObject",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (DerivedValidationDomainObjectClass),
          false,
          null,
          new PersistentMixinFinderMock (typeof (DomainObject), new Type[0]));

      var validationResult = _validationRule.Validate (classDefinition).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void ClassTypeResolved_NoUnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity (_tableDefinition);

      var validationResult = _validationRule.Validate (_abstractClassDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_Abstract ()
    {
      _abstractClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_abstractClassDefinition).Where (result => !result.IsValid).ToArray ();

      Assert.That (validationResult, Is.Empty);
    }

    [Test]
    public void ClassTypeResolved_UnionViewDefinition_NotAbstract ()
    {
      _noAbstractClassDefinition.SetStorageEntity (_unionViewDefinition);

      var validationResult = _validationRule.Validate (_noAbstractClassDefinition).Where (result => !result.IsValid).ToArray();

      Assert.That (validationResult.Length, Is.EqualTo(1));

      var expectedMessage = "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
        +"Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of it's base classes.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass";
      AssertMappingValidationResult (validationResult[0], false, expectedMessage);
    }
    
  }
}