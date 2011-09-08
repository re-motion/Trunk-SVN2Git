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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Text;
using Rhino.Mocks;
using System.Linq;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderGeneratedSqlTest : StandardMappingTest
  {
    private ObservableRdbmsProvider.ICommandExecutionListener _executionListenerStrictMock;
    private RdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();

      _executionListenerStrictMock = MockRepository.GenerateStrictMock<ObservableRdbmsProvider.ICommandExecutionListener>();
      _provider = RdbmsProviderObjectMother.CreateForIntegrationTest (
          TestDomainStorageProviderDefinition,
          new SqlStorageTypeInformationProvider(),
          new SqlDbCommandBuilderFactory (SqlDialect.Instance),
          SqlDialect.Instance,
          (providerDefinition, nameProvider, dialect, persistenceListener, commandFactory) =>
          new ObservableRdbmsProvider (
              providerDefinition,
              nameProvider,
              dialect,
              NullPersistenceListener.Instance,
              commandFactory,
              () => new SqlConnection(),
              _executionListenerStrictMock));
    }

    public override void TearDown ()
    {
      _provider.Dispose();
      base.TearDown();
    }

    [Test]
    public void LoadDataContainer ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleRow,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] WHERE [ID] = @ID;",
          Tuple.Create ("@ID", DbType.Guid, DomainObjectIDs.Computer1.Value));
      _executionListenerStrictMock.Replay();

      _provider.LoadDataContainer (DomainObjectIDs.Computer1);

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void LoadDataContainers_SingleID ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] WHERE [ID] = @ID;",
          Tuple.Create ("@ID", DbType.Guid, DomainObjectIDs.Computer1.Value));
      _executionListenerStrictMock.Replay();

      _provider.LoadDataContainers (new[] { DomainObjectIDs.Computer1 }).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void LoadDataContainers_MultiIDs_SameTable ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>c7c26bf5-871d-48c7-822a-e9b05aac4e5a</I><I>176a0ff6-296d-4934-bd1a-23cf52c22411</I></L>"));
      _executionListenerStrictMock.Replay();

      _provider.LoadDataContainers (new[] { DomainObjectIDs.Computer1, DomainObjectIDs.Computer2 }).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void LoadDataContainers_MultiIDs_DifferentTables ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [SerialNumber], [EmployeeID] FROM [Computer] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>c7c26bf5-871d-48c7-822a-e9b05aac4e5a</I><I>176a0ff6-296d-4934-bd1a-23cf52c22411</I></L>"));
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [SupervisorID] FROM [Employee] "
          + "WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));
      _executionListenerStrictMock.Replay();

      _provider.LoadDataContainers (
          new[] { DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Employee1, DomainObjectIDs.Employee2 }).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void LoadDataContainersByRelatedID_NoSortExpression ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality] "
          + "FROM [Company] WHERE [IndustrialSectorID] = @IndustrialSectorID;",
          Tuple.Create ("@IndustrialSectorID", DbType.Guid, DomainObjectIDs.IndustrialSector1.Value));
      _executionListenerStrictMock.Replay();

      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Company), "IndustrialSector");
      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, DomainObjectIDs.IndustrialSector1).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithSortExpression ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp], [Name], [IndustrialSectorID], [CustomerSince], [CustomerType], [ContactPersonID], [NumberOfShops], [SupplierQuality] "
          + "FROM [Company] WHERE [IndustrialSectorID] = @IndustrialSectorID ORDER BY [CustomerSince] DESC, [Name] ASC;",
          Tuple.Create ("@IndustrialSectorID", DbType.Guid, DomainObjectIDs.IndustrialSector1.Value));
      _executionListenerStrictMock.Replay();

      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Company), "IndustrialSector");
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              new SortedPropertySpecification (GetPropertyDefinition (typeof (Customer), "CustomerSince"), SortOrder.Descending),
              new SortedPropertySpecification (GetPropertyDefinition (typeof (Company), "Name"), SortOrder.Ascending)
          });
      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, DomainObjectIDs.IndustrialSector1).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void ExecuteCollectionQuery ()
    {
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT * FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
          Tuple.Create ("@p1", DbType.Int32, (object) 1),
          Tuple.Create ("@p2", DbType.Guid, DomainObjectIDs.Order2.Value),
          Tuple.Create ("@p3", DbType.String, (object) DomainObjectIDs.Official1.ToString()),
          Tuple.Create ("@p4", DbType.String, (object) DBNull.Value)
          );
      _executionListenerStrictMock.Replay();

      var query =
          new Query (
              new QueryDefinition (
                  "id",
                  TestDomainStorageProviderDefinition,
                  "SELECT * FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
                  QueryType.Collection),
              new QueryParameterCollection
              {
                  { "@p1", 1 }, 
                  { "@p2", DomainObjectIDs.Order2 }, 
                  { "@p3", DomainObjectIDs.Official1 },
                  { "@p4", null }
              });
      _provider.ExecuteCollectionQuery (query).ToArray();

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      ExpectExecuteScalar (
          "SELECT COUNT(*) FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
          Tuple.Create ("@p1", DbType.Int32, (object) 1),
          Tuple.Create ("@p2", DbType.Guid, DomainObjectIDs.Order2.Value),
          Tuple.Create ("@p3", DbType.String, (object) DomainObjectIDs.Official1.ToString()),
          Tuple.Create ("@p4", DbType.String, (object) DBNull.Value)
          );
      _executionListenerStrictMock.Replay();

      var query =
          new Query (
              new QueryDefinition (
                  "id",
                  TestDomainStorageProviderDefinition,
                  "SELECT COUNT(*) FROM [Order] WHERE OrderNo=@p1 OR ID=@p2 OR OfficialID=@p3 OR OfficialID=@p4",
                  QueryType.Scalar),
              new QueryParameterCollection
              {
                  { "@p1", 1 }, 
                  { "@p2", DomainObjectIDs.Order2 }, 
                  { "@p3", DomainObjectIDs.Official1 },
                  { "@p4", null }
              });
      _provider.ExecuteScalarQuery (query);

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void Save ()
    {
      SetDatabaseModifyable();

      var newGuid = new Guid ("322D1DCB-19E4-49BA-90AB-7F5C9C8126E8");
      var newDataContainer = DataContainer.CreateNew (new ObjectID (Configuration.GetTypeDefinition (typeof (Employee)), newGuid));
      var changedDataContainer = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee1);
      changedDataContainer[GetPropertyIdentifier (typeof (Employee), "Name")] = "George";
      var markedAsChangedDataContainer = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee2);
      markedAsChangedDataContainer.MarkAsChanged();
      var unchangedDataContainer = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee3);
      var deletedDataContainer = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee7);
      deletedDataContainer[GetPropertyIdentifier (typeof (Employee), "Supervisor")] = null;
      deletedDataContainer.Delete();

      ExpectExecuteNonQuery (
          "INSERT INTO [Employee] ([ID], [ClassID], [Name]) VALUES (@ID, @ClassID, @Name);",
          Tuple.Create ("@ID", DbType.Guid, newDataContainer.ID.Value),
          Tuple.Create ("@ClassID", DbType.String, (object) "Employee"),
          Tuple.Create ("@Name", DbType.String, (object) ""));
      ExpectExecuteNonQuery (
          "UPDATE [Employee] SET [Name] = @Name WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create ("@Name", DbType.String, (object) "George"),
          Tuple.Create ("@ID", DbType.Guid, changedDataContainer.ID.Value),
          Tuple.Create ("@Timestamp", DbType.Binary, changedDataContainer.Timestamp)
          );
      ExpectExecuteNonQuery (
          "UPDATE [Employee] SET [SupervisorID] = @SupervisorID WHERE [ID] = @ID;",
          Tuple.Create ("@SupervisorID", DbType.Guid, (object) DBNull.Value),
          Tuple.Create ("@ID", DbType.Guid, newDataContainer.ID.Value));
      ExpectExecuteNonQuery (
          "UPDATE [Employee] SET [SupervisorID] = @SupervisorID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create ("@SupervisorID", DbType.Guid, (object) DBNull.Value),
          Tuple.Create ("@ID", DbType.Guid, deletedDataContainer.ID.Value),
          Tuple.Create ("@Timestamp", DbType.Binary, deletedDataContainer.Timestamp));
      ExpectExecuteNonQuery (
          "UPDATE [Employee] SET [ClassID] = @ClassID WHERE [ID] = @ID AND [Timestamp] = @Timestamp;",
          Tuple.Create ("@ClassID", DbType.String, (object) "Employee"),
          Tuple.Create ("@ID", DbType.Guid, markedAsChangedDataContainer.ID.Value),
          Tuple.Create ("@Timestamp", DbType.Binary, markedAsChangedDataContainer.Timestamp));
      ExpectExecuteNonQuery (
          "DELETE FROM [Employee] WHERE [ID] = @ID;",
          Tuple.Create ("@ID", DbType.Guid, deletedDataContainer.ID.Value));
      _executionListenerStrictMock.Replay();
      
      _provider.Save (new[] { changedDataContainer, newDataContainer, deletedDataContainer, markedAsChangedDataContainer, unchangedDataContainer});

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void UpdateTimestamps_SingleID ()
    {
      SetDatabaseModifyable();

      var dataContainer = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee1);

      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] = @ID;",
          Tuple.Create ("@ID", DbType.Guid, dataContainer.ID.Value));

      _executionListenerStrictMock.Replay();
      
      _provider.UpdateTimestamps (new[] { dataContainer });

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void UpdateTimestamps_MultipleIDs_SameTable ()
    {
      SetDatabaseModifyable();

      var dataContainer1 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee1);
      var dataContainer2 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee2);

      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));

      _executionListenerStrictMock.Replay();
      
      _provider.UpdateTimestamps (new[] { dataContainer1, dataContainer2 });

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void UpdateTimestamps_MultipleIDs_MultipleTables ()
    {
      SetDatabaseModifyable();

      var dataContainer1 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee1);
      var dataContainer2 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Employee2);
      var dataContainer3 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Customer1);
      var dataContainer4 = LoadDataContainerInSeparateProvider (DomainObjectIDs.Partner1);

      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Employee] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>51ece39b-f040-45b0-8b72-ad8b45353990</I><I>c3b2bbc3-e083-4974-bac7-9cee1fb85a5e</I></L>"));
      ExpectExecuteReader (
          CommandBehavior.SingleResult,
          "SELECT [ID], [ClassID], [Timestamp] FROM [Company] WHERE [ID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @ID.nodes('/L/I') T(c));",
          Tuple.Create ("@ID", DbType.Xml, (object) "<L><I>55b52e75-514b-4e82-a91b-8f0bb59b80ad</I><I>5587a9c0-be53-477d-8c0a-4803c7fae1a9</I></L>"));

      _executionListenerStrictMock.Replay();
      
      _provider.UpdateTimestamps (new[] { dataContainer1, dataContainer2, dataContainer3, dataContainer4 });

      _executionListenerStrictMock.VerifyAllExpectations();
    }

    private void ExpectExecuteReader (
        CommandBehavior expectedCommandBehavior,
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .Expect (mock => mock.OnExecuteReader (Arg<IDbCommand>.Is.Anything, Arg.Is (expectedCommandBehavior)))
          .WhenCalled (mi => CheckCommand ((IDbCommand) mi.Arguments[0], expectedSql, expectedParametersData))
          .Repeat.Once();
    }

    private void ExpectExecuteScalar (
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .Expect (mock => mock.OnExecuteScalar (Arg<IDbCommand>.Is.Anything))
          .WhenCalled (mi => CheckCommand ((IDbCommand) mi.Arguments[0], expectedSql, expectedParametersData))
          .Repeat.Once();
    }

    private void ExpectExecuteNonQuery (
        string expectedSql,
        params Tuple<string, DbType, object>[] expectedParametersData)
    {
      _executionListenerStrictMock
          .Expect (mock => mock.OnExecuteNonQuery (Arg<IDbCommand>.Is.Anything))
          .WhenCalled (mi => CheckCommand ((IDbCommand) mi.Arguments[0], expectedSql, expectedParametersData))
          .Repeat.Once();
    }

    private void CheckCommand (IDbCommand sqlCommand, string expectedSql, params Tuple<string, DbType, object>[] expectedParametersData)
    {
      try
      {
        Assert.That (
            sqlCommand.CommandText,
            Is.EqualTo (expectedSql),
            "Command text doesn't match.\r\nActual statement: {0}\r\nExpected statement: {1})",
            sqlCommand.CommandText,
            expectedSql);
        Assert.That (
            sqlCommand.CommandType,
            Is.EqualTo (CommandType.Text),
            "Command type doesn't match.\r\nExpected statement: {0})");
        Assert.That (
            sqlCommand.Parameters.Count,
            Is.EqualTo (expectedParametersData.Length),
            "Number of parameters doesn't match.\r\nStatement: {0})",
            expectedSql);
        for (int i = 0; i < expectedParametersData.Length; ++i)
        {
          var actualParameter = (IDataParameter) sqlCommand.Parameters[i];
          var expectedParameterData = expectedParametersData[i];

          Assert.That (
              actualParameter.ParameterName,
              Is.EqualTo (expectedParameterData.Item1),
              "Name of parameter " + i + " doesn't match.\r\nStatement: {0})",
              expectedSql);
          Assert.That (
              actualParameter.DbType,
              Is.EqualTo (expectedParameterData.Item2),
              "DbType of parameter " + i + " doesn't match.\r\nSstatement: {0})",
              expectedSql);
          Assert.That (
              actualParameter.Value,
              Is.EqualTo (expectedParameterData.Item3),
              "Value of parameter " + i + " doesn't match.\r\nStatement: {0})",
              expectedSql);
        }
      }
      catch (AssertionException)
      {
        Console.WriteLine (sqlCommand.CommandText);
        Console.WriteLine (sqlCommand.CommandType);
        Console.WriteLine (SeparatedStringBuilder.Build (
          "," + Environment.NewLine,
          sqlCommand.Parameters.Cast<IDataParameter>(),
          parameter =>
          {
            string valueString;
            if (parameter.Value == DBNull.Value)
              valueString = "DBNull.Value";
            else if (parameter.Value is string)
              valueString = "\"" + parameter.Value + "\"";
            else if (parameter.Value == null)
              valueString = "null";
            else
              valueString = parameter.Value.ToString();

            return string.Format ("Tuple.Create (\"{0}\", DbType.{1}, (object) {2})", parameter.ParameterName, parameter.DbType, valueString);
          }));

        throw;
      }
    }

    private DataContainer LoadDataContainerInSeparateProvider (ObjectID objectID)
    {
      using (var provider = RdbmsProviderObjectMother.CreateForIntegrationTest (
          TestDomainStorageProviderDefinition,
          new SqlStorageTypeInformationProvider (),
          new SqlDbCommandBuilderFactory (SqlDialect.Instance),
          SqlDialect.Instance,
          (providerDefinition, nameProvider, dialect, persistenceListener, commandFactory) =>
          new RdbmsProvider (
              providerDefinition,
              nameProvider,
              dialect,
              NullPersistenceListener.Instance,
              commandFactory,
              () => new SqlConnection ())))
      {
        return provider.LoadDataContainer (objectID).LocatedObject;
      }

    }
  }
}