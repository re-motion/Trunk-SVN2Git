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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest : StandardMappingTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ClassDefinition _classDefinition;
    private IStorageNameProvider _storageNameProvider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _storageNameProvider = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProvider.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver(_storageNameProvider);
      _classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
    }

    [Test]
    public void ResolveEntity ()
    {
      var primaryKeyColumn = new SimpleColumnDefinition ("ID", typeof (ObjectID), "uniqueidentifier", false, true);
      var classIDColumn = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false);
      var foreignKeyColumn = new SimpleColumnDefinition ("ForeignKey", typeof (int), "integer", false, false);
      var simpleColumn = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", true, false);
      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "Test"),
          null,
          new[] { primaryKeyColumn, classIDColumn, foreignKeyColumn, simpleColumn }, new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _classDefinition.SetStorageEntity (tableDefinition);

      var result = _storageSpecificExpressionResolver.ResolveEntity (_classDefinition, "o");

      var expectedIdColumn = new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "ID", true);
      var expectedClassIdColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "ClassID", false);
      var expectedForeignKeyColumn = new SqlColumnDefinitionExpression (typeof (int), "o", "ForeignKey", false);
      var expectedColumn = new SqlColumnDefinitionExpression (typeof (string), "o", "Column1", false);

      var expectedExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, expectedIdColumn, new[] { expectedIdColumn, expectedClassIdColumn, expectedForeignKeyColumn, expectedColumn });
      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveColumn_NoPrimaryKeyColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = PropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition, false);

      Assert.That (result.ColumnName, Is.EqualTo ("OrderNumber"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
      Assert.That (result.IsPrimaryKey, Is.False);
    }

    [Test]
    public void ResolveColumn_PrimaryKeyColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = PropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition, true);

      Assert.That (result.ColumnName, Is.EqualTo ("OrderNumber"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
      Assert.That (result.IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResoveIDColumn ()
    {
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = _storageSpecificExpressionResolver.ResolveIDColumn (entityExpression, _classDefinition);

      Assert.That (result.ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.IsPrimaryKey, Is.True);
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void ResolveTable ()
    {
      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTable (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("OrderView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveJoin_LeftSideIsReal_RightSideIsVirtual ()
    {
      var propertyDefinition = CreatePropertyDefinition (
          _classDefinition, "Customer", "Customer", typeof (ObjectID), true, null, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var leftEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);
      var rightEndPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = _storageSpecificExpressionResolver.ResolveJoin (entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
      Assert.That (result.ForeignTableInfo, Is.TypeOf (typeof (ResolvedSimpleTableInfo)));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableName, Is.EqualTo ("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableAlias, Is.EqualTo ("o"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).ItemType, Is.SameAs (typeof (Order)));

      Assert.That (((SqlColumnExpression) result.LeftKey).ColumnName, Is.EqualTo ("Customer"));
      Assert.That (((SqlColumnExpression) result.LeftKey).OwningTableAlias, Is.EqualTo ("c"));
      Assert.That (result.LeftKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) result.LeftKey).IsPrimaryKey, Is.False);
      Assert.That (((SqlColumnExpression) result.RightKey).ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.RightKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) result.RightKey).OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (((SqlColumnExpression) result.RightKey).IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveJoin_LeftSideIsVirtual_RightSideIsReal ()
    {
      var propertyDefinition = CreatePropertyDefinition (
          _classDefinition, "Customer", "Customer", typeof (ObjectID), true, null, StorageClass.Persistent);
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var leftEndPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var rightEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = _storageSpecificExpressionResolver.ResolveJoin (entityExpression, leftEndPointDefinition, rightEndPointDefinition, "o");

      Assert.That (((SqlColumnExpression) result.LeftKey).IsPrimaryKey, Is.True);
      Assert.That (((SqlColumnExpression) result.RightKey).IsPrimaryKey, Is.False);
    }

    private PropertyDefinition CreatePropertyDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      PropertyInfo dummyPropertyInfo = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          dummyPropertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (new SimpleColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable, false));
      return propertyDefinition;
    }
  }
}