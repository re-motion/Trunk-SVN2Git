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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class StorageSpecificExpressionResolverTest
  {
    private StorageSpecificExpressionResolver _storageSpecificExpressionResolver;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageSpecificExpressionResolver = new StorageSpecificExpressionResolver();
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Order));
    }

    [Test]
    public void ResolveEntity ()
    {
      var result = _storageSpecificExpressionResolver.ResolveEntity (_classDefinition, "o");

      var primaryKeyColumn = new SqlColumnDefinitionExpression (typeof (ObjectID), "o", "ID", true);
      var starColumn = new SqlColumnDefinitionExpression (typeof (Order), "o", "*", false);
      var expectedExpression = new SqlEntityDefinitionExpression (typeof (Order), "o", null, primaryKeyColumn, starColumn);
      ExpressionTreeComparer.CheckAreEqualTrees (result, expectedExpression);
    }

    [Test]
    public void ResolveTableInfo ()
    {
      var result = (ResolvedSimpleTableInfo) _storageSpecificExpressionResolver.ResolveTableInfo (_classDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.TableName, Is.EqualTo ("OrderView"));
      Assert.That (result.TableAlias, Is.EqualTo ("o"));
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void ResolveColumn_NoPrimaryKeyColumn ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
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
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (_classDefinition, StorageClass.Persistent, property);
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Order), "o", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = (SqlColumnDefinitionExpression) _storageSpecificExpressionResolver.ResolveColumn (entityExpression, propertyDefinition, true);

      Assert.That (result.ColumnName, Is.EqualTo ("OrderNumber"));
      Assert.That (result.OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (result.Type, Is.EqualTo (typeof (int)));
      Assert.That (result.IsPrimaryKey, Is.True);
    }

    [Test]
    public void ResolveJoinInfo ()
    {
      var propertyDefinition = CreatePropertyDefinition (_classDefinition, "Customer", "Customer", typeof(ObjectID), null, null,StorageClass.Persistent); 
      _classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
     
      var leftEndPointDefinition = new RelationEndPointDefinition (_classDefinition, "Customer", false);
      new RelationDefinition ("Test", leftEndPointDefinition, new AnonymousRelationEndPointDefinition (_classDefinition));
      var entityExpression = new SqlEntityDefinitionExpression (
          typeof (Customer), "c", null, new SqlColumnDefinitionExpression (typeof (string), "c", "Name", false));

      var result = _storageSpecificExpressionResolver.ResolveJoinInfo (entityExpression, leftEndPointDefinition, "o");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ItemType, Is.EqualTo (typeof (Order)));
      Assert.That (result.ForeignTableInfo, Is.TypeOf(typeof(ResolvedSimpleTableInfo)));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableName, Is.EqualTo("OrderView"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).TableAlias, Is.EqualTo ("o"));
      Assert.That (((ResolvedSimpleTableInfo) result.ForeignTableInfo).ItemType, Is.SameAs(typeof(Order)));
      
      Assert.That (((SqlColumnExpression) result.LeftKey).ColumnName, Is.EqualTo ("Customer"));
      Assert.That (((SqlColumnExpression) result.LeftKey).OwningTableAlias, Is.EqualTo ("c"));
      Assert.That (result.LeftKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) result.LeftKey).IsPrimaryKey, Is.False);
      Assert.That (((SqlColumnExpression) result.RightKey).ColumnName, Is.EqualTo ("ID"));
      Assert.That (result.RightKey.Type, Is.EqualTo (typeof (ObjectID)));
      Assert.That (((SqlColumnExpression) result.RightKey).OwningTableAlias, Is.EqualTo ("o"));
      Assert.That (((SqlColumnExpression) result.RightKey).IsPrimaryKey, Is.True);
    }

    private PropertyDefinition CreatePropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName,
        Type propertyType, bool? isNullable, int? maxLength, StorageClass storageClass)
    {
      PropertyInfo dummyPropertyInfo = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          classDefinition,
          dummyPropertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (new ColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue ? isNullable.Value : true));
      return propertyDefinition;
    }

  }
}