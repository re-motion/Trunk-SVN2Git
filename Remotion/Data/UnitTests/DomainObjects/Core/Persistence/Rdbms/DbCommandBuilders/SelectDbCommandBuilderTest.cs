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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class SelectDbCommandBuilderTest : StandardMappingTest
  {
    private ISelectedColumnsSpecification _selectedColumnsStub;
    private ISqlDialect _sqlDialectStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;
    private IValueConverter _valueConverterStub;
    private ObjectID _objectID;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;
    private IComparedColumnsSpecification _comparedColumnsSpecificationStub;
    private IOrderedColumnsSpecification _orderedColumnsSpecificationStub;

    public override void SetUp ()
    {
      base.SetUp();

      _selectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      _selectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg<ISqlDialect>.Is.Anything))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.StatementDelimiter).Return (";");
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();

      _comparedColumnsSpecificationStub = MockRepository.GenerateStub<IComparedColumnsSpecification>();
      _orderedColumnsSpecificationStub = MockRepository.GenerateStub<IOrderedColumnsSpecification>();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);

      var guid = Guid.NewGuid();
      _objectID = new ObjectID ("Order", guid);

      _valueConverterStub = MockRepository.GenerateStub<IValueConverter>();
    }

    [Test]
    public void Create_DefaultSchema_WithOrderings ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      var builder = new SelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _comparedColumnsSpecificationStub,
          _orderedColumnsSpecificationStub,
          _sqlDialectStub,
          _valueConverterStub);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table")).Return ("[delimited Table]");

      _comparedColumnsSpecificationStub
          .Stub (stub => stub.AppendComparisons (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      _orderedColumnsSpecificationStub.Stub (stub => stub.IsEmpty).Return (false);
      _orderedColumnsSpecificationStub
          .Stub (stub => stub.AppendOrderings (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Name] ASC, [City] DESC"));

      _valueConverterStub.Stub (stub => stub.GetDBValue (_objectID)).Return (_objectID.Value);

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (result.CommandText, Is.EqualTo (
        "SELECT [Column1], [Column2], [Column3] FROM [delimited Table] WHERE [ID] = @ID ORDER BY [Name] ASC, [City] DESC;"));
    }

    [Test]
    public void Create_CustomSchema_NoOrderings ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new SelectDbCommandBuilder (
          tableDefinition,
          _selectedColumnsStub,
          _comparedColumnsSpecificationStub,
          _orderedColumnsSpecificationStub,
          _sqlDialectStub,
          _valueConverterStub);

      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("customSchema")).Return ("[delimited customSchema]");
      _sqlDialectStub.Stub (mock => mock.DelimitIdentifier ("Table")).Return ("[delimited Table]");

      _comparedColumnsSpecificationStub
          .Stub (stub => stub.AppendComparisons (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));

      _orderedColumnsSpecificationStub.Stub (stub => stub.IsEmpty).Return (true);
      
      _valueConverterStub.Stub (stub => stub.GetDBValue (_objectID)).Return (_objectID.Value);

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (result.CommandText, Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [delimited customSchema].[delimited Table] WHERE [ID] = @ID;"));
    }
  }
}