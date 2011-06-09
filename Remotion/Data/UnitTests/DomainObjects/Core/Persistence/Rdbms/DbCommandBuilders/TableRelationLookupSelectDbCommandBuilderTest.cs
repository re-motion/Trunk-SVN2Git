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
using System.Data;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class TableRelationLookupSelectDbCommandBuilderTest : SqlProviderBaseTest
  {
    private SimpleColumnDefinition _objectIDColumnDefinition;
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private ISqlDialect _sqlDialectStub;
    private IDbCommandFactory _commandFactoryStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private Guid _guid;
    private SimpleColumnDefinition _foreignKeyColumnDefinition;
    private IOrderedColumnsSpecification _orderedColumnsStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectIDColumnDefinition = new SimpleColumnDefinition ("ID", typeof (Guid), "uniqueidentifier", false, true);
      _foreignKeyColumnDefinition = new SimpleColumnDefinition ("FKID", typeof (Guid), "uniqueidentifier", true, false);

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification> ();
      _selectedColumnsStub
        .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
        .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append (" [Column1], [Column2], [Column3] "));

      _orderedColumnsStub = MockRepository.GenerateStub<IOrderedColumnsSpecification>();
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderByClause (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append (" ORDER BY [Column1] ASC, [Column2] DESC"));

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("dbo")).Return ("[dbo]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID")).Return ("[FKID]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("FKID")).Return ("@FKID");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("@FKID")).Return ("@FKID");

      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();

      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();
      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay ();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandFactoryStub = MockRepository.GenerateStub<IDbCommandFactory> ();
      _commandFactoryStub.Stub (stub => stub.CreateDbCommand ()).Return (_dbCommandStub);

      _guid = Guid.NewGuid ();
    }

    [Test]
    public void Create_DefaultSchema ()
    {
      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "Table"),
          null,
          _objectIDColumnDefinition,
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false),
          new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var builder = new TableRelationLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _foreignKeyColumnDefinition,
          new ObjectID ("Order", _guid),
          _orderedColumnsStub,
          _sqlDialectStub,
          (RdbmsProviderDefinition) TestDomainStorageProviderDefinition,
          Provider.CreateValueConverter ());

      var result = builder.Create (_commandFactoryStub);

      Assert.That (result.CommandText, Is.EqualTo (
        "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
      _dataParameterCollectionMock.VerifyAllExpectations ();
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = new TableDefinition (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition ("customSchema", "Table"),
          null,
          _objectIDColumnDefinition,
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", true, false),
          new SimpleColumnDefinition ("Timestamp", typeof (DateTime), "datetime", true, false),
          new SimpleColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var builder = new TableRelationLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _foreignKeyColumnDefinition,
          new ObjectID ("Order", _guid),
          _orderedColumnsStub,
          _sqlDialectStub,
          (RdbmsProviderDefinition) TestDomainStorageProviderDefinition,
          Provider.CreateValueConverter ());

      var result = builder.Create (_commandFactoryStub);

      Assert.That (result.CommandText, Is.EqualTo (
        "SELECT [Column1], [Column2], [Column3] FROM [customSchema].[Table] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
      _dataParameterCollectionMock.VerifyAllExpectations ();
    }
  }
}