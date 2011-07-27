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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class UpdateDbCommandBuilderTest : SqlProviderBaseTest
  {
    private IValueConverter _valueConverterStub;
    private IComparedColumnsSpecification _comparedColumnsSpecificationStub;
    private IUpdatedColumnsSpecification _updatedColumnsSpecificationStub;
    private ISqlDialect _sqlDialectMock;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private IDbCommand _dbCommandStub;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter> ();
      _comparedColumnsSpecificationStub = MockRepository.GenerateStub<IComparedColumnsSpecification> ();
      
      _updatedColumnsSpecificationStub = MockRepository.GenerateStub<IUpdatedColumnsSpecification>();
      _updatedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnValueAssignments(Arg<StringBuilder>.Is.Anything, Arg<IDbCommand>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1] = 5, [Column2] = 'test', [Column3] = true"));


      _sqlDialectMock = MockRepository.GenerateStrictMock<ISqlDialect> ();
      _sqlDialectMock.Stub (stub => stub.StatementDelimiter).Return (";");
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand ()).Return (_dbCommandStub);
    }

    [Test]
    public void Create_WithDefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      var builder = new UpdateDbCommandBuilder (
          tableDefinition,
          _updatedColumnsSpecificationStub,
          _comparedColumnsSpecificationStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (mock => mock.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Replay ();

      _comparedColumnsSpecificationStub
          .Stub (
              stub => stub.AppendComparisons (
                  Arg<StringBuilder>.Matches (sb => sb.ToString () == "UPDATE [Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE "),
                  Arg.Is (_dbCommandStub),
                  Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (result.CommandText, Is.EqualTo ("UPDATE [Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE [ID] = @ID;"));
    }

    [Test]
    public void Create_WithCustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new UpdateDbCommandBuilder (
          tableDefinition,
          _updatedColumnsSpecificationStub,
          _comparedColumnsSpecificationStub,
          _sqlDialectMock,
          _valueConverterStub);

      _sqlDialectMock.Expect (mock => mock.DelimitIdentifier ("Table")).Return ("[Table]");
      _sqlDialectMock.Expect (mock => mock.DelimitIdentifier ("customSchema")).Return ("[customSchema]");
      _sqlDialectMock.Replay ();

      _comparedColumnsSpecificationStub
          .Stub (
              stub => stub.AppendComparisons (
                  Arg<StringBuilder>.Matches (sb => sb.ToString () == "UPDATE [customSchema].[Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE "),
                  Arg.Is (_dbCommandStub),
                  Arg.Is (_sqlDialectMock)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      var result = builder.Create (_commandExecutionContextStub);

      _sqlDialectMock.VerifyAllExpectations ();
      Assert.That (result.CommandText, Is.EqualTo ("UPDATE [customSchema].[Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE [ID] = @ID;"));
    }

    
  }
}