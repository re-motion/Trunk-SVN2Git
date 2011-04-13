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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
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
    private SimpleColumnDefinition _fakeColumnDefinition1;
    private IDColumnDefinition _fakeObjectIDColumnDefinition;
    private SimpleColumnDefinition _fakeTimestampColumnDefinition;
    private SimpleColumnDefinition _fakeIDColumnDefinition;
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

      _fakeIDColumnDefinition = new SimpleColumnDefinition ("ID", typeof (ObjectID), "uniqueidentifier", false, true);
      _fakeColumnDefinition1 = new SimpleColumnDefinition ("Test1", typeof (string), "varchar", true, false);
      _fakeObjectIDColumnDefinition = new IDColumnDefinition (_fakeIDColumnDefinition, _fakeColumnDefinition1);
      _fakeTimestampColumnDefinition = new SimpleColumnDefinition ("Timestamp", typeof (object), "rowversion", false, false);

      _fakeForeignKeyConstraint = new ForeignKeyConstraintDefinition (
          "FakeForeignKeyConstraint", new EntityNameDefinition(null, "Test"), new[] { _fakeIDColumnDefinition }, new[] { _fakeColumnDefinition1 });
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
      AssertTableDefinition (
          result,
          _storageProviderID,
          "FakeTableName",
          "FakeViewName",
          new IColumnDefinition[] { _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1 },
          new ITableConstraintDefinition[]
          { new PrimaryKeyConstraintDefinition ("FakePrimaryKeyName", true, new[] { _fakeIDColumnDefinition }), _fakeForeignKeyConstraint },
          new IIndexDefinition[0]);
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
      var fakeBaseEntityDefiniton = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new EntityNameDefinition (null, "TestView"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0]);

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
      AssertFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived1Class" },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1
          },
          new IIndexDefinition[0]);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithDerivations ()
    {
      var fakeBaseEntityDefiniton = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new EntityNameDefinition (null, "TestView"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0]);

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
      AssertFilterViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1
          },
          new IIndexDefinition[0]);
    }

    [Test]
    public void CreateUnionViewDefinition ()
    {
      var fakeUnionEntity1 = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test1"),
          new EntityNameDefinition (null, "TestView1"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0]);
      var fakeUnionEntity2 = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test2"),
          new EntityNameDefinition (null, "TestView2"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0]);

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
      AssertUnionViewDefinition (
          result,
          _storageProviderID,
          "FakeViewName",
          new[] { fakeUnionEntity1, fakeUnionEntity2 },
          new IColumnDefinition[]
          {
              _fakeObjectIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeColumnDefinition1
          },
          new IIndexDefinition[0]);
    }

    private void AssertTableDefinition (
        IEntityDefinition entityDefinition,
        string storageProviderID,
        string tableName,
        string viewName,
        IColumnDefinition[] columnDefinitions,
        ITableConstraintDefinition[] tableConstraintDefinitions,
        IIndexDefinition[] indexDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) entityDefinition).TableName.EntityName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) entityDefinition).TableName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) entityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((TableDefinition) entityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
      Assert.That (((TableDefinition) entityDefinition).Indexes, Is.EqualTo (indexDefinitions));

      var tableConstraints = ((TableDefinition) entityDefinition).Constraints;
      Assert.That (tableConstraints.Count, Is.EqualTo (tableConstraintDefinitions.Length));

      for (var i = 0; i < tableConstraintDefinitions.Length; i++)
      {
        Assert.That (tableConstraintDefinitions[i].ConstraintName, Is.EqualTo (tableConstraints[i].ConstraintName));
        var tableConstraintDefinitioAsPrimaryKeyConstraint = tableConstraintDefinitions[i] as PrimaryKeyConstraintDefinition;
        if (tableConstraintDefinitioAsPrimaryKeyConstraint != null)
        {
          Assert.That (tableConstraints[i], Is.TypeOf (typeof (PrimaryKeyConstraintDefinition)));
          Assert.That (
              ((PrimaryKeyConstraintDefinition) tableConstraints[i]).IsClustered,
              Is.EqualTo (tableConstraintDefinitioAsPrimaryKeyConstraint.IsClustered));
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
        IColumnDefinition[] columnDefinitions,
        IIndexDefinition[] indexDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((FilterViewDefinition) entityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((FilterViewDefinition) entityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((FilterViewDefinition) entityDefinition).BaseEntity, Is.SameAs (baseEntity));
      Assert.That (((FilterViewDefinition) entityDefinition).ClassIDs, Is.EqualTo (classIDs));
      Assert.That (((FilterViewDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
      Assert.That (((FilterViewDefinition) entityDefinition).Indexes, Is.EqualTo (indexDefinitions));
    }

    private void AssertUnionViewDefinition (
        IEntityDefinition entityDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition[] storageEntityDefinitions,
        IColumnDefinition[] columnDefinitions,
        IIndexDefinition[] indexDefinitions)
    {
      Assert.That (entityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (entityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((UnionViewDefinition) entityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((UnionViewDefinition) entityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((UnionViewDefinition) entityDefinition).UnionedEntities, Is.EqualTo (storageEntityDefinitions));
      Assert.That (((UnionViewDefinition) entityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
      Assert.That (((UnionViewDefinition) entityDefinition).Indexes, Is.EqualTo (indexDefinitions));
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