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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class ColumnDefinitionResolverTest
  {
    private ColumnDefinitionResolver _resolver;
    private SimpleColumnDefinition _fakeColumnDefinition1;
    private SimpleColumnDefinition _fakeColumnDefinition2;
    private SimpleColumnDefinition _fakeColumnDefinition3;
    private SimpleColumnDefinition _fakeColumnDefinition4;
    private SimpleColumnDefinition _fakeColumnDefinition5;
    private SimpleColumnDefinition _fakeColumnDefinition6;
    private SimpleColumnDefinition _fakeColumnDefinition7;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ColumnDefinitionResolver();
      _testModel = new RdbmsPersistenceModelLoaderTestHelper ();
      
      _fakeColumnDefinition1 = ColumnDefinitionObjectMother.CreateColumn("Test1");
      _fakeColumnDefinition2 = ColumnDefinitionObjectMother.CreateColumn ("Test2");
      _fakeColumnDefinition3 = ColumnDefinitionObjectMother.CreateColumn ("Test3");
      _fakeColumnDefinition4 = ColumnDefinitionObjectMother.CreateColumn ("Test4");
      _fakeColumnDefinition5 = ColumnDefinitionObjectMother.CreateColumn ("Test5");
      _fakeColumnDefinition6 = ColumnDefinitionObjectMother.CreateColumn ("Test6");
      _fakeColumnDefinition7 = ColumnDefinitionObjectMother.CreateColumn ("Test7");
      
      _testModel.BaseBasePropertyDefinition.SetStorageProperty (_fakeColumnDefinition1);
      _testModel.BasePropertyDefinition.SetStorageProperty (_fakeColumnDefinition2);
      _testModel.TablePropertyDefinition1.SetStorageProperty (_fakeColumnDefinition3);
      _testModel.TablePropertyDefinition2.SetStorageProperty (_fakeColumnDefinition4);
      _testModel.DerivedPropertyDefinition1.SetStorageProperty (_fakeColumnDefinition5);
      _testModel.DerivedPropertyDefinition2.SetStorageProperty (_fakeColumnDefinition6);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (_fakeColumnDefinition7);
    }

    [Test]
    public void GetColumnDefinitionsForHierarchy_GetsColumnsFromDerivedClasses_SortedFromBaseToDerived ()
    {
      var columns = _resolver.GetColumnDefinitionsForHierarchy (_testModel.BaseBaseClassDefinition).ToArray();

      Assert.That (
          columns,
          Is.EqualTo (
              new[]
              {
                  _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition5,
                  _fakeColumnDefinition6, _fakeColumnDefinition7
              }));
    }

    [Test]
    public void GetColumnDefinitionsForHierarchy_AlsoGetsColumnsFromBaseClasses_SortedFromBaseToDerived ()
    {
      var columns = _resolver.GetColumnDefinitionsForHierarchy (_testModel.DerivedClassDefinition1).ToArray ();

      Assert.That (columns, Is.EqualTo (new[] { _fakeColumnDefinition1, _fakeColumnDefinition2,  _fakeColumnDefinition4, _fakeColumnDefinition5 }));
    }

    [Test]
    public void GetColumnDefinitionsForHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      var columns = _resolver.GetColumnDefinitionsForHierarchy (classDefinition).ToArray ();

      Assert.That (columns, Is.EqualTo (new IColumnDefinition[0]));
    }

    [Test]
    public void GetColumnDefinitionsForHierarchy_PropertiesWithSamePropertyInfoAreFiltered ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test1", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      var propertyDefinition2 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test2", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      propertyDefinition1.SetStorageProperty (_fakeColumnDefinition1);
      propertyDefinition2.SetStorageProperty (_fakeColumnDefinition2);

      var columns = _resolver.GetColumnDefinitionsForHierarchy (classDefinition).ToArray ();

      Assert.That (columns.Length, Is.EqualTo (1)); //instead of 2
    }

    [Test]
    public void GetColumnDefinition ()
    {
      var result = _resolver.GetColumnDefinition (_testModel.BaseBasePropertyDefinition);

      Assert.That (result, Is.SameAs (_fakeColumnDefinition1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "Storage property definition has not been set.\r\n"
      + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute'\r\n"
      + "Property: 'StorageSpecificName'")]
    public void GetColumnDefinition_CreateTableDefinition_StoragePropertyDefinitionHasNotBeenSet ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = PropertyDefinitionFactory.Create (
          classDefinition,
          "Test1", 
          false,
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      _resolver.GetColumnDefinition (propertyDefinition1);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.RdbmsPersistenceModelLoaderTestDomain.BaseBaseClass'\r\n"
        + "Property: 'BaseBaseProperty'")]
    public void GetColumnDefinition_StoragePropertyDefinitionIsNoColumnDefinition ()
    {
      var fakeResult = new FakeStoragePropertyDefinition ("Invalid");
      _testModel.BaseBasePropertyDefinition.SetStorageProperty (fakeResult);

      _resolver.GetColumnDefinition (_testModel.BaseBasePropertyDefinition);
    }
  }
}