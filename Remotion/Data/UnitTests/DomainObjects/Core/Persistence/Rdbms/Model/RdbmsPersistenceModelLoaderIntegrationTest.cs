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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderIntegrationTest
  {
    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private string _storageProviderID;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private SimpleColumnDefinition _fakeBaseBaseColumnDefinition;
    private SimpleColumnDefinition _fakeBaseColumnDefinition;
    private SimpleColumnDefinition _fakeTableColumnDefinition1;
    private SimpleColumnDefinition _fakeTableColumnDefinition2;
    private SimpleColumnDefinition _fakeDerivedColumnDefinition1;
    private SimpleColumnDefinition _fakeDerivedColumnDefinition2;
    private SimpleColumnDefinition _fakeDerivedDerivedColumnDefinition;
    private IDColumnDefinition _fakeIDColumnDefinition;
    private SimpleColumnDefinition _fakeTimestampColumnDefinition;
    private IColumnDefinitionFactory _columnDefinitionFactory;
    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;
    private ReflectionBasedStorageNameProvider _storageNameProvider;
    private ForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private ColumnDefinitionResolver _columnDefinitionResolver;
    private EntityDefinitionFactory _entityDefinitionFactory;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _storageNameProvider = new ReflectionBasedStorageNameProvider();
      _columnDefinitionFactory = new ColumnDefinitionFactory (
          new SqlStorageTypeCalculator (_storageProviderDefinitionFinder), _storageNameProvider, _storageProviderDefinitionFinder);
      _columnDefinitionResolver = new ColumnDefinitionResolver();
      _foreignKeyConstraintDefinitionFactory = new ForeignKeyConstraintDefinitionFactory (
          _storageNameProvider, _columnDefinitionResolver, _columnDefinitionFactory, _storageProviderDefinitionFinder);
      _entityDefinitionFactory = new EntityDefinitionFactory (
          _columnDefinitionFactory,
          _foreignKeyConstraintDefinitionFactory,
          _columnDefinitionResolver,
          _storageNameProvider,
          _storageProviderDefinition);
      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _entityDefinitionFactory, _columnDefinitionFactory, _storageProviderDefinition, _storageNameProvider);

      _fakeBaseBaseColumnDefinition = new SimpleColumnDefinition ("BaseBaseProperty", typeof (string), "nvarchar (max)", true, false);
      _fakeBaseColumnDefinition = new SimpleColumnDefinition ("BaseProperty", typeof (string), "nvarchar (max)", true, false);
      _fakeTableColumnDefinition1 = new SimpleColumnDefinition ("TableProperty1", typeof (string), "nvarchar (max)", true, false);
      _fakeTableColumnDefinition2 = new SimpleColumnDefinition ("TableProperty2", typeof (string), "nvarchar (max)", true, false);
      _fakeDerivedColumnDefinition1 = new SimpleColumnDefinition ("DerivedProperty1", typeof (string), "nvarchar (max)", true, false);
      _fakeDerivedColumnDefinition2 = new SimpleColumnDefinition ("DerivedProperty2", typeof (string), "nvarchar (max)", true, false);
      _fakeDerivedDerivedColumnDefinition = new SimpleColumnDefinition ("DerivedDerivedProperty", typeof (string), "nvarchar (max)", true, false);
      _fakeIDColumnDefinition = _columnDefinitionFactory.CreateIDColumnDefinition();
      _fakeTimestampColumnDefinition = _columnDefinitionFactory.CreateTimestampColumnDefinition();

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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition1,
              _fakeTableColumnDefinition2, _fakeDerivedColumnDefinition1, _fakeDerivedColumnDefinition2, _fakeDerivedDerivedColumnDefinition
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition1,
              _fakeTableColumnDefinition2, _fakeDerivedColumnDefinition1, _fakeDerivedColumnDefinition2, _fakeDerivedDerivedColumnDefinition
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition1
          },
          new PrimaryKeyConstraintDefinition ("PK_Table1Class", true, new[] { (SimpleColumnDefinition) _fakeIDColumnDefinition.ObjectIDColumn })
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition2,
              _fakeDerivedColumnDefinition1, _fakeDerivedColumnDefinition2, _fakeDerivedDerivedColumnDefinition
          },
          new PrimaryKeyConstraintDefinition ("PK_Table2Class", true, new[] { (SimpleColumnDefinition) _fakeIDColumnDefinition.ObjectIDColumn })
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition2, _fakeDerivedColumnDefinition1
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition2, _fakeDerivedColumnDefinition2, _fakeDerivedDerivedColumnDefinition
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
          new IColumnDefinition[]
          {
              _fakeIDColumnDefinition, _fakeTimestampColumnDefinition, _fakeBaseBaseColumnDefinition, _fakeBaseColumnDefinition,
              _fakeTableColumnDefinition2, _fakeDerivedColumnDefinition2, _fakeDerivedDerivedColumnDefinition
          });
    }

    private void AssertTableDefinition (
        ClassDefinition classDefinition,
        string storageProviderID,
        string tableName,
        string viewName,
        IColumnDefinition[] columnDefinitions,
        PrimaryKeyConstraintDefinition primaryKeyConstraintDefinition)
    {
      Assert.That (classDefinition.StorageEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (classDefinition.StorageEntityDefinition.StorageProviderID, Is.EqualTo (storageProviderID));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).TableName, Is.EqualTo (tableName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).ViewName, Is.EqualTo (viewName));
      Assert.That (((TableDefinition) classDefinition.StorageEntityDefinition).GetColumns(), Is.EqualTo (columnDefinitions));
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
  }
}