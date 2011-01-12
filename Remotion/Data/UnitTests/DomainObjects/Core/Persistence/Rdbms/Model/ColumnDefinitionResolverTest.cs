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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnDefinitionResolverTest
  {
    private ColumnDefinitionResolver _resolver;
    private ReflectionBasedClassDefinition _baseBaseClassDefinition;
    private ReflectionBasedClassDefinition _baseClassDefinition;
    private ReflectionBasedClassDefinition _tableClassDefinition1;
    private ReflectionBasedClassDefinition _tableClassDefinition2;
    private ReflectionBasedClassDefinition _derivedClassDefinition1;
    private ReflectionBasedClassDefinition _derivedClassDefinition2;
    private ReflectionBasedClassDefinition _derivedDerivedClassDefinition;
    private SimpleColumnDefinition _fakeColumnDefinition1;
    private SimpleColumnDefinition _fakeColumnDefinition2;
    private SimpleColumnDefinition _fakeColumnDefinition3;
    private SimpleColumnDefinition _fakeColumnDefinition4;
    private SimpleColumnDefinition _fakeColumnDefinition5;
    private SimpleColumnDefinition _fakeColumnDefinition6;
    private SimpleColumnDefinition _fakeColumnDefinition7;
    private ReflectionBasedPropertyDefinition _baseBasePropertyDefinition;
    private ReflectionBasedPropertyDefinition _basePropertyDefinition;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition1;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition1;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedDerivedPropertyDefinition;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ColumnDefinitionResolver();

      _baseBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Customer), null);
      _baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Folder), _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Company), _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Distributor), _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Partner), _tableClassDefinition2);
      _derivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Supplier), _derivedClassDefinition2);

      _baseBasePropertyDefinition = CreateAndAddPropertyDefinition (
          _baseBaseClassDefinition, "BaseBaseProperty", typeof (Customer).GetProperty ("CustomerSince"));
      _basePropertyDefinition = CreateAndAddPropertyDefinition (_baseClassDefinition, "BaseProperty", typeof (Folder).GetProperty ("FileSystemItems"));
      _tablePropertyDefinition1 = CreateAndAddPropertyDefinition (
          _tableClassDefinition1, "TableProperty1", typeof (Order).GetProperty ("OrderNumber"));
      _tablePropertyDefinition2 = CreateAndAddPropertyDefinition (_tableClassDefinition2, "TableProperty2", typeof (Company).GetProperty ("Name"));
      _derivedPropertyDefinition1 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition1, "DerivedProperty1", typeof (Distributor).GetProperty ("NumberOfShops"));
      _derivedPropertyDefinition2 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition2, "DerivedProperty2", typeof (Partner).GetProperty ("ContactPerson"));
      _derivedDerivedPropertyDefinition = CreateAndAddPropertyDefinition (
          _derivedDerivedClassDefinition, "DerivedDerivedProperty", typeof (Supplier).GetProperty ("SupplierQuality"));

      _baseBaseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _baseClassDefinition }, true, true));
      _baseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _tableClassDefinition1, _tableClassDefinition2 }, true, true));
      _tableClassDefinition2.SetDerivedClasses (
          new ClassDefinitionCollection (new[] { _derivedClassDefinition1, _derivedClassDefinition2 }, true, true));
      _derivedClassDefinition2.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedClassDefinition }, true, true));
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      _fakeColumnDefinition1 = new SimpleColumnDefinition ("Test1", typeof (string), "varchar", true, false);
      _fakeColumnDefinition2 = new SimpleColumnDefinition ("Test2", typeof (int), "int", false, false);
      _fakeColumnDefinition3 = new SimpleColumnDefinition ("Test3", typeof (string), "varchar", true, false);
      _fakeColumnDefinition4 = new SimpleColumnDefinition ("Test4", typeof (int), "int", false, false);
      _fakeColumnDefinition5 = new SimpleColumnDefinition ("Test5", typeof (string), "varchar", true, false);
      _fakeColumnDefinition6 = new SimpleColumnDefinition ("Test6", typeof (int), "int", false, false);
      _fakeColumnDefinition7 = new SimpleColumnDefinition ("Test7", typeof (string), "varchar", true, false);
      
      _baseBasePropertyDefinition.SetStorageProperty (_fakeColumnDefinition1);
      _basePropertyDefinition.SetStorageProperty (_fakeColumnDefinition2);
      _tablePropertyDefinition1.SetStorageProperty (_fakeColumnDefinition3);
      _tablePropertyDefinition2.SetStorageProperty (_fakeColumnDefinition4);
      _derivedPropertyDefinition1.SetStorageProperty (_fakeColumnDefinition5);
      _derivedPropertyDefinition2.SetStorageProperty (_fakeColumnDefinition6);
      _derivedDerivedPropertyDefinition.SetStorageProperty (_fakeColumnDefinition7);
    }

    [Test]
    public void GetColumnDefinitionsForHierarchy_InheritanceRoot ()
    {
      var columns = _resolver.GetColumnDefinitionsForHierarchy (_baseBaseClassDefinition).ToArray();

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
    public void GetColumnDefinitionsForHierarchy_DerivedClass ()
    {
      var columns = _resolver.GetColumnDefinitionsForHierarchy (_derivedClassDefinition1).ToArray ();

      Assert.That (columns, Is.EqualTo (new[] { _fakeColumnDefinition1, _fakeColumnDefinition2,  _fakeColumnDefinition4, _fakeColumnDefinition5 }));
    }

    [Test]
    public void CreateTableDefinition_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection ());

      var columns = _resolver.GetColumnDefinitionsForHierarchy (classDefinition).ToArray ();

      Assert.That (columns, Is.EqualTo (new IColumnDefinition[0]));
    }

    [Test]
    public void CreateTableDefinition_PropertiesWithSamePropertyInfoAreFiltered ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          "Test1",
          typeof (string),
          null,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      var propertyDefinition2 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          "Test2",
          typeof (string),
          null,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection ());
      propertyDefinition1.SetStorageProperty (_fakeColumnDefinition1);
      propertyDefinition2.SetStorageProperty (_fakeColumnDefinition2);

      var columns = _resolver.GetColumnDefinitionsForHierarchy (classDefinition).ToArray ();

      Assert.That (columns.Length, Is.EqualTo (1)); //instead of 2
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "Storage property definition has not been set.\r\n"
      + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassHavingStorageSpecificIdentifierAttribute'\r\n"
      + "Property: 'StorageSpecificName'")]
    public void CreateTableDefinition_StoragePropertyDefinitionHasNotBeenSet ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      var propertyInfo = typeof (ClassHavingStorageSpecificIdentifierAttribute).GetProperty ("StorageSpecificName");
      var propertyDefinition1 = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          "Test1",
          typeof (string),
          null,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition1 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection ());

      _resolver.GetColumnDefinitionsForHierarchy (classDefinition).ToArray ();
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer'\r\n"
        + "Property: 'CustomerSince'")]
    public void StoragePropertyDefinitionIsNoColumnDefinition ()
    {
      var fakeResult = new FakeStoragePropertyDefinition ("Invalid");
      _baseBasePropertyDefinition.SetStorageProperty (fakeResult);

      _resolver.GetColumnDefinitionsForHierarchy (_tableClassDefinition1).ToArray ();
    }

    private ReflectionBasedPropertyDefinition CreateAndAddPropertyDefinition (
        ReflectionBasedClassDefinition classDefinition, string propertyName, PropertyInfo propertyInfo)
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          propertyName,
          typeof (string),
          null,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }
  }
}