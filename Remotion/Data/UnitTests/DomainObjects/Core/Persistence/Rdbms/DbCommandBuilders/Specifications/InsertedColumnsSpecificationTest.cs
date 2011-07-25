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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class InsertedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;

    private Guid _value1;
    private object _value2;
    private ColumnValue _columnValue1;
    private ColumnValue _columnValue2;
    private ISqlDialect _sqlDialectStub;
    private InsertedColumnsSpecification _insertedColumnsSpecification;
    private IDbCommand _dbCommandStub;
    private StringBuilder _statement;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.IDColumn;
      _column2 = ColumnDefinitionObjectMother.TimestampColumn;

      _value1 = Guid.NewGuid();
      _value2 = DateTime.Now;

      _columnValue1 = new ColumnValue (_column1, _value1);
      _columnValue2 = new ColumnValue (_column2, _value2);

      _insertedColumnsSpecification = new InsertedColumnsSpecification (new[] { _columnValue1,  _columnValue2 });

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _statement = new StringBuilder();
    }

    [Test]
    public void AppendColumnNames ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID")).Return ("[ID]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Timestamp")).Return ("[Timestamp]");

      _insertedColumnsSpecification.AppendColumnNames (_statement, _dbCommandStub, _sqlDialectStub);

      Assert.That (_statement.ToString(), Is.EqualTo ("[ID], [Timestamp]"));
    }

    [Test]
    public void AppendColumnValues ()
    {
      var parameter1Stub = MockRepository.GenerateStub<IDbDataParameter>();
      var parameter2Stub = MockRepository.GenerateStub<IDbDataParameter> ();
      
      var dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();
      dataParameterCollectionMock.Expect (mock => mock.Add (parameter1Stub)).Return(0).Repeat.Once();
      dataParameterCollectionMock.Expect (mock => mock.Add (parameter2Stub)).Return(1).Repeat.Once ();
      dataParameterCollectionMock.Replay();
      
      _dbCommandStub.Stub (stub => stub.Parameters).Return (dataParameterCollectionMock);
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (parameter1Stub).Repeat.Once();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (parameter2Stub).Repeat.Once ();
      
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("ID")).Return ("@ID");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("Timestamp")).Return ("@Timestamp");

      _insertedColumnsSpecification.AppendColumnValues (_statement, _dbCommandStub, _sqlDialectStub);

      dataParameterCollectionMock.VerifyAllExpectations();
      Assert.That (_dbCommandStub.Parameters, Is.SameAs(dataParameterCollectionMock));
    }
  }
}