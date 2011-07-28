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
using System.Data;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions;
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class DbCommandBuilderTest : StandardMappingTest
  {
    private ISqlDialect _sqlDialectStub;
    private IValueConverter _valueConverterStub;
    private TestableDbCommandBuilder _commandBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _valueConverterStub = MockRepository.GenerateStub<IValueConverter> ();
      _commandBuilder = new TestableDbCommandBuilder (_sqlDialectStub, _valueConverterStub);
    }

    [Test]
    public void AddCommandParameter_ForExtensibleEnum ()
    {
      var commandStub = MockRepository.GenerateStub<IDbCommand> ();
      var parameterMock = MockRepository.GenerateMock<IDbDataParameter> ();
      var parameterCollectionStub = MockRepository.GenerateStub<IDataParameterCollection> ();

      commandStub.Stub (stub => stub.CreateParameter ()).Return (parameterMock);
      commandStub.Stub (stub => stub.Parameters).Return (parameterCollectionStub);
      commandStub.Replay ();

      parameterCollectionStub.Stub (stub => stub.Add (parameterMock)).Return (0);
      parameterCollectionStub.Replay ();

      _sqlDialectStub.Stub (stub => stub.GetParameterName ("x")).Return ("x");
      _valueConverterStub.Stub (stub => stub.GetDBValue (Color.Values.Red ())).Return ("VALUE");

      parameterMock.Expect (mock => mock.Value = "VALUE"); // string!
      parameterMock.Replay ();
      
      _commandBuilder.AddCommandParameter (commandStub, "x", Color.Values.Red ());

      parameterMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetOrderClause ()
    {
      var sortExpressionDefinition = SortExpressionDefinitionObjectMother.CreateOrderItemSortExpressionPositionAscProductDesc ();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Position")).Return ("<Position>");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Product")).Return ("<Product>");

      var result = _commandBuilder.GetOrderClause (sortExpressionDefinition);

      Assert.That (result, Is.EqualTo (" ORDER BY <Position> ASC, <Product> DESC"));
    }

    [Test]
    public void GetOrderClause_Null ()
    {
      var result = _commandBuilder.GetOrderClause (null);

      Assert.That (result, Is.EqualTo (string.Empty));
    }

    [Test]
    public void AppendWhereClause ()
    {
      var statement = new StringBuilder();
      var command = MockRepository.GenerateStub<IDbCommand>();
      
      var specificationMock = MockRepository.GenerateStrictMock<IComparedColumnsSpecification>();
      specificationMock
          .Expect (mock => mock.AppendComparisons (statement, command, _sqlDialectStub))
          .WhenCalled (
              mi =>
              {
                Assert.That (statement.ToString(), Is.EqualTo (" WHERE "));
                statement.Append ("<conditions>");
              });
      specificationMock.Replay();

      _commandBuilder.AppendWhereClause (statement, specificationMock, command);

      specificationMock.VerifyAllExpectations();
    }

    [Test]
    public void AppendUpdateClause()
    {
      var statement = new StringBuilder();
      var command = MockRepository.GenerateStub<IDbCommand>();

      var specificationMock = MockRepository.GenerateStrictMock<IUpdatedColumnsSpecification>();
      specificationMock
          .Expect (mock => mock.AppendColumnValueAssignments(statement, command, _sqlDialectStub))
          .WhenCalled (
              mi =>
              {
                Assert.That (statement.ToString(), Is.EqualTo(" SET "));
                statement.Append ("updates...");
              });
      specificationMock.Replay();

      _commandBuilder.AppendUpdateClause (statement, specificationMock, command);

      specificationMock.VerifyAllExpectations ();
      Assert.That (statement.ToString(), Is.EqualTo (" SET updates..."));
    }

    [Test]
    public void AppendOrderByClause ()
    {
      var statement = new StringBuilder ();

      var specificationMock = MockRepository.GenerateStrictMock<IOrderedColumnsSpecification> ();
      specificationMock.Expect (mock => mock.IsEmpty).Return (false);
      specificationMock
          .Expect (mock => mock.AppendOrderings(statement, _sqlDialectStub))
          .WhenCalled (
              mi =>
              {
                Assert.That (statement.ToString (), Is.EqualTo(" ORDER BY "));
                statement.Append ("orders...");
              });
      specificationMock.Replay ();

      _commandBuilder.AppendOrderByClause (statement, specificationMock);

      specificationMock.VerifyAllExpectations();
      Assert.That (statement.ToString (), Is.EqualTo(" ORDER BY orders..."));
    }
    
  }
}