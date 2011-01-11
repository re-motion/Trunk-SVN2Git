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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class EntityDefinitionFactoryTest
  {
    private EntityDefinitionFactory _factory;
    private IColumnDefinitionFactory _columnDefinitionFactoryMock;
    private string _storageProviderID;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
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
    private SimpleColumnDefinition _fakeColumnDefinition1;
    private SimpleColumnDefinition _fakeColumnDefinition2;
    private SimpleColumnDefinition _fakeColumnDefinition3;
    private SimpleColumnDefinition _fakeColumnDefinition4;
    private SimpleColumnDefinition _fakeColumnDefinition5;
    private SimpleColumnDefinition _fakeColumnDefinition6;
    private SimpleColumnDefinition _fakeColumnDefinition7;
    private IDColumnDefinition _fakeObjectIDColumnDefinition;
    private SimpleColumnDefinition _fakeTimestampColumnDefinition;
    private SimpleColumnDefinition _fakeIDColumnDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID, typeof (UnitTestStorageObjectFactoryStub));
      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IColumnDefinitionFactory>();
      _factory = new EntityDefinitionFactory (_columnDefinitionFactoryMock, _storageProviderDefinition);

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

      _fakeIDColumnDefinition = new SimpleColumnDefinition ("ID", typeof (ObjectID), "uniqueidentifier", false, true);
      _fakeColumnDefinition1 = new SimpleColumnDefinition ("Test1", typeof (string), "varchar", true, false);
      _fakeColumnDefinition2 = new SimpleColumnDefinition ("Test2", typeof (int), "int", false, false);
      _fakeColumnDefinition3 = new SimpleColumnDefinition ("Test3", typeof (string), "varchar", true, false);
      _fakeColumnDefinition4 = new SimpleColumnDefinition ("Test4", typeof (int), "int", false, false);
      _fakeColumnDefinition5 = new SimpleColumnDefinition ("Test5", typeof (string), "varchar", true, false);
      _fakeColumnDefinition6 = new SimpleColumnDefinition ("Test6", typeof (int), "int", false, false);
      _fakeColumnDefinition7 = new SimpleColumnDefinition ("Test7", typeof (string), "varchar", true, false);
      _fakeObjectIDColumnDefinition = new IDColumnDefinition (_fakeIDColumnDefinition, _fakeColumnDefinition2);
      _fakeTimestampColumnDefinition = new SimpleColumnDefinition ("Timestamp", typeof (object), "rowversion", false, false);
    }

    [Test]
    public void CreateTableDefinition ()
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
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateTableDefinition (_tableClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          result,
          _storageProviderID,
          "Order",
          "OrderView",
          new IColumnDefinition[]
          { _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3 },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition ("PK_Order", true, new[] { _fakeIDColumnDefinition }) });
    }

    [Test]
    public void CreateTableDefinition_TableNameOfTheDBTableAttributeIsUsed ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      MockSpecialColumns();
      _columnDefinitionFactoryMock.Replay();

      var result = (TableDefinition) _factory.CreateTableDefinition (classDefinition);

      Assert.That (result.TableName, Is.EqualTo ("ClassHavingStorageSpecificIdentifierAttributeTable"));

      _columnDefinitionFactoryMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateTableDefinition_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateTableDefinition (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          result,
          _storageProviderID,
          "Order",
          "OrderView",
          new IColumnDefinition[] { _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition ("PK_Order", true, new[] { _fakeIDColumnDefinition }) });
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
      classDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (propertyDefinition1))
          .Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (propertyDefinition2))
          .Return (_fakeColumnDefinition1);
      MockSpecialColumns();
      _columnDefinitionFactoryMock.Replay();

      var result = (TableDefinition) _factory.CreateTableDefinition (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (result.GetColumns().Count, Is.EqualTo (3)); //instead of 4 (ID, ClassID and propertxdefinition)
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Folder' has no 'DBTableAttribute' defined.")]
    public void CreateTableDefinition_ClassHasNoDBTableAttribute_ThrowsException ()
    {
      _factory.CreateTableDefinition (_baseClassDefinition);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithoutDerivations ()
    {
      var fakeBaseEntityDefiniton = new TableDefinition (
          _storageProviderDefinition, "Test", "TestView", new IColumnDefinition[0], new ITableConstraintDefinition[0]);

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
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_derivedClassDefinition1, fakeBaseEntityDefiniton);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          result,
          _storageProviderID,
          "DistributorView",
          fakeBaseEntityDefiniton,
          new[] { "Distributor" },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2,
              _fakeColumnDefinition3, _fakeColumnDefinition4
          });
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithDerivations ()
    {
      var fakeBaseEntityDefiniton = new TableDefinition (
          _storageProviderDefinition, "Test", "TestView", new IColumnDefinition[0], new ITableConstraintDefinition[0]);

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
          .Expect (mock => mock.CreateColumnDefinition (_derivedPropertyDefinition2))
          .Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateColumnDefinition (_derivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition6);
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_derivedClassDefinition2, fakeBaseEntityDefiniton);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          result,
          _storageProviderID,
          "PartnerView",
          fakeBaseEntityDefiniton,
          new[] { "Partner", "Supplier", "File" },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2,
              _fakeColumnDefinition3, _fakeColumnDefinition5, _fakeColumnDefinition6
          });
    }

    [Test]
    public void CreateUnionViewDefinition ()
    {
      var fakeUnionEntity1 = new TableDefinition (
          _storageProviderDefinition, "Test1", "TestView1", new IColumnDefinition[0], new ITableConstraintDefinition[0]);
      var fakeUnionEntity2 = new TableDefinition (
          _storageProviderDefinition, "Test2", "TestView2", new IColumnDefinition[0], new ITableConstraintDefinition[0]);

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
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateUnionViewDefinition (_baseBaseClassDefinition, new IEntityDefinition[] { fakeUnionEntity1, fakeUnionEntity2 });

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertUnionViewDefinition (
          result,
          _storageProviderID,
          "CustomerView",
          new[] { fakeUnionEntity1, fakeUnionEntity2 },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3,
              _fakeColumnDefinition4, _fakeColumnDefinition5, _fakeColumnDefinition6, _fakeColumnDefinition7
          });
    }

    [Test]
    public void CreateUnionViewDefinition_NoUnionedEntities ()
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
      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateUnionViewDefinition (_baseBaseClassDefinition, new IEntityDefinition[0]);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (NullEntityDefinition)));
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

      MockSpecialColumns();

      _factory.CreateTableDefinition (_tableClassDefinition1);
    }

    private void AssertTableDefinition (
        IEntityDefinition entityDefinition,
        string storageProviderID,
        string tableName,
        string viewName,
        IColumnDefinition[] columnDefinitions,
        ITableConstraintDefinition[] tableConstraintDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) entityDefinition).TableName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) entityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((TableDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));

      var tableConstraints = ((TableDefinition) entityDefinition).Constraints;
      Assert.That (tableConstraints.Count, Is.EqualTo (tableConstraintDefinitions.Length));
      
      for (var i = 0; i<tableConstraintDefinitions.Length; i++)
      {
        Assert.That (tableConstraintDefinitions[i].ConstraintName, Is.EqualTo (tableConstraints[i].ConstraintName));
        var tableConstraintDefinitioAsPrimaryKeyConstraint = tableConstraintDefinitions[i] as PrimaryKeyConstraintDefinition;
        if (tableConstraintDefinitioAsPrimaryKeyConstraint!=null)
        {
          Assert.That (tableConstraints[i], Is.TypeOf (typeof (PrimaryKeyConstraintDefinition)));
          Assert.That (
              ((PrimaryKeyConstraintDefinition) tableConstraints[i]).IsClustered, Is.EqualTo (tableConstraintDefinitioAsPrimaryKeyConstraint.IsClustered));
          Assert.That (
              ((PrimaryKeyConstraintDefinition) tableConstraints[i]).Columns, Is.EqualTo (tableConstraintDefinitioAsPrimaryKeyConstraint.Columns));
        }

        //TODO: Check for ForeignKey Constraints
        
        //TODO: Throw NotSupportedException on unsupported constraints
      }
    }

    private void AssertFilterViewDefinition (
        IEntityDefinition entityDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition baseEntity,
        string[] classIDs,
        IColumnDefinition[] columnDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((FilterViewDefinition) entityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((FilterViewDefinition) entityDefinition).BaseEntity, Is.SameAs (baseEntity));
      Assert.That (((FilterViewDefinition) entityDefinition).ClassIDs, Is.EqualTo (classIDs));
      Assert.That (((FilterViewDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
    }

    private void AssertUnionViewDefinition (
        IEntityDefinition entityDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition[] storageEntityDefinitions,
        IColumnDefinition[] columnDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((UnionViewDefinition) entityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((UnionViewDefinition) entityDefinition).UnionedEntities, Is.EqualTo (storageEntityDefinitions));
      Assert.That (((UnionViewDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
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

    private void MockSpecialColumns ()
    {
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateIDColumnDefinition())
          .Return (_fakeObjectIDColumnDefinition);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateTimestampColumnDefinition())
          .Return (_fakeTimestampColumnDefinition);
    }
  }
}