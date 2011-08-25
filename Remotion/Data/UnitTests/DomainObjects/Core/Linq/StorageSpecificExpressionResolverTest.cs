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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest : StandardMappingTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ClassDefinition _classDefinition;
    private IStorageNameProvider _storageNameProvider;
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private IRdbmsStoragePropertyDefinition _rdbmsStoragePropertyDefinitionStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageNameProvider = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProvider.Stub (stub => stub.IDColumnName).Return ("ID");
      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _rdbmsStoragePropertyDefinitionStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();

      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver (_rdbmsPersistenceModelProviderStub);
      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
    }

    [Test]
    public void ResolveEntity ()
    {
      var objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      var timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      var foreignKeyProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ForeignKey");
      var simpleProperty = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          null,
          objectIDProperty,
          timestampProperty,
          foreignKeyProperty,
          simpleProperty);
      _classDefinition.SetStorageEntity (tableDefinition);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IEntityDefinition);

      var result = _storageSpecificExpressionResolver.ResolveEntity (_classDefinition, "o");

      var expectedIdColumn = new SqlColumnDefinitionExpression (typeof (Guid), "o", "ID", true);
      var expectedClassIdColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "ClassID", false);
      var expectedTimestampColumn = new SqlColumnDefinitionExpression (typeof (DateTime), "o", "Timestamp", false);
      var expectedForeignKeyColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "ForeignKey", false);
      var expectedColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "Column1", false);

      var expectedExpression = new SqlEntityDefinitionExpression (
          typeof (Order),
          "o",
          null,
          expectedIdColumn,
          new[] { expectedIdColumn, expectedClassIdColumn, expectedTimestampColumn, expectedForeignKeyColumn, expectedColumn });
      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveColumn_NoPrimaryKeyColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = PropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("OrderNumber");
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumns ())
          .Return (new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("OrderNumber"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.EqualTo (columnDefinition.PropertyType));
      Assert.That (result.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveColumn_PrimaryKeyColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = PropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      var columnDefinition = ColumnDefinitionObjectMother.IDColumn;
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumns())
          .Return (new[] { columnDefinition });

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.EqualTo (columnDefinition.PropertyType));
      Assert.That (result.IsPrimaryKey, Is.True);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Compound-column properties are not supported by this LINQ provider.")]
    public void ResolveColumn_CompoundColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = PropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumns())
          .Return (new[] { ColumnDefinitionObjectMother.IDColumn, ColumnDefinitionObjectMother.ClassIDColumn });

      _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition);
    }

    [Test]
    public void ResoveIDColumn ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));
      var entityDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Test"));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (entityDefinition);

      var result = _storageSpecificExpressionResolver.ResolveIDColumn (entityExpression, _classDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.IsPrimaryKey, Is.True);
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.SameAs (entityDefinition.IDColumn.PropertyType));
    }

    [Test]
    public void ResolveTable_TableDefinition ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), new EntityNameDefinition (null, "TableView"));
      _classDefinition.SetStorageEntity (tableDefinition);

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (_classDefinition))
          .Return (_classDefinition.StorageEntityDefinition as IEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("TableView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_FilterViewDefinition ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView"),
          (IEntityDefinition) _classDefinition.StorageEntityDefinition);
      _classDefinition.SetStorageEntity (filterViewDefinition);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("FilterView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveTable_UnionViewDefinition ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new[] { (IEntityDefinition) _classDefinition.StorageEntityDefinition });
      _classDefinition.SetStorageEntity (unionViewDefinition);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IEntityDefinition);

      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("UnionView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Class 'Order' does not have an associated database table and can therefore not be queried.")]
    public void ResolveTable_NullEntityDefinition ()
    {
      _classDefinition.SetStorageEntity (new NullEntityDefinition (TestDomainStorageProviderDefinition));

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (_classDefinition)).Return (
          _classDefinition.StorageEntityDefinition as IEntityDefinition);

      _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");
    }

    [Test]
    public void ResolveJoin_LeftSideIsReal_RightSideIsVirtual ()
    {
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var leftEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var entityDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, 
          new EntityNameDefinition (null, "OrderTable"), 
          new EntityNameDefinition (null, "OrderView"));
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (leftEndPointDefinition.ClassDefinition))
          .Return (entityDefinition);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (leftEndPointDefinition.PropertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("Customer");
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumnForLookup())
          .Return (columnDefinition);

      var result = _storageSpecificExpressionResolver.ResolveJoin (entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
      Assert.That (result.ForeignTableInfo, Is.TypeOf (typeof (ResolvedSimpleTableInfo)));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableName, Is.EqualTo ("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableAlias, Is.EqualTo ("o"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).ItemType, Is.SameAs (typeof (Order)));

      Assert.That (((SqlColumnExpression) result.LeftKey).ColumnName, Is.EqualTo ("Customer"));
      Assert.That (((SqlColumnExpression) result.LeftKey).OwningTableAlias, Is.EqualTo ("c"));
      Assert.That (result.LeftKey.Type, Is.EqualTo (columnDefinition.PropertyType));
      Assert.That (((SqlColumnExpression) result.LeftKey).IsPrimaryKey, Is.False);
      Assert.That (((SqlColumnExpression) result.RightKey).ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.RightKey.Type, Is.EqualTo (entityDefinition.IDColumn.PropertyType));
      Assert.That (((SqlColumnExpression) result.RightKey).OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (((SqlColumnExpression) result.RightKey).IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveJoin_LeftSideIsVirtual_RightSideIsReal ()
    {
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer");
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var leftEndPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var rightEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetEntityDefinition (rightEndPointDefinition.ClassDefinition))
          .Return (rightEndPointDefinition.ClassDefinition.StorageEntityDefinition as IEntityDefinition);
      _rdbmsPersistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (rightEndPointDefinition.PropertyDefinition))
          .Return (_rdbmsStoragePropertyDefinitionStub);
      _rdbmsStoragePropertyDefinitionStub
          .Stub (stub => stub.GetColumnForLookup()).Return (ColumnDefinitionObjectMother.CreateColumn ("Customer"));

      var result = _storageSpecificExpressionResolver.ResolveJoin (entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      Assert.That (((SqlColumnExpression) result.LeftKey).IsPrimaryKey, Is.True);
      Assert.That (((SqlColumnExpression) result.RightKey).IsPrimaryKey, Is.False);
    }

    private PropertyDefinition CreatePropertyDefinition (ClassDefinition classDefinition, string propertyName, string columnName)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          MockRepository.GenerateStub<IPropertyInformation>(),
          propertyName,
          true,
          true,
          null,
          StorageClass.Persistent);
      propertyDefinition.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty (columnName));
      return propertyDefinition;
    }
  }
}