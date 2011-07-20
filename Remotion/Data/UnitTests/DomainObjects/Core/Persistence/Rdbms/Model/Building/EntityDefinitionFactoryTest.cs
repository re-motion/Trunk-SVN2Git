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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class EntityDefinitionFactoryTest
  {
    private string _storageProviderID;
    private EntityDefinitionFactory _factory;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProviderMock;
    private IColumnDefinitionResolver _columnDefinitionResolverMock;
    private IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactoryMock;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private SimpleStoragePropertyDefinition _fakeStorageProperty1;
    private SimpleStoragePropertyDefinition _fakeTimestampStorageProperty;
    private SimpleStoragePropertyDefinition _fakeObjectIDStorageProperty;
    private ForeignKeyConstraintDefinition _fakeForeignKeyConstraint;
    private IStorageNameProvider _storageNameProviderMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _infrastructureStoragePropertyDefinitionProviderMock = MockRepository.GenerateStrictMock<IInfrastructureStoragePropertyDefinitionProvider>();
      _columnDefinitionResolverMock = MockRepository.GenerateStrictMock<IColumnDefinitionResolver>();
      _storageNameProviderMock = MockRepository.GenerateStrictMock<IStorageNameProvider>();
      _foreignKeyConstraintDefinitionFactoryMock = MockRepository.GenerateStrictMock<IForeignKeyConstraintDefinitionFactory>();
      _factory = new EntityDefinitionFactory (
          _infrastructureStoragePropertyDefinitionProviderMock,
          _foreignKeyConstraintDefinitionFactoryMock,
          _columnDefinitionResolverMock,
          _storageNameProviderMock,
          _storageProviderDefinition);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _fakeObjectIDStorageProperty = SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _fakeStorageProperty1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test1");
      _fakeTimestampStorageProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      _fakeForeignKeyConstraint = new ForeignKeyConstraintDefinition (
          "FakeForeignKeyConstraint",
          new EntityNameDefinition (null, "Test"),
          new[] { _fakeObjectIDStorageProperty.ColumnDefinition },
          new[] { _fakeStorageProperty1.ColumnDefinition });
    }

    [Test]
    public void CreateTableDefinition ()
    {
      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.TableClassDefinition1))
          .Return (new[] { _fakeStorageProperty1 });
      _columnDefinitionResolverMock.Replay();

      _foreignKeyConstraintDefinitionFactoryMock
          .Expect (mock => mock.CreateForeignKeyConstraints (_testModel.TableClassDefinition1))
          .Return (new[] { _fakeForeignKeyConstraint });
      _foreignKeyConstraintDefinitionFactoryMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (_testModel.TableClassDefinition1))
          .Return ("FakeTableName");
      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.TableClassDefinition1))
          .Return ("FakeViewName");
      _storageNameProviderMock
          .Expect (mock => mock.GetPrimaryKeyConstraintName (_testModel.TableClassDefinition1))
          .Return ("FakePrimaryKeyName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateTableDefinition (_testModel.TableClassDefinition1);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _foreignKeyConstraintDefinitionFactoryMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckTableDefinition (
          result,
          _storageProviderID,
          "FakeTableName",
          "FakeViewName",
          new[]
          {
              _fakeObjectIDStorageProperty.ColumnDefinition, _fakeStorageProperty1.ColumnDefinition, _fakeTimestampStorageProperty.ColumnDefinition
              ,
              _fakeStorageProperty1.ColumnDefinition
          },
          new ITableConstraintDefinition[]
          {
              new PrimaryKeyConstraintDefinition ("FakePrimaryKeyName", true, new[] { _fakeObjectIDStorageProperty.ColumnDefinition }),
              _fakeForeignKeyConstraint
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Table1Class' has no table name defined.")]
    public void CreateTableDefinition_ClassHasNoDBTableAttributeDefined ()
    {
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (_testModel.TableClassDefinition1))
          .Return (null);
      _storageNameProviderMock.Replay();

      _factory.CreateTableDefinition (_testModel.TableClassDefinition1);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithoutDerivations ()
    {
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.DerivedClassDefinition1))
          .Return (new[] { _fakeStorageProperty1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition1))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition1, fakeBaseEntityDefiniton);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived1Class" },
          new[]
          {
              _fakeObjectIDStorageProperty.ColumnDefinition, _fakeStorageProperty1.ColumnDefinition, _fakeTimestampStorageProperty.ColumnDefinition
              , _fakeStorageProperty1.ColumnDefinition
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithDerivations ()
    {
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.DerivedClassDefinition2))
          .Return (new[] { _fakeStorageProperty1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition2))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition2, fakeBaseEntityDefiniton);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _fakeObjectIDStorageProperty.ColumnDefinition, _fakeStorageProperty1.ColumnDefinition, _fakeTimestampStorageProperty.ColumnDefinition
              , _fakeStorageProperty1.ColumnDefinition
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateUnionViewDefinition ()
    {
      var fakeUnionEntity1 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test1"),
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);
      var fakeUnionEntity2 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test2"),
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.BaseBaseClassDefinition))
          .Return (new[] { _fakeStorageProperty1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.BaseBaseClassDefinition))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();

      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateUnionViewDefinition (
          _testModel.BaseBaseClassDefinition, new IEntityDefinition[] { fakeUnionEntity1, fakeUnionEntity2 });

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckUnionViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          new[] { fakeUnionEntity1, fakeUnionEntity2 },
          new[]
          {
              _fakeObjectIDStorageProperty.ColumnDefinition, _fakeStorageProperty1.ColumnDefinition, _fakeTimestampStorageProperty.ColumnDefinition
              , _fakeStorageProperty1.ColumnDefinition
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    private void CheckTableDefinition (
        IEntityDefinition expectedEntityDefinition,
        string expectedStorageProviderID,
        string expectedTableName,
        string expectedViewName,
        ColumnDefinition[] expectedColumnDefinitions,
        ITableConstraintDefinition[] expectedTableConstraintDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That (expectedEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (expectedEntityDefinition.StorageProviderID, Is.EqualTo (expectedStorageProviderID));
      Assert.That (((TableDefinition) expectedEntityDefinition).TableName.EntityName, Is.EqualTo (expectedTableName));
      Assert.That (((TableDefinition) expectedEntityDefinition).TableName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) expectedEntityDefinition).ViewName.EntityName, Is.EqualTo (expectedViewName));
      Assert.That (((TableDefinition) expectedEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) expectedEntityDefinition).GetAllColumns(), Is.EqualTo (expectedColumnDefinitions));
      Assert.That (((TableDefinition) expectedEntityDefinition).Indexes, Is.EqualTo (expectedIndexDefinitions));
      Assert.That (((TableDefinition) expectedEntityDefinition).Synonyms, Is.EqualTo (expectedSynonyms));

      var tableConstraints = ((TableDefinition) expectedEntityDefinition).Constraints;
      Assert.That (tableConstraints.Count, Is.EqualTo (expectedTableConstraintDefinitions.Length));

      for (var i = 0; i < expectedTableConstraintDefinitions.Length; i++)
      {
        Assert.That (expectedTableConstraintDefinitions[i].ConstraintName, Is.EqualTo (tableConstraints[i].ConstraintName));
        var tableConstraintDefinitioAsPrimaryKeyConstraint = expectedTableConstraintDefinitions[i] as PrimaryKeyConstraintDefinition;
        if (tableConstraintDefinitioAsPrimaryKeyConstraint != null)
        {
          Assert.That (tableConstraints[i], Is.TypeOf (typeof (PrimaryKeyConstraintDefinition)));
          Assert.That (
              ((PrimaryKeyConstraintDefinition) tableConstraints[i]).IsClustered,
              Is.EqualTo (tableConstraintDefinitioAsPrimaryKeyConstraint.IsClustered));
          Assert.That (
              ((PrimaryKeyConstraintDefinition) tableConstraints[i]).Columns, Is.EqualTo (tableConstraintDefinitioAsPrimaryKeyConstraint.Columns));
        }
        else
          Assert.That (tableConstraints[i], Is.EqualTo (expectedTableConstraintDefinitions[i]));
      }
    }

    private void CheckFilterViewDefinition (
        IEntityDefinition expectedEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IStorageEntityDefinition expectedBaseEntity,
        string[] expectedClassIDs,
        ColumnDefinition[] expectedColumnDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That (expectedEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (expectedEntityDefinition.StorageProviderID, Is.EqualTo (expectedStorageProviderID));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).ViewName.EntityName, Is.EqualTo (expectedViewName));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).BaseEntity, Is.SameAs (expectedBaseEntity));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).ClassIDs, Is.EqualTo (expectedClassIDs));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).GetAllColumns(), Is.EqualTo (expectedColumnDefinitions));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).Indexes, Is.EqualTo (expectedIndexDefinitions));
      Assert.That (((FilterViewDefinition) expectedEntityDefinition).Synonyms, Is.EqualTo (expectedSynonyms));
    }

    private void CheckUnionViewDefinition (
        IEntityDefinition expectedEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IStorageEntityDefinition[] expectedStorageEntityDefinitions,
        ColumnDefinition[] expectedColumnDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That (expectedEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (expectedEntityDefinition.StorageProviderID, Is.EqualTo (expectedStorageProviderID));
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).ViewName.EntityName, Is.EqualTo (expectedViewName));
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).UnionedEntities, Is.EqualTo (expectedStorageEntityDefinitions));
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).GetAllColumns(), Is.EqualTo (expectedColumnDefinitions));
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).Indexes, Is.EqualTo (expectedIndexDefinitions));
      Assert.That (((UnionViewDefinition) expectedEntityDefinition).Synonyms, Is.EqualTo (expectedSynonyms));
    }

    private void MockSpecialColumns ()
    {
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetObjectIDColumnDefinition())
          .Return (_fakeObjectIDStorageProperty.ColumnDefinition);
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetClassIDColumnDefinition())
          .Return (_fakeStorageProperty1.ColumnDefinition);
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetTimestampColumnDefinition())
          .Return (_fakeTimestampStorageProperty.ColumnDefinition);
    }
  }
}