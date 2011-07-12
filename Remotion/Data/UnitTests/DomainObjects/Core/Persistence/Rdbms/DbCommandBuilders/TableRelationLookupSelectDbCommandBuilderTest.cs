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
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class TableRelationLookupSelectDbCommandBuilderTest : StandardMappingTest
  {
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private ISqlDialect _sqlDialectMock;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private Guid _guid;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;
    private IOrderedColumnsSpecification _orderedColumnsStub;
    private IValueConverter _valueConverterStub;
    private ObjectID _objectID;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      var foreignKeyColumn = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("FKID");
      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (foreignKeyColumn, foreignKeyColumn);

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _selectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));

      _orderedColumnsStub = MockRepository.GenerateStub<IOrderedColumnsSpecification>();
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderByClause (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append (" ORDER BY [Column1] ASC, [Column2] DESC"));

      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect>();
      _sqlDialectMock.Stub (stub => stub.StatementDelimiter).Return (";");

      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();

      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();

      _guid = Guid.NewGuid();
      _objectID = new ObjectID ("Order", _guid);
      _valueConverterStub.Stub (stub => stub.GetDBValue (Arg.Is (_objectID))).Return (_guid);
    }

    [Test]
    public void Create_DefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));

      var builder = new TableRelationLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _foreignKeyColumnDefinition,
          _objectID,
          _orderedColumnsStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("FKID")).Return ("[FKID]");
      _sqlDialectMock.Expect (stub => stub.GetParameterName ("FKID")).Return ("@FKID");
      _sqlDialectMock.Replay();

      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay();

      var result = builder.Create (_commandExecutionContextStub);

      _dataParameterCollectionMock.VerifyAllExpectations();
      _sqlDialectMock.VerifyAllExpectations();

      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "SELECT [Column1], [Column2], [Column3] FROM [Table] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));

      var builder = new TableRelationLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _foreignKeyColumnDefinition,
          _objectID,
          _orderedColumnsStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("FKID")).Return ("[FKID]");
      _sqlDialectMock.Expect (stub => stub.GetParameterName ("FKID")).Return ("@FKID");
      _sqlDialectMock.Replay();

      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay();

      var result = builder.Create (_commandExecutionContextStub);

      _dataParameterCollectionMock.VerifyAllExpectations();
      _sqlDialectMock.VerifyAllExpectations();

      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "SELECT [Column1], [Column2], [Column3] FROM [customSchema].[Table] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
    }
  }
}