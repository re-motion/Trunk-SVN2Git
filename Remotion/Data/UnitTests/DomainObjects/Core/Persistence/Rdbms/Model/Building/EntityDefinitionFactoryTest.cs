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
    private IColumnDefinitionFactory _columnDefinitionFactoryMock;
    private IColumnDefinitionResolver _columnDefinitionResolverMock;
    private IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactoryMock;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition1;
    private SimpleStoragePropertyDefinition _fakeTimestampColumnDefinition;
    private SimpleStoragePropertyDefinition _fakeObjectIDColumnDefinition;
    private ForeignKeyConstraintDefinition _fakeForeignKeyConstraint;
    private IStorageNameProvider _storageNameProviderMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _columnDefinitionFactoryMock = MockRepository.GenerateStrictMock<IColumnDefinitionFactory>();
      _columnDefinitionResolverMock = MockRepository.GenerateStrictMock<IColumnDefinitionResolver>();
      _storageNameProviderMock = MockRepository.GenerateStrictMock<IStorageNameProvider>();
      _foreignKeyConstraintDefinitionFactoryMock = MockRepository.GenerateStrictMock<IForeignKeyConstraintDefinitionFactory>();
      _factory = new EntityDefinitionFactory (
          _columnDefinitionFactoryMock,
          _foreignKeyConstraintDefinitionFactoryMock,
          _columnDefinitionResolverMock,
          _storageNameProviderMock,
          _storageProviderDefinition);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _fakeObjectIDColumnDefinition = ColumnDefinitionObjectMother.ObjectIDColumn;
      _fakeColumnDefinition1 = ColumnDefinitionObjectMother.CreateColumn ("Test1");
      _fakeTimestampColumnDefinition = ColumnDefinitionObjectMother.TimestampColumn;

      _fakeForeignKeyConstraint = new ForeignKeyConstraintDefinition (
          "FakeForeignKeyConstraint",
          new EntityNameDefinition (null, "Test"),
          new[] { _fakeObjectIDColumnDefinition.ColumnDefinition },
          new[] { _fakeColumnDefinition1.ColumnDefinition });
    }

    [Test]
    public void CreateTableDefinition ()
    {
      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.TableClassDefinition1))
          .Return (new[] { _fakeColumnDefinition1 });
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
      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateTableDefinition (_testModel.TableClassDefinition1);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _foreignKeyConstraintDefinitionFactoryMock.VerifyAllExpectations();
      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckTableDefinition (
          result,
          _storageProviderID,
          "FakeTableName",
          "FakeViewName",
          new[]
          {
              _fakeObjectIDColumnDefinition.ColumnDefinition, _fakeColumnDefinition1.ColumnDefinition, _fakeTimestampColumnDefinition.ColumnDefinition
              ,
              _fakeColumnDefinition1.ColumnDefinition
          },
          new ITableConstraintDefinition[]
          {
              new PrimaryKeyConstraintDefinition ("FakePrimaryKeyName", true, new[] { _fakeObjectIDColumnDefinition.ColumnDefinition }),
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
          ColumnDefinitionObjectMother.ObjectIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.ClassIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.TimestampColumn.ColumnDefinition);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.DerivedClassDefinition1))
          .Return (new[] { _fakeColumnDefinition1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition1))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();
      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition1, fakeBaseEntityDefiniton);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived1Class" },
          new[]
          {
              _fakeObjectIDColumnDefinition.ColumnDefinition, _fakeColumnDefinition1.ColumnDefinition, _fakeTimestampColumnDefinition.ColumnDefinition
              , _fakeColumnDefinition1.ColumnDefinition
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
          ColumnDefinitionObjectMother.ObjectIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.ClassIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.TimestampColumn.ColumnDefinition);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.DerivedClassDefinition2))
          .Return (new[] { _fakeColumnDefinition1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition2))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();
      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition2, fakeBaseEntityDefiniton);

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _fakeObjectIDColumnDefinition.ColumnDefinition, _fakeColumnDefinition1.ColumnDefinition, _fakeTimestampColumnDefinition.ColumnDefinition
              , _fakeColumnDefinition1.ColumnDefinition
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
          ColumnDefinitionObjectMother.ObjectIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.ClassIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.TimestampColumn.ColumnDefinition);
      var fakeUnionEntity2 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test2"),
          ColumnDefinitionObjectMother.ObjectIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.ClassIDColumn.ColumnDefinition,
          ColumnDefinitionObjectMother.TimestampColumn.ColumnDefinition);

      _columnDefinitionResolverMock
          .Expect (mock => mock.GetColumnDefinitionsForHierarchy (_testModel.BaseBaseClassDefinition))
          .Return (new[] { _fakeColumnDefinition1 });
      _columnDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.BaseBaseClassDefinition))
          .Return ("FakeViewName");
      _storageNameProviderMock.Replay();

      MockSpecialColumns();

      _columnDefinitionFactoryMock.Replay();

      var result = _factory.CreateUnionViewDefinition (
          _testModel.BaseBaseClassDefinition, new IEntityDefinition[] { fakeUnionEntity1, fakeUnionEntity2 });

      _columnDefinitionResolverMock.VerifyAllExpectations();
      _columnDefinitionFactoryMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckUnionViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          new[] { fakeUnionEntity1, fakeUnionEntity2 },
          new[]
          {
              _fakeObjectIDColumnDefinition.ColumnDefinition, _fakeColumnDefinition1.ColumnDefinition, _fakeTimestampColumnDefinition.ColumnDefinition
              , _fakeColumnDefinition1.ColumnDefinition
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
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateObjectIDColumnDefinition())
          .Return (_fakeObjectIDColumnDefinition.ColumnDefinition);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateClassIDColumnDefinition())
          .Return (_fakeColumnDefinition1.ColumnDefinition);
      _columnDefinitionFactoryMock
          .Expect (mock => mock.CreateTimestampColumnDefinition())
          .Return (_fakeTimestampColumnDefinition.ColumnDefinition);
    }
  }
}