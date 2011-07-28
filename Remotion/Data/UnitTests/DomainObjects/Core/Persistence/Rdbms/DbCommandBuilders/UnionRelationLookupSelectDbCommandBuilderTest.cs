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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class UnionRelationLookupSelectDbCommandBuilderTest : StandardMappingTest
  {
    private ISelectedColumnsSpecification _originalSelectedColumnsStub;
    private ISqlDialect _sqlDialectMock;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private Guid _guid;
    private ObjectIDStoragePropertyDefinition _foreignKeyColumnDefinition;
    private IOrderedColumnsSpecification _orderedColumnsStub;
    private TableDefinition _table1;
    private TableDefinition _table2;
    private TableDefinition _table3;
    private IValueConverter _valueConverterStub;
    private ISelectedColumnsSpecification _fullSelectedColumnsStub;
    private ObjectID _objectID;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _originalSelectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification> ();
      _fullSelectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification> ();
      _orderedColumnsStub = MockRepository.GenerateStub<IOrderedColumnsSpecification> ();
      _foreignKeyColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("FKID"),
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("FKIDClassID"));

      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect>();
      _sqlDialectMock.Stub (stub => stub.StatementDelimiter).Return (";");

      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();

      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();
      _dataParameterCollectionMock.Expect (mock => mock.Add (_dbDataParameterStub)).Return (1);
      _dataParameterCollectionMock.Replay();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);

      _guid = Guid.NewGuid();
      _objectID = new ObjectID ("Order", _guid);
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _valueConverterStub.Stub (stub => stub.GetDBValue (_objectID)).Return (_guid);

      _table1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _table2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table2"));
      _table3 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table3"));
    }

    [Test]
    public void Create_WithOneUnionedTable ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, null, _table1);

      var builder = new UnionRelationLookupSelectDbCommandBuilder (
          unionViewDefinition,
          _originalSelectedColumnsStub,
          _foreignKeyColumnDefinition,
          _objectID,
          _orderedColumnsStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[Table1]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("FKID")).Return ("[FKID]");
      _sqlDialectMock.Stub (stub => stub.GetParameterName ("FKID")).Return ("@FKID");
      _sqlDialectMock.Replay ();

      _orderedColumnsStub.Stub (stub => stub.UnionWithSelectedColumns (_originalSelectedColumnsStub)).Return (_fullSelectedColumnsStub);
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderByClause (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append (" ORDER BY [Column1] ASC, [Column2] DESC"));

      _fullSelectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));
      
      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (
          result.CommandText,
          Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [Table1] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
      _dataParameterCollectionMock.VerifyAllExpectations();
      _sqlDialectMock.VerifyAllExpectations();
    }

    [Test]
    public void Create_WithSeveralUnionedTables ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          null,
          _table1,
          _table2,
          _table3);

      var builder = new UnionRelationLookupSelectDbCommandBuilder (
          unionViewDefinition,
          _originalSelectedColumnsStub,
          _foreignKeyColumnDefinition,
          _objectID,
          _orderedColumnsStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[Table1]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("Table2")).Return ("[Table2]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("Table3")).Return ("[Table3]");
      _sqlDialectMock.Stub (stub => stub.DelimitIdentifier ("FKID")).Return ("[FKID]");
      _sqlDialectMock.Stub (stub => stub.GetParameterName ("FKID")).Return ("@FKID");
      _sqlDialectMock.Replay ();

      _orderedColumnsStub.Stub (stub => stub.UnionWithSelectedColumns (_originalSelectedColumnsStub)).Return (_fullSelectedColumnsStub);
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderByClause (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append (" ORDER BY [Column1] ASC, [Column2] DESC"));

      _fullSelectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "SELECT [Column1], [Column2], [Column3] FROM [Table1] WHERE [FKID] = @FKID UNION ALL "
              + "SELECT [Column1], [Column2], [Column3] FROM [Table2] WHERE [FKID] = @FKID UNION ALL "
              + "SELECT [Column1], [Column2], [Column3] FROM [customSchema].[Table3] WHERE [FKID] = @FKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      Assert.That (_dbDataParameterStub.Value, Is.EqualTo (_guid));
      _dataParameterCollectionMock.VerifyAllExpectations();
      _sqlDialectMock.VerifyAllExpectations();
    }
  }
}