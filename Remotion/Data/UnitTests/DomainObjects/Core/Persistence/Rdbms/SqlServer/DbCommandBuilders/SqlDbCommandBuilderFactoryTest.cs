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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
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
    private ObjectID _objectID;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnValue _columnValue1;
    private ColumnValue _columnValue2;
    private OrderedColumn _orderColumn1;
    private OrderedColumn _orderColumn2;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _factory = new SqlDbCommandBuilderFactory (_sqlDialectStub, _valueConverterStub, TestDomainStorageProviderDefinition);

      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.IDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);
      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));

      _column1 = ColumnDefinitionObjectMother.CreateColumn ("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");
      _columnValue1 = new ColumnValue (_column1, new object());
      _columnValue2 = new ColumnValue (_column2, new object());

      _orderColumn1 = new OrderedColumn (_column1, SortOrder.Ascending);
      _orderColumn2 = new OrderedColumn (_column2, SortOrder.Descending);

      _objectID = new ObjectID ("Order", Guid.NewGuid());
    }

    [Test]
    public void CreateForSingleIDLookupFromTable ()
    {
      var result = _factory.CreateForSingleIDLookupFromTable (_tableDefinition, new[] { _column1, _column2 }, _objectID);

      Assert.That (result, Is.TypeOf (typeof (SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (((SelectedColumnsSpecification) dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo (new[] { _column1, _column2 }));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumnsSpecification).ComparedColumnValues[0].Column,
          Is.SameAs (_tableDefinition.IDColumn));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumnsSpecification).ComparedColumnValues[0].Value, Is.SameAs (_objectID.Value));
    }

    [Test]
    public void CreateForMultiIDLookupFromTable ()
    {
      var result = _factory.CreateForMultiIDLookupFromTable (_tableDefinition, new[] { _column1, _column2 }, new[] { _objectID });

      Assert.That (result, Is.TypeOf (typeof (SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (((SelectedColumnsSpecification) dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo (new[] { _column1, _column2 }));
      Assert.That (
          ((SqlXmlSetComparedColumnSpecification) dbCommandBuilder.ComparedColumnsSpecification).ColumnDefinition,
          Is.SameAs (_tableDefinition.IDColumn));
      Assert.That (
          ((SqlXmlSetComparedColumnSpecification) dbCommandBuilder.ComparedColumnsSpecification).ObjectValues, Is.EqualTo (new[] { _objectID.Value }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Multi-ID lookups can only be performed for ObjectIDs from this storage provider.\r\nParameter name: objectIDs")]
    public void CreateForMultiIDLookupFromTable_DifferentStorageProvider ()
    {
      _factory.CreateForMultiIDLookupFromTable (_tableDefinition, new[] { _column1, _column2 }, new[] { DomainObjectIDs.Official1 });
    }

    [Test]
    public void CreateForRelationLookupFromTable ()
    {
      var result = _factory.CreateForRelationLookupFromTable (
          _tableDefinition, new[] { _column1, _column2 }, _foreignKeyColumnDefinition, _objectID, new[] { _orderColumn1, _orderColumn2 });

      Assert.That (result, Is.TypeOf (typeof (SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.Table, Is.SameAs (_tableDefinition));
      Assert.That (((SelectedColumnsSpecification) dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo (new[] { _column1, _column2 }));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumnsSpecification).ComparedColumnValues,
          Is.EqualTo (
              new[]
              { new ColumnValue (((SimpleStoragePropertyDefinition) _foreignKeyColumnDefinition.ValueProperty).ColumnDefinition, _objectID.Value) }));
      Assert.That (
          ((OrderedColumnsSpecification) dbCommandBuilder.OrderedColumnsSpecification).Columns, Is.EqualTo (new[] { _orderColumn1, _orderColumn2 }));
    }

    [Test]
    public void CreateForRelationLookupFromUnionView ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionEntity"),
          _tableDefinition);

      var result = _factory.CreateForRelationLookupFromUnionView (
          unionViewDefinition, new[] { _column1, _column2 }, _foreignKeyColumnDefinition, _objectID, new[] { _orderColumn1, _orderColumn2 });

      Assert.That (result, Is.TypeOf (typeof (UnionSelectDbCommandBuilder)));
      var dbCommandBuilder = (UnionSelectDbCommandBuilder) result;
      Assert.That (dbCommandBuilder.UnionViewDefinition, Is.SameAs (unionViewDefinition));
      Assert.That (((SelectedColumnsSpecification) dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo (new[] { _column1, _column2 }));
      Assert.That (
          ((ComparedColumnsSpecification) dbCommandBuilder.ComparedColumns).ComparedColumnValues,
          Is.EqualTo (
              new[] { new ColumnValue (
                  ((SimpleStoragePropertyDefinition)
                   _foreignKeyColumnDefinition.ValueProperty).ColumnDefinition,
                  _objectID.Value)}));
      Assert.That (((OrderedColumnsSpecification) dbCommandBuilder.OrderedColumns).Columns, Is.EqualTo (new[] { _orderColumn1, _orderColumn2 }));
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
      var result = _factory.CreateForInsert (_tableDefinition, new[] { _columnValue1, _columnValue2 });

      Assert.That (result, Is.TypeOf (typeof (InsertDbCommandBuilder)));
      Assert.That (((InsertDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((InsertDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((InsertDbCommandBuilder) result).TableDefinition, Is.SameAs (_tableDefinition));
      Assert.That (
          ((InsertedColumnsSpecification) ((InsertDbCommandBuilder) result).InsertedColumnsSpecification).ColumnValues,
          Is.EqualTo (new[] { _columnValue1, _columnValue2 }));
    }

    [Test]
    public void CreateForUpdate ()
    {
      var result = _factory.CreateForUpdate (_tableDefinition, new[] { _columnValue1, _columnValue2 }, new[] { _columnValue2, _columnValue1 });

      Assert.That (result, Is.TypeOf (typeof (UpdateDbCommandBuilder)));
      Assert.That (((UpdateDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((UpdateDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((UpdateDbCommandBuilder) result).TableDefinition, Is.SameAs (_tableDefinition));
      Assert.That (
          ((UpdatedColumnsSpecification) ((UpdateDbCommandBuilder) result).UpdatedColumnsSpecification).ColumnValues,
          Is.EqualTo (new[] { _columnValue1, _columnValue2 }));
      Assert.That (
          ((ComparedColumnsSpecification) ((UpdateDbCommandBuilder) result).ComparedColumnsSpecification).ComparedColumnValues,
          Is.EqualTo (new[] { _columnValue2, _columnValue1 }));
    }

    [Test]
    public void CreateForDelete ()
    {
      var result = _factory.CreateForDelete (_tableDefinition, new[] { _columnValue1, _columnValue2 });

      Assert.That (result, Is.TypeOf (typeof (DeleteDbCommandBuilder)));
      Assert.That (((DeleteDbCommandBuilder) result).SqlDialect, Is.SameAs (_sqlDialectStub));
      Assert.That (((DeleteDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
      Assert.That (((DeleteDbCommandBuilder) result).TableDefinition, Is.SameAs (_tableDefinition));
      Assert.That (
          ((ComparedColumnsSpecification) ((DeleteDbCommandBuilder) result).ComparedColumnsSpecification).ComparedColumnValues,
          Is.EqualTo (new[] { _columnValue1, _columnValue2 }));
    }
  }
}