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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  [TestFixture]
  public class SqlDbCommandBuilderFactoryTest : StandardMappingTest
  {
    private ISqlDialect _sqlDialectStub;
    private IValueConverter _valueConverterStub;
    private SqlDbCommandBuilderFactory _factory;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;
    private TableDefinition _tableDefinition;
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private IOrderedColumnsSpecification _orderedColumnStub;
    private ObjectID _objectID;
    private IStorageNameProvider _storageNameProviderStub;
    private DataContainer _dataContainer;
    private IInsertedColumnsSpecification _insertedColumnsStub;
    private IComparedColumnsSpecification _comparedColumnsStub;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _factory = new SqlDbCommandBuilderFactory (_sqlDialectStub, _valueConverterStub, _storageNameProviderStub);

      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.IDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);
      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _orderedColumnStub = MockRepository.GenerateStub<IOrderedColumnsSpecification>();
      _insertedColumnsStub = MockRepository.GenerateStub<IInsertedColumnsSpecification>();
      _comparedColumnsStub = MockRepository.GenerateStub<IComparedColumnsSpecification>();

      _objectID = new ObjectID ("Order", Guid.NewGuid());
      _dataContainer = DataContainer.CreateNew (_objectID);
    }

    [Test]
    public void CreateForSingleIDLookupFromTable ()
    {
      var result = _factory.CreateForSingleIDLookupFromTable (_tableDefinition, _selectedColumnsStub, _objectID);

      Assert.That (result, Is.TypeOf (typeof (SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (dbCommandBuilder.SelectedColumns, Is.SameAs (_selectedColumnsStub));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumnsSpecification).ComparedColumnValues[0].Column,
          Is.SameAs (_tableDefinition.IDColumn));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumnsSpecification).ComparedColumnValues[0].Value, Is.SameAs (_objectID.Value));
    }

    [Test]
    public void CreateForMultiIDLookupFromTable ()
    {
      var result = _factory.CreateForMultiIDLookupFromTable (_tableDefinition, _selectedColumnsStub, new[] { _objectID });

      Assert.That (result, Is.TypeOf (typeof (SqlXmlMultiIDLookupSelectDbCommandBuilder)));
      var dbCommandBuilder = (SqlXmlMultiIDLookupSelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (dbCommandBuilder.SelectedColumns, Is.SameAs (_selectedColumnsStub));
      Assert.That (dbCommandBuilder.ObjectIDs, Is.EqualTo(new[]{_objectID}));
      
    }

    [Test]
    public void CreateForRelationLookupFromTable ()
    {
      var result = _factory.CreateForRelationLookupFromTable (
          _tableDefinition, _selectedColumnsStub, _foreignKeyColumnDefinition, _objectID, _orderedColumnStub);

      Assert.That (result, Is.TypeOf (typeof (TableRelationLookupSelectDbCommandBuilder)));
      var dbCommandBuilder = (TableRelationLookupSelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (dbCommandBuilder.SelectedColumns, Is.SameAs (_selectedColumnsStub));
      Assert.That (dbCommandBuilder.ForeignKeyValue, Is.EqualTo (_objectID));
      Assert.That (dbCommandBuilder.ForeignKeyColumn, Is.SameAs (_foreignKeyColumnDefinition));
      Assert.That (dbCommandBuilder.OrderedColumns, Is.SameAs(_orderedColumnStub));
    }

    [Test]
    public void CreateForRelationLookupFromUnionView ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionEntity"),
          _tableDefinition);

      var result = _factory.CreateForRelationLookupFromUnionView (
          unionViewDefinition, _selectedColumnsStub, _foreignKeyColumnDefinition, _objectID, _orderedColumnStub);

      Assert.That (result, Is.TypeOf (typeof (UnionRelationLookupSelectDbCommandBuilder)));
      var dbCommandBuilder = (UnionRelationLookupSelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.UnionViewDefinition, Is.SameAs (unionViewDefinition));
      Assert.That (dbCommandBuilder.SelectedColumns, Is.SameAs (_selectedColumnsStub));
      Assert.That (dbCommandBuilder.ForeignKeyValue, Is.EqualTo (_objectID));
      Assert.That (dbCommandBuilder.ForeignKeyColumn, Is.SameAs (_foreignKeyColumnDefinition));
      Assert.That (dbCommandBuilder.OrderedColumns, Is.SameAs (_orderedColumnStub));
    }

    [Test]
    public void CreateForQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();

      var result = _factory.CreateForQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (QueryDbCommandBuilder)));
      Assert.That (((QueryDbCommandBuilder) result).Query, Is.SameAs (queryStub));
      Assert.That (((QueryDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((QueryDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
    }

    [Test]
    public void CreateForInsert ()
    {
      var result = _factory.CreateForInsert (_tableDefinition, _insertedColumnsStub);

      Assert.That (result, Is.TypeOf (typeof (InsertDbCommandBuilder)));
      Assert.That (((InsertDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((InsertDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((InsertDbCommandBuilder) result).TableDefinition, Is.SameAs (_tableDefinition));
      Assert.That (((InsertDbCommandBuilder) result).InsertedColumnsSpecification, Is.SameAs (_insertedColumnsStub));
    }

    [Test]
    public void CreateForUpdate ()
    {
      var result = _factory.CreateForUpdate (_dataContainer);

      Assert.That (result, Is.TypeOf (typeof (LegacyUpdateDbCommandBuilder)));
      Assert.That (((LegacyUpdateDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((LegacyUpdateDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((LegacyUpdateDbCommandBuilder) result).StorageNameProvider, Is.SameAs (_storageNameProviderStub));
    }

    [Test]
    public void CreateForDelete ()
    {
      var result = _factory.CreateForDelete (_tableDefinition, _comparedColumnsStub);

      Assert.That (result, Is.TypeOf (typeof (DeleteDbCommandBuilder)));
      Assert.That (((DeleteDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((DeleteDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((DeleteDbCommandBuilder) result).TableDefinition, Is.SameAs (_tableDefinition));
      Assert.That (((DeleteDbCommandBuilder) result).ComparedColumnsSpecification, Is.SameAs (_comparedColumnsStub));
    }
  }
}