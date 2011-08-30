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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class QueryDbCommandBuilderTest
  {
    private IStorageTypeInformation _storageTypeInformationMock1;
    private IStorageTypeInformation _storageTypeInformationMock2;
    private QueryParameter _queryParameter1;
    private QueryParameter _queryParameter2;
    private QueryParameterWithType _queryParameterWithType1;
    private QueryParameterWithType _queryParameterWithType2;
    private QueryDbCommandBuilder _commandBuilder;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationMock1 = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
      _storageTypeInformationMock2 = MockRepository.GenerateStrictMock<IStorageTypeInformation>();

      _queryParameter1 = new QueryParameter ("@param1", 5,QueryParameterType.Value);
      _queryParameter2 = new QueryParameter ("@param2", "test", QueryParameterType.Text);

      _queryParameterWithType1 = new QueryParameterWithType (_queryParameter1, _storageTypeInformationMock1);
      _queryParameterWithType2 = new QueryParameterWithType (_queryParameter2, _storageTypeInformationMock2);

      _commandBuilder = new QueryDbCommandBuilder (
          "Statement @param1 @param2", new[] { _queryParameterWithType1, _queryParameterWithType2 }, MockRepository.GenerateStub<ISqlDialect>());
    }

    [Test]
    public void Create ()
    {
      var dataParameterCollectionStrictMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();

      var dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      dbCommandStub.Stub (stub => stub.Parameters).Return (dataParameterCollectionStrictMock);

      var executionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      executionContextStub.Stub (stub => stub.CreateDbCommand()).Return(dbCommandStub);

      var dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _storageTypeInformationMock1
        .Expect (mock => mock.CreateDataParameter (dbCommandStub, 5))
        .Return (dbDataParameterStub);
      _storageTypeInformationMock1.Replay();
      _storageTypeInformationMock2.Replay();

      dataParameterCollectionStrictMock.Expect (mock => mock.Add (dbDataParameterStub)).Return (0);
      dataParameterCollectionStrictMock.Replay();
      
      var result = _commandBuilder.Create (executionContextStub);

      dataParameterCollectionStrictMock.VerifyAllExpectations();
      _storageTypeInformationMock1.VerifyAllExpectations();
      _storageTypeInformationMock2.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (dbCommandStub));
      Assert.That (result.CommandText, Is.EqualTo ("Statement @param1 test"));
    }
  }
}