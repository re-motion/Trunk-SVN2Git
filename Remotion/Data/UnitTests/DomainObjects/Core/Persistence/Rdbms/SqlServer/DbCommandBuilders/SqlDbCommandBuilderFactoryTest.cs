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
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  [TestFixture]
  public class SqlDbCommandBuilderFactoryTest : SqlProviderBaseTest
  {
    private ISqlDialect _sqlDialectStub;
    private ValueConverter _valueConverter;
    private SqlDbCommandBuilderFactory _factory;
    private SimpleColumnDefinition _objectIDColumnDefinition;
    private IDColumnDefinition _foreignKeyColumnDefinition;
    private SimpleColumnDefinition _classIDColumnDefinition;
    private SimpleColumnDefinition _timstampColumnDefinition;
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private IOrderedColumnsSpecification _orderedColumnStub;
    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _valueConverter = Provider.CreateValueConverter();
      _factory = new SqlDbCommandBuilderFactory (_sqlDialectStub, _valueConverter);

      _objectIDColumnDefinition = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      _foreignKeyColumnDefinition = new IDColumnDefinition (
          new SimpleColumnDefinition ("FKID", typeof (Guid), "uniqueidentifier", true, false), _objectIDColumnDefinition);
      _classIDColumnDefinition = new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false);
      _timstampColumnDefinition = new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false);
      _tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          _objectIDColumnDefinition,
          _classIDColumnDefinition,
          _timstampColumnDefinition);
      _unionViewDefinition = new UnionViewDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionEntity"),
          new[] { _tableDefinition },
          _objectIDColumnDefinition,
          _classIDColumnDefinition,
          _timstampColumnDefinition,
          new SimpleColumnDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

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
      var result = _factory.CreateForRelationLookupFromUnionView (
          _unionViewDefinition, _selectedColumnsStub, _foreignKeyColumnDefinition, _objectID, _orderedColumnStub);

      Assert.That (result, Is.TypeOf (typeof (UnionRelationLookupSelectDbCommandBuilder)));
    }
  }
}