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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;
using Customer = Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer;
using File = Remotion.Data.UnitTests.DomainObjects.TestDomain.File;
using Folder = Remotion.Data.UnitTests.DomainObjects.TestDomain.Folder;
using Order = Remotion.Data.UnitTests.DomainObjects.TestDomain.Order;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderTest
  {
    private string _storageProviderID;

    private ReflectionBasedClassDefinition _baseBaseClassDefinition;
    private ReflectionBasedClassDefinition _baseClassDefinition;
    private ReflectionBasedClassDefinition _tableClassDefinition1;
    private ReflectionBasedClassDefinition _tableClassDefinition2;
    private ReflectionBasedClassDefinition _derivedClassDefinition1;
    private ReflectionBasedClassDefinition _derivedClassDefinition2;
    private ReflectionBasedClassDefinition _derivedDerivedClassDefinition;
    private ReflectionBasedClassDefinition _derivedDerivedDerivedClassDefinition;

    private ReflectionBasedPropertyDefinition _baseBasePropertyDefinition;
    private ReflectionBasedPropertyDefinition _basePropertyDefinition;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition1;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition1;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedDerivedPropertyDefinition;

    private IColumnDefinitionFactory _columnDefinitionFactoryMock;
    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;

    private SimpleColumnDefinition _fakeColumnDefinition1;
    private SimpleColumnDefinition _fakeColumnDefinition2;
    private SimpleColumnDefinition _fakeColumnDefinition3;
    private SimpleColumnDefinition _fakeColumnDefinition4;
    private SimpleColumnDefinition _fakeColumnDefinition5;
    private SimpleColumnDefinition _fakeColumnDefinition6;
    private SimpleColumnDefinition _fakeColumnDefinition7;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private IDColumnDefinition _fakeIDColumnDefinition;
    private SimpleColumnDefinition _fakeTimestampColumnDefinition;

    // Test Domain:
    //
    //                 BaseBase
    //                     |
    //                   Base
    //                 /      \
    //            Table1       Table2
    //                         /    \
    //                   Derived1  Derived2
    //                                |
    //                          DerivedDerived
    //                                |
    //                       DerivedDerivedDerived
    //
    // All Base classes are persisted as UnionViewDefinitions, all Tables as TableDefinitions, all Derived as FilterViewDefinitions.

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID, typeof (UnitTestStorageObjectFactoryStub));
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);

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
      _derivedDerivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (File), _derivedDerivedClassDefinition);

      _baseBaseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _baseClassDefinition }, true, true));
      _baseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _tableClassDefinition1, _tableClassDefinition2 }, true, true));
      _tableClassDefinition2.SetDerivedClasses (
          new ClassDefinitionCollection (new[] { _derivedClassDefinition1, _derivedClassDefinition2 }, true, true));
      _derivedClassDefinition2.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedClassDefinition }, true, true));
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedDerivedClassDefinition }, true, true));
      _derivedDerivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection());

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
      _derivedDerivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      _fakeColumnDefinition1 = new SimpleColumnDefinition ("Test1", typeof (string), "varchar", true);
      _fakeColumnDefinition2 = new SimpleColumnDefinition ("Test2", typeof (int), "int", false);
      _fakeColumnDefinition3 = new SimpleColumnDefinition ("Test3", typeof (string), "varchar", true);
      _fakeColumnDefinition4 = new SimpleColumnDefinition ("Test4", typeof (int), "int", false);
      _fakeColumnDefinition5 = new SimpleColumnDefinition ("Test5", typeof (string), "varchar", true);
      _fakeColumnDefinition6 = new SimpleColumnDefinition ("Test6", typeof (int), "int", false);
      _fakeColumnDefinition7 = new SimpleColumnDefinition ("Test7", typeof (string), "varchar", true);
      _fakeIDColumnDefinition = new IDColumnDefinition (_fakeColumnDefinition1, _fakeColumnDefinition2);
      _fakeTimestampColumnDefinition = new SimpleColumnDefinition ("Timestamp", typeof (object), "rowversion", false);

      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IColumnDefinitionFactory>();
      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _columnDefinitionFactoryMock, _storageProviderDefinition);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var validator = (PersistenceMappingValidator) _rdbmsPersistenceModelLoader.CreatePersistenceMappingValidator (_baseBaseClassDefinition);

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (5));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (OnlyOneTablePerHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (ClassAboveTableIsAbstractValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (ColumnNamesAreUniqueWithinInheritanceTreeValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (PropertyTypeIsSupportedByStorageProviderValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_AlreadyHasPersistenceModelApplied_AndNoDerivedClassDefinitions ()
    {
      var storageEntityDefinition = new TableDefinition (_storageProviderDefinition, "Test", "TestView", new SimpleColumnDefinition[] { });
      _derivedDerivedDerivedClassDefinition.SetStorageEntity (storageEntityDefinition);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedDerivedClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (_derivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (storageEntityDefinition));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseBaseClassDefinition ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition1))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition6);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition7);
      MockSpecialColumns (8);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_baseBaseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertUnionViewDefinition (
          _baseBaseClassDefinition,
          _storageProviderID,
          "CustomerView",
          new[] { _baseClassDefinition.StorageEntityDefinition },
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3,
              _fakeColumnDefinition4, _fakeColumnDefinition5, _fakeColumnDefinition6, _fakeColumnDefinition7
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinition ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition1))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition6);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition7);
      MockSpecialColumns (7);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_baseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertUnionViewDefinition (
          _baseClassDefinition,
          _storageProviderID,
          "FolderView",
          new[] { _tableClassDefinition1.StorageEntityDefinition, _tableClassDefinition2.StorageEntityDefinition },
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3,
              _fakeColumnDefinition4, _fakeColumnDefinition5, _fakeColumnDefinition6, _fakeColumnDefinition7
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition1 ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition1))
          .Return (_fakeColumnDefinition3);
      MockSpecialColumns (1);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          _tableClassDefinition1,
          _storageProviderID,
          "Order",
          "OrderView",
          new IColumnDefinition[]
          { _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3 });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition2 ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition6);
      MockSpecialColumns (5);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition2);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          _tableClassDefinition2,
          _storageProviderID,
          "Company",
          "CompanyView",
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3,
              _fakeColumnDefinition4, _fakeColumnDefinition5, _fakeColumnDefinition6
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition1 ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition6);
      MockSpecialColumns (2);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedClassDefinition1,
          _storageProviderID,
          "DistributorView",
          _tableClassDefinition2.StorageEntityDefinition,
          new[] { "Distributor" },
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2,
              _fakeColumnDefinition3, _fakeColumnDefinition4
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition2 ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition6);
      MockSpecialColumns (4);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedClassDefinition2);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedClassDefinition2,
          _storageProviderID,
          "PartnerView",
          _tableClassDefinition2.StorageEntityDefinition,
          new[] { "Partner", "Supplier", "File" },
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2,
              _fakeColumnDefinition3, _fakeColumnDefinition5, _fakeColumnDefinition6
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedDerivedClassDefinition ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_baseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_basePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_tablePropertyDefinition2))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition1))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition6);
      MockSpecialColumns (4);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedDerivedClassDefinition,
          _storageProviderID,
          "SupplierView",
          _derivedClassDefinition2.StorageEntityDefinition,
          new[] { "Supplier", "File" },
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2,
              _fakeColumnDefinition3, _fakeColumnDefinition5, _fakeColumnDefinition6
          });
    }

    [Test]
    public void ApplyPersistentModelToHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      MockSpecialColumns (1);

      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          classDefinition,
          _storageProviderID,
          "Order",
          "OrderView",
          new IColumnDefinition[] { _fakeIDColumnDefinition, _fakeTimestampColumnDefinition });
    }

    [Test]
    public void ApplyPersistentModelToHierarchy_TableNameOfTheDBTableAttributeIsUsed ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      MockSpecialColumns (1);
      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          classDefinition,
          _storageProviderID,
          "ClassHavingStorageSpecificIdentifierAttributeTable",
          "ClassHavingStorageSpecificIdentifierAttributeView",
          new IColumnDefinition[] { _fakeIDColumnDefinition, _fakeTimestampColumnDefinition });
    }

    [Test]
    public void ApplyPersistentModelToHierarchy_PropertiesWithSamePropertyInfoAreFiltered ()
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
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (propertyDefinition1))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (propertyDefinition2))
          .Return (_fakeColumnDefinition1);
      MockSpecialColumns (1);
      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns().Count, Is.EqualTo (3));
          //instead of 4 (ID, ClassID and propertxdefinition)
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_AbstractClassWithoutDerivedClasses ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (AbstractClassWithoutDerivations), null);
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      var propertyDefinition = CreateAndAddPropertyDefinition (
          classDefinition, "DomainBase", typeof (AbstractClassWithoutDerivations).GetProperty ("DomainBase"));

      _columnDefinitionFactoryMock.Expect (mock => mock.CreateColumnDefinition (propertyDefinition)).Return
          (_fakeColumnDefinition1);
      MockSpecialColumns (1);
      _columnDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (NullEntityDefinition)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_AbstracrClassWithAbstractSubclassWithoutDerivedClasses ()
    {
      var baseClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (AbstractBaseClassWithHierarchy), null);
      var derivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (AbstractClassWithoutDerivations), baseClassDefinition);
      baseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { derivedClassDefinition }, true, true));
      derivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      var propertyDefinition1 = CreateAndAddPropertyDefinition (
          baseClassDefinition, "ClientFromAbstractBaseClass", typeof (AbstractBaseClassWithHierarchy).GetProperty ("ClientFromAbstractBaseClass"));
      var propertyDefinition2 = CreateAndAddPropertyDefinition (
          derivedClassDefinition, "DomainBase", typeof (AbstractClassWithoutDerivations).GetProperty ("DomainBase"));

      _columnDefinitionFactoryMock.Expect (mock => mock.CreateColumnDefinition (propertyDefinition1)).Return
          (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateColumnDefinition (propertyDefinition2)).Return
          (_fakeColumnDefinition2);
      MockSpecialColumns (2);

      _columnDefinitionFactoryMock.Replay ();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (baseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations ();
      Assert.That (baseClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (NullEntityDefinition)));
      Assert.That (derivedClassDefinition.StorageEntityDefinition, Is.TypeOf (typeof (NullEntityDefinition)));
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

      MockSpecialColumns (1);
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition1);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void StorageEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _derivedClassDefinition2.SetStorageEntity (invalidStorageEntityDefinition);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);
    }

    private void AssertTableDefinition (
        ClassDefinition classDefinition, string storageProviderID, string tableName, string viewName, IColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
    }

    private void AssertFilterViewDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition baseEntity,
        string[] classIDs,
        IColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (baseEntity));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ClassIDs, Is.EqualTo (classIDs));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
    }

    private void AssertUnionViewDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition[] storageEntityDefinitions,
        IColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).UnionedEntities, Is.EqualTo (storageEntityDefinitions));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
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

    private void MockSpecialColumns (int columnCount)
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateIDColumnDefinition())
          .Return (_fakeIDColumnDefinition)
          .Repeat.Times (columnCount);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateTimestampColumnDefinition())
          .Return (_fakeTimestampColumnDefinition)
          .Repeat.Times (columnCount);
    }
  }
}