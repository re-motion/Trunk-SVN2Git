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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderIntegrationTest
  {
    private string _storageProviderID;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    private ReflectionBasedStorageNameProvider _storageNameProvider;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;
    private ColumnDefinitionResolver _columnDefinitionResolver;
    private ForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private EntityDefinitionFactory _entityDefinitionFactory;
    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;

    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private SimpleStoragePropertyDefinition _fakeBaseBaseColumnDefinition;
    private SimpleStoragePropertyDefinition _fakeBaseColumnDefinition;
    private SimpleStoragePropertyDefinition _fakeTableColumnDefinition1;
    private SimpleStoragePropertyDefinition _fakeTableColumnDefinition2;
    private SimpleStoragePropertyDefinition _fakeDerivedColumnDefinition1;
    private SimpleStoragePropertyDefinition _fakeDerivedColumnDefinition2;
    private SimpleStoragePropertyDefinition _fakeDerivedDerivedColumnDefinition;
    private ObjectIDStoragePropertyDefinition _fakeIDColumnDefinition;
    private SimpleStoragePropertyDefinition _fakeObjectIDColumn;
    private SimpleStoragePropertyDefinition _fakeClassIDColumn;
    private ColumnDefinition _fakeTimestampColumnDefinition;


    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);

      _storageNameProvider = new ReflectionBasedStorageNameProvider();
      _infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider (
          new SqlStorageTypeCalculator (), _storageNameProvider);
      _dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          new SqlStorageTypeCalculator (), _storageNameProvider, _storageProviderDefinitionFinder);
      _columnDefinitionResolver = new ColumnDefinitionResolver();
      _foreignKeyConstraintDefinitionFactory = new ForeignKeyConstraintDefinitionFactory (
          _storageNameProvider, _columnDefinitionResolver, _infrastructureStoragePropertyDefinitionProvider, _storageProviderDefinitionFinder);
      _entityDefinitionFactory = new EntityDefinitionFactory (
          _infrastructureStoragePropertyDefinitionProvider,
          _foreignKeyConstraintDefinitionFactory,
          _columnDefinitionResolver,
          _storageNameProvider,
          _storageProviderDefinition);
      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _storageProviderDefinition,
          _entityDefinitionFactory,
          _dataStoragePropertyDefinitionFactory,
          _storageNameProvider,
          new RdbmsPersistenceModelProvider ());

      _testModel = new RdbmsPersistenceModelLoaderTestHelper();
      _fakeBaseBaseColumnDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("BaseBaseProperty");
      _fakeBaseColumnDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("BaseProperty");
      _fakeTableColumnDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("TableProperty1");
      _fakeTableColumnDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("TableProperty2");
      _fakeDerivedColumnDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("DerivedProperty1");
      _fakeDerivedColumnDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("DerivedProperty2");
      _fakeDerivedDerivedColumnDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("DerivedDerivedProperty");
      _fakeObjectIDColumn = new SimpleStoragePropertyDefinition (_infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition ());
      _fakeClassIDColumn = new SimpleStoragePropertyDefinition (_infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ());
      _fakeIDColumnDefinition = new ObjectIDStoragePropertyDefinition (_fakeObjectIDColumn, _fakeClassIDColumn);
      _fakeTimestampColumnDefinition = _infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition ();

      _testModel.BaseBasePropertyDefinition.SetStorageProperty (_fakeBaseBaseColumnDefinition);
      _testModel.BasePropertyDefinition.SetStorageProperty (_fakeBaseColumnDefinition);
      _testModel.TablePropertyDefinition1.SetStorageProperty (_fakeTableColumnDefinition1);
      _testModel.TablePropertyDefinition2.SetStorageProperty (_fakeTableColumnDefinition2);
      _testModel.DerivedPropertyDefinition1.SetStorageProperty (_fakeDerivedColumnDefinition1);
      _testModel.DerivedPropertyDefinition2.SetStorageProperty (_fakeDerivedColumnDefinition2);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (_fakeDerivedDerivedColumnDefinition);
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseBaseClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.BaseBaseClassDefinition);

      AssertUnionViewDefinition (
          _testModel.BaseBaseClassDefinition,
          _storageProviderID,
          "BaseBaseClassView",
          new[] { _testModel.BaseClassDefinition.StorageEntityDefinition },
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition1.ColumnDefinition, _fakeTableColumnDefinition2.ColumnDefinition,
              _fakeDerivedColumnDefinition1.ColumnDefinition, _fakeDerivedColumnDefinition2.ColumnDefinition,
              _fakeDerivedDerivedColumnDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.BaseClassDefinition);

      AssertUnionViewDefinition (
          _testModel.BaseClassDefinition,
          _storageProviderID,
          "BaseClassView",
          new[] { _testModel.TableClassDefinition1.StorageEntityDefinition, _testModel.TableClassDefinition2.StorageEntityDefinition },
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition1.ColumnDefinition,
              _fakeTableColumnDefinition2.ColumnDefinition, _fakeDerivedColumnDefinition1.ColumnDefinition,
              _fakeDerivedColumnDefinition2.ColumnDefinition, _fakeDerivedDerivedColumnDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition1 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.TableClassDefinition1);

      AssertTableDefinition (
          _testModel.TableClassDefinition1,
          _storageProviderID,
          "Table1Class",
          "Table1ClassView",
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition1.ColumnDefinition
          },
          new PrimaryKeyConstraintDefinition ("PK_Table1Class", true, new[] { _fakeIDColumnDefinition.GetColumnForForeignKey() })
          );
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition2 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.TableClassDefinition2);

      AssertTableDefinition (
          _testModel.TableClassDefinition2,
          _storageProviderID,
          "Table2Class",
          "Table2ClassView",
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition2.ColumnDefinition,
              _fakeDerivedColumnDefinition1.ColumnDefinition, _fakeDerivedColumnDefinition2.ColumnDefinition,
              _fakeDerivedDerivedColumnDefinition.ColumnDefinition
          },
          new PrimaryKeyConstraintDefinition ("PK_Table2Class", true, new[] { _fakeIDColumnDefinition.GetColumnForForeignKey () })
          );
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition1 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedClassDefinition1);

      AssertFilterViewDefinition (
          _testModel.DerivedClassDefinition1,
          _storageProviderID,
          "Derived1ClassView",
          _testModel.TableClassDefinition2.StorageEntityDefinition,
          new[] { "Derived1Class" },
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition2.ColumnDefinition, _fakeDerivedColumnDefinition1.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition2 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedClassDefinition2);

      AssertFilterViewDefinition (
          _testModel.DerivedClassDefinition2,
          _storageProviderID,
          "Derived2ClassView",
          _testModel.TableClassDefinition2.StorageEntityDefinition,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition2.ColumnDefinition, _fakeDerivedColumnDefinition2.ColumnDefinition,
              _fakeDerivedDerivedColumnDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedDerivedClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedDerivedClassDefinition);

      AssertFilterViewDefinition (
          _testModel.DerivedDerivedClassDefinition,
          _storageProviderID,
          "DerivedDerivedClassView",
          _testModel.DerivedClassDefinition2.StorageEntityDefinition,
          new[] { "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _fakeObjectIDColumn.ColumnDefinition, _fakeClassIDColumn.ColumnDefinition, _fakeTimestampColumnDefinition,
              _fakeBaseBaseColumnDefinition.ColumnDefinition,
              _fakeBaseColumnDefinition.ColumnDefinition,
              _fakeTableColumnDefinition2.ColumnDefinition, _fakeDerivedColumnDefinition2.ColumnDefinition,
              _fakeDerivedDerivedColumnDefinition.ColumnDefinition
          });
    }

    private void AssertTableDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string tableName,
        string viewName,
        ColumnDefinition[] columnDefinitions,
        PrimaryKeyConstraintDefinition primaryKeyConstraintDefinition)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName.EntityName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo (columnDefinitions));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).Constraints[0], Is.TypeOf (typeof (PrimaryKeyConstraintDefinition)));
      var primaryKey = (PrimaryKeyConstraintDefinition) ((TableDefinition) classDefinition.StorageEntityDefinition).Constraints[0];
      Assert.That (primaryKey.ConstraintName, Is.EqualTo (primaryKeyConstraintDefinition.ConstraintName));
      Assert.That (primaryKey.Columns, Is.EqualTo (primaryKeyConstraintDefinition.Columns));
    }

    private void AssertFilterViewDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string viewName,
        IStorageEntityDefinition baseEntity,
        string[] classIDs,
        ColumnDefinition[] columnDefinitions)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs (baseEntity));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).ClassIDs, Is.EqualTo (classIDs));
      Assert.That (((FilterViewDefinition) classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo (columnDefinitions));
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
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo (viewName));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).UnionedEntities, Is.EqualTo (storageEntityDefinitions));
      Assert.That (((UnionViewDefinition) classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo (columnDefinitions));
    }
  }
}