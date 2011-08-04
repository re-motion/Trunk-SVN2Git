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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class SqlXmlSetComparedColumnSpecificationTest
  {
    private ColumnDefinition _columnDefinition;
    private string _objectValue1;
    private int _objectValue2;
    private object _objectValue3;
    private SqlXmlSetComparedColumnSpecification _specification;
    private StringBuilder _statement;
    private IDataParameterCollection _parametersCollectionMock;
    private IDbCommand _commandStub;
    private ISqlDialect _sqlDialectStub;
    private IDbDataParameter _parameterStub;


    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn ("Column");
      _objectValue1 = "<Test1";
      _objectValue2 = 689;
      _objectValue3 = null;
      
      _specification = new SqlXmlSetComparedColumnSpecification (_columnDefinition, new[] { _objectValue1, _objectValue2, _objectValue3 });

      _statement = new StringBuilder ();

      _parametersCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();

      _commandStub = MockRepository.GenerateStub<IDbCommand> ();
      _commandStub.Stub (stub => stub.Parameters).Return (_parametersCollectionMock);
      
      _parameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _commandStub.Stub (stub => stub.CreateParameter ()).Return (_parameterStub);

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _sqlDialectStub.Stub (stub => stub.StatementDelimiter).Return ("delimiter");
    }

    [Test]
    public void AppendComparisons ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column")).Return ("[delimited Column]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("Column")).Return ("pColumn");
      
      _parametersCollectionMock.Expect (mock => mock.Add (_parameterStub)).Return (0);
      _parametersCollectionMock.Replay ();

      _specification.AppendComparisons (_statement, _commandStub, _sqlDialectStub, null);

      Assert.That (_statement.ToString (), Is.EqualTo ("[delimited Column] IN (SELECT T.c.value('.', 'varchar') FROM pColumn.nodes('/L/I') T(c))delimiter"));
      Assert.That (_parameterStub.Value, Is.EqualTo ("<L><I>&lt;Test1</I><I>689</I><I /></L>"));
      Assert.That (_parameterStub.DbType, Is.EqualTo (DbType.Xml));
      Assert.That (_parameterStub.ParameterName, Is.EqualTo ("pColumn"));
    }
  }
}