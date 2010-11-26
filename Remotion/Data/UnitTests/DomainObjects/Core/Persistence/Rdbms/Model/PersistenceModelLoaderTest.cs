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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class PersistenceModelLoaderTest
  {
    private string _storageProviderID;

    private ReflectionBasedClassDefinition _baseBaseClassDefinition;
    private ReflectionBasedClassDefinition _baseClassDefinition;
    private ReflectionBasedClassDefinition _tableClassDefinition1;
    private ReflectionBasedClassDefinition _tableClassDefinition2;
    private ReflectionBasedClassDefinition _derivedClassDefinition1;
    private ReflectionBasedClassDefinition _derivedClassDefinition2;
    private ReflectionBasedClassDefinition _derivedDerivedClassDefinition;

    private ReflectionBasedPropertyDefinition _baseBasePropertyDefinition;
    private ReflectionBasedPropertyDefinition _basePropertyDefinition;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition1;
    private ReflectionBasedPropertyDefinition _tablePropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition1;
    private ReflectionBasedPropertyDefinition _derivedPropertyDefinition2;
    private ReflectionBasedPropertyDefinition _derivedDerivedPropertyDefinition;

    private IStoragePropertyDefinitionFactory _columnDefinitionFactoryMock;
    private PersistenceModelLoader _persistenceModelLoader;

    private ColumnDefinition _fakeColumnDefinition1;
    private ColumnDefinition _fakeColumnDefinition2;
    private ColumnDefinition _fakeColumnDefinition3;
    private ColumnDefinition _fakeColumnDefinition4;
    private ColumnDefinition _fakeColumnDefinition5;
    private ColumnDefinition _fakeColumnDefinition6;
    private ColumnDefinition _fakeColumnDefinition7;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID, typeof (UnitTestStorageObjectFactoryStub));
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);

      var typeWithDBTableAttribute1 = typeof (Order);
      var typeWithDBTableAttribute2 = typeof (Company);
      var typeWithoutDBTableAttribute1 = typeof (Distributor);
      var typeWithoutDBTableAttribute2 = typeof (Partner);
      var typeWithoutDBTableAttribute3 = typeof (Supplier);

      _baseBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithoutDBTableAttribute1, null);
      _baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithoutDBTableAttribute1, _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithDBTableAttribute1, _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithDBTableAttribute2, _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithoutDBTableAttribute1, _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeWithoutDBTableAttribute2, _tableClassDefinition2);
      _derivedDerivedClassDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeWithoutDBTableAttribute3, _derivedClassDefinition2);

      _baseBasePropertyDefinition = CreateAndAddPropertyDefinition (_baseBaseClassDefinition, "BaseBaseProperty");
      _basePropertyDefinition = CreateAndAddPropertyDefinition (_baseClassDefinition, "BaseProperty");
      _tablePropertyDefinition1 = CreateAndAddPropertyDefinition (_tableClassDefinition1, "TableProperty1");
      _tablePropertyDefinition2 = CreateAndAddPropertyDefinition (_tableClassDefinition2, "TableProperty2");
      _derivedPropertyDefinition1 = CreateAndAddPropertyDefinition (_derivedClassDefinition1, "DerivedProperty1");
      _derivedPropertyDefinition2 = CreateAndAddPropertyDefinition (_derivedClassDefinition2, "DerivedProperty2");
      _derivedDerivedPropertyDefinition = CreateAndAddPropertyDefinition (_derivedDerivedClassDefinition, "DerivedDerivedProperty");

      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IStoragePropertyDefinitionFactory>();
      _persistenceModelLoader = new PersistenceModelLoader (
          _columnDefinitionFactoryMock, _storageProviderDefinition, _storageProviderDefinitionFinder);

      _fakeColumnDefinition1 = new ColumnDefinition ("Test1", typeof (string), "varchar", true);
      _fakeColumnDefinition2 = new ColumnDefinition ("Test2", typeof (int), "int", false);
      _fakeColumnDefinition3 = new ColumnDefinition ("Test3", typeof (string), "varchar", true);
      _fakeColumnDefinition4 = new ColumnDefinition ("Test4", typeof (int), "int", false);
      _fakeColumnDefinition5 = new ColumnDefinition ("Test5", typeof (string), "varchar", true);
      _fakeColumnDefinition6 = new ColumnDefinition ("Test6", typeof (int), "int", false);
      _fakeColumnDefinition7 = new ColumnDefinition ("Test7", typeof (string), "varchar", true);
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_AlreadyHasPersistenceModelApplied_AndNoDerivedClassDefinitions ()
    {
      var storageEntityDefinition = new TableDefinition (_storageProviderDefinition, "Test", "TestView", new ColumnDefinition[] { });
      _derivedDerivedClassDefinition.SetStorageEntity (storageEntityDefinition);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (_derivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (storageEntityDefinition));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseBaseClassDefinition ()
    {
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).
          Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition1, _storageProviderDefinitionFinder))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition6);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition7);
      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_baseBaseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertUnionViewDefinition (
          _baseBaseClassDefinition,
          _storageProviderID,
          "DistributorView",
          new[] { _baseClassDefinition.StorageEntityDefinition },
          new[]
          {
              _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition5,
              _fakeColumnDefinition6, _fakeColumnDefinition7
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinition ()
    {
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).
          Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition1, _storageProviderDefinitionFinder))
          .Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder))
          .Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition6);
      _columnDefinitionFactoryMock.Expect (
          mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
              _fakeColumnDefinition7);
      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_baseClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertUnionViewDefinition (
          _baseClassDefinition,
          _storageProviderID,
          "DistributorView",
          new[] { _tableClassDefinition1.StorageEntityDefinition, _tableClassDefinition2.StorageEntityDefinition },
          new[]
          {
              _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition5,
              _fakeColumnDefinition6, _fakeColumnDefinition7
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition1 ()
    {
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition1, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition3);

      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          _tableClassDefinition1,
          _storageProviderID,
          "Order",
          "OrderView",
          new[] { _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3 });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition2 ()
    {
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
          _fakeColumnDefinition6);
      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition2);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          _tableClassDefinition2,
          _storageProviderID,
          "Company",
          "CompanyView",
          new[]
          {
              _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition5,
              _fakeColumnDefinition6
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition1 ()
    {
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
          _fakeColumnDefinition6);

      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedClassDefinition1);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedClassDefinition1,
          _storageProviderID,
          "DistributorView",
          _tableClassDefinition2.StorageEntityDefinition,
          "Distributor",
          new[] { _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4 });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition2 ()
    {
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
          _fakeColumnDefinition6);
      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedClassDefinition2);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedClassDefinition2,
          _storageProviderID,
          "PartnerView",
          _tableClassDefinition2.StorageEntityDefinition,
          "Partner",
          new[] { _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition6 });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedDerivedClassDefinition ()
    {
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_baseBasePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition1);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_basePropertyDefinition, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition2);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition3);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition2, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition4);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedDerivedPropertyDefinition, _storageProviderDefinitionFinder)).Return (
          _fakeColumnDefinition5);
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_derivedPropertyDefinition1, _storageProviderDefinitionFinder)).Return (_fakeColumnDefinition6);
      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertFilterViewDefinition (
          _derivedDerivedClassDefinition,
          _storageProviderID,
          "SupplierView",
          _derivedClassDefinition2.StorageEntityDefinition,
          "Supplier",
          new[] { _fakeColumnDefinition1, _fakeColumnDefinition2, _fakeColumnDefinition3, _fakeColumnDefinition4, _fakeColumnDefinition5 });
    }

    [Test]
    public void ApplyPersistentModelToHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var nonPersistentProperty = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition, "NonPersistentProperty", "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));

      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (classDefinition, _storageProviderID, "Order", "OrderView", new ColumnDefinition[0]);
    }

    [Test]
    public void ApplyPersistentModelToHierarchy_TableNameOfTheDBTableAttributeIsUsed ()
    {
      var classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
              typeof (ClassHavingStorageSpecificIdentifierAttribute), null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));

      _columnDefinitionFactoryMock.Replay();

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _columnDefinitionFactoryMock.VerifyAllExpectations();
      AssertTableDefinition (
          classDefinition,
          _storageProviderID,
          "ClassHavingStorageSpecificIdentifierAttributeTable",
          "ClassHavingStorageSpecificIdentifierAttributeView",
          new ColumnDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\n"
        + "Declaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.ReflectionBasedPropertyDefinitionFactory'\r\n"
        + "Property: 'FakeProperty'")]
    public void StoragePropertyDefinitionIsNoColumnDefinition ()
    {
      var fakeResult = new FakeColumnDefinition ("Invalid");
      _columnDefinitionFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (_tablePropertyDefinition1, _storageProviderDefinitionFinder)).Return (fakeResult);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_tableClassDefinition1);
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void StorageEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _derivedClassDefinition2.SetStorageEntity (invalidStorageEntityDefinition);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy (_derivedDerivedClassDefinition);
    }

    private void AssertTableDefinition (
        ClassDefinition classDefinition, string storageProviderID, string tableName, string viewName, ColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (
          ((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns().OrderBy (c => c.Name).ToArray(), Is.EqualTo (columnDefinitions));
    }

    private void AssertFilterViewDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition baseEntity,
        string classID,
        ColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (baseEntity));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ClassID, Is.EqualTo (classID));
      Assert.That (
          ((FilterViewDefinition) classDefinition.StorageEntityDefinition).GetColumns().OrderBy (c => c.Name).ToArray(),
          Is.EqualTo (columnDefinitions));
    }

    private void AssertUnionViewDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition[] storageEntityDefinitions,
        ColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).UnionedEntities, Is.EqualTo (storageEntityDefinitions));
      Assert.That (
          ((UnionViewDefinition) classDefinition.StorageEntityDefinition).GetColumns().OrderBy (c => c.Name).ToArray(), Is.EqualTo (columnDefinitions));
    }

    private ReflectionBasedPropertyDefinition CreateAndAddPropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName)
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition,
          propertyName,
          null);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }
  }
}