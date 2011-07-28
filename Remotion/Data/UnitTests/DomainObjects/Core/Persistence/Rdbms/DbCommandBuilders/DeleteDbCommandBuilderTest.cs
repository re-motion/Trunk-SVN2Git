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
  public class DeleteDbCommandBuilderTest : SqlProviderBaseTest
  {
    private IValueConverter _valueConverterStub;
    private IComparedColumnsSpecification _comparedColumnsSpecificationStub;
    private ISqlDialect _sqlDialectStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private IDbCommand _dbCommandStub;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
      _comparedColumnsSpecificationStub = MockRepository.GenerateStub<IComparedColumnsSpecification> ();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _sqlDialectStub.Stub (stub => stub.StatementDelimiter).Return (";");
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
      var builder = new DeleteDbCommandBuilder (
          tableDefinition,
          _comparedColumnsSpecificationStub,
          _sqlDialectStub,
          _valueConverterStub);

      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("Table")).Return ("[delimited Table]");

      _comparedColumnsSpecificationStub
          .Stub (stub => stub.AppendComparisons (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (result.CommandText, Is.EqualTo ("DELETE FROM [delimited Table] WHERE [ID] = @ID;"));
    }

    [Test]
    public void Create_WithCustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new DeleteDbCommandBuilder (
          tableDefinition,
          _comparedColumnsSpecificationStub,
          _sqlDialectStub,
          _valueConverterStub);

      _sqlDialectStub.Expect (mock => mock.DelimitIdentifier ("Table")).Return ("[delimited Table]");
      _sqlDialectStub.Expect (mock => mock.DelimitIdentifier ("customSchema")).Return ("[delimited customSchema]");

      _comparedColumnsSpecificationStub
          .Stub (stub => stub.AppendComparisons (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (result.CommandText, Is.EqualTo ("DELETE FROM [delimited customSchema].[delimited Table] WHERE [ID] = @ID;"));
    }
  }
}