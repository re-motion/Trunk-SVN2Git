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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class SingleIDLookupSelectDbCommandBuilderTest : StandardMappingTest
  {
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private ISqlDialect _sqlDialectMock;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private IValueConverter _valueConverterStub;
    private ObjectID _objectID;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _selectedColumnsStub
        .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
        .WhenCalled(mi=> ((StringBuilder) mi.Arguments[0]).Append("[Column1], [Column2], [Column3]"));

      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect>();
      _sqlDialectMock.Stub (stub => stub.StatementDelimiter).Return (";");
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub(stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);
      
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand ()).Return (_dbCommandStub);

      var guid = Guid.NewGuid ();
      _objectID = new ObjectID ("Order", guid);

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter> ();
    }

    [Test]
    public void Create_DefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      var builder = new SingleIDLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _objectID,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("ID")).Return ("[ID]");
      _sqlDialectMock.Expect (stub => stub.GetParameterName ("ID")).Return ("@ID");
      _sqlDialectMock.Replay();

      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay ();

      _valueConverterStub.Stub (stub => stub.GetDBValue (_objectID)).Return (_objectID.Value);

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations();
      _dataParameterCollectionMock.VerifyAllExpectations ();

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [Table] WHERE [ID] = @ID;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_objectID.Value));
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new SingleIDLookupSelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _objectID,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Expect (stub => stub.DelimitIdentifier ("ID")).Return ("[ID]");
      _sqlDialectMock.Expect (stub => stub.GetParameterName ("ID")).Return ("@ID");
      _sqlDialectMock.Replay ();

      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay ();

      _valueConverterStub.Stub (stub => stub.GetDBValue (_objectID)).Return (_objectID.Value);

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations ();
      _dataParameterCollectionMock.VerifyAllExpectations ();

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [customSchema].[Table] WHERE [ID] = @ID;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_objectID.Value));
    }
  }
}