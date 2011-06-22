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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
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
    private IDColumnDefinition _foreignKeyColumnDefinition;
    private TableDefinition _tableDefinition;
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private IOrderedColumnsSpecification _orderedColumnStub;
    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _factory = new SqlDbCommandBuilderFactory (_sqlDialectStub, _valueConverterStub);

      _foreignKeyColumnDefinition = new IDColumnDefinition (ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn);
      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _orderedColumnStub = MockRepository.GenerateStub<IOrderedColumnsSpecification>();

      _objectID = new ObjectID ("Order", Guid.NewGuid());
    }

    [Test]
    public void CreateForSingleIDLookupFromTable ()
    {
      var result = _factory.CreateForSingleIDLookupFromTable (_tableDefinition, _selectedColumnsStub, _objectID);

      Assert.That (result, Is.TypeOf (typeof (SingleIDLookupSelectDbCommandBuilder)));
    }

    [Test]
    public void CreateForMultiIDLookupFromTable ()
    {
      var result = _factory.CreateForMultiIDLookupFromTable (_tableDefinition, _selectedColumnsStub, new[] { _objectID });

      Assert.That (result, Is.TypeOf (typeof (SqlXmlMultiIDLookupSelectDbCommandBuilder)));
    }

    [Test]
    public void CreateForRelationLookupFromTable ()
    {
      var result = _factory.CreateForRelationLookupFromTable (
          _tableDefinition, _selectedColumnsStub, _foreignKeyColumnDefinition, _objectID, _orderedColumnStub);

      Assert.That (result, Is.TypeOf (typeof (TableRelationLookupSelectDbCommandBuilder)));
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
    }

    [Test]
    public void CreateForQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();

      var result = _factory.CreateForQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (QueryDbCommandBuilder)));
      Assert.That (((QueryDbCommandBuilder) result).Query, Is.SameAs (queryStub));
      Assert.That (((QueryDbCommandBuilder) result).SqlDialect, Is.SameAs(_sqlDialectStub));
      Assert.That (((QueryDbCommandBuilder) result).ValueConverter, Is.SameAs (_valueConverterStub));
    }
  }
}