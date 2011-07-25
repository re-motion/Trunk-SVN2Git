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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderTest : SqlProviderBaseTest
  {
    private RdbmsProviderDefinition _definition;

    private MockRepository _mockRepository;

    private ISqlDialect _dialectStub;
    private IStorageNameProvider _storageNameProviderStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _commandFactoryMock;
    private IDbConnection _connectionStub;

    private TestableRdbmsProvider _providerWithSqlConnection;

    private TestableRdbmsProvider.IConnectionCreator _connectionCreatorMock;
    private TestableRdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = TestDomainStorageProviderDefinition;

      _mockRepository = new MockRepository();

      _dialectStub = _mockRepository.Stub<ISqlDialect>();

      _storageNameProviderStub = _mockRepository.Stub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.IDColumnName).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.ClassIDColumnName).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.TimestampColumnName).Return ("Timestamp");
      _storageNameProviderStub.Replay();

      _commandFactoryMock = _mockRepository.StrictMock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _connectionStub = _mockRepository.Stub<IDbConnection>();

      var sqlConnectionCreatorStub = _mockRepository.Stub<TestableRdbmsProvider.IConnectionCreator>();
      sqlConnectionCreatorStub.Stub (stub => stub.CreateConnection()).Return (new SqlConnection());
      sqlConnectionCreatorStub.Replay();

      _providerWithSqlConnection = new TestableRdbmsProvider (
          _definition,
          _storageNameProviderStub,
          _dialectStub,
          NullPersistenceListener.Instance,
          _commandFactoryMock,
          sqlConnectionCreatorStub);

      _connectionCreatorMock = _mockRepository.StrictMock<TestableRdbmsProvider.IConnectionCreator>();

      _provider = new TestableRdbmsProvider (
          _definition,
          _storageNameProviderStub,
          _dialectStub,
          NullPersistenceListener.Instance,
          _commandFactoryMock,
          _connectionCreatorMock);
    }

    public override void TearDown ()
    {
      _providerWithSqlConnection.Dispose();
      base.TearDown();
    }

    [Test]
    public void CreateDbCommand_CreatesCommand ()
    {
      _providerWithSqlConnection.Connect();
      _providerWithSqlConnection.BeginTransaction();

      using (var command = _providerWithSqlConnection.CreateDbCommand())
      {
        Assert.That (command.WrappedInstance.Connection, Is.SameAs (_providerWithSqlConnection.Connection.WrappedInstance));
        Assert.That (command.WrappedInstance.Transaction, Is.SameAs (_providerWithSqlConnection.Transaction.WrappedInstance));
      }
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenCreateDbCommandThrows_InSetConnection ()
    {
      var commandMock = MockRepository.GenerateMock<IDbCommand>();
      commandMock.Expect (mock => mock.Connection = Arg<IDbConnection>.Is.Anything).WhenCalled (mi => { throw new ApplicationException ("Test"); });
      commandMock.Expect (mock => mock.Dispose());
      commandMock.Replay();

      var connectionStub = MockRepository.GenerateStub<IDbConnection>();
      connectionStub.Stub (stub => stub.CreateCommand()).Return (commandMock);
      connectionStub.Stub (stub => stub.State).Return (ConnectionState.Open);
      connectionStub.Replay();

      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var providerPartialMock = MockRepository.GeneratePartialMock<RdbmsProvider> (
          _definition, storageNameProvider, SqlDialect.Instance, NullPersistenceListener.Instance, CommandFactory);
      providerPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, typeof (RdbmsProvider), "CreateConnection"))
          .Return (new TracingDbConnection (connectionStub, NullPersistenceListener.Instance));
      providerPartialMock.Replay();

      providerPartialMock.Connect();
      try
      {
        providerPartialMock.CreateDbCommand();
        Assert.Fail ("Expected ApplicationException");
      }
      catch (ApplicationException)
      {
        // ok
      }

      providerPartialMock.VerifyAllExpectations();
      commandMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenCreateDbCommandThrows_InSetTransaction ()
    {
      var commandMock = MockRepository.GenerateMock<IDbCommand>();
      commandMock.Expect (mock => mock.Connection = Arg<IDbConnection>.Is.Anything);
      commandMock.Expect (mock => mock.Transaction = Arg<IDbTransaction>.Is.Anything).WhenCalled (mi => { throw new ApplicationException ("Test"); });
      commandMock.Expect (mock => mock.Dispose());
      commandMock.Replay();

      var connectionStub = MockRepository.GenerateStub<IDbConnection>();
      connectionStub.Stub (stub => stub.CreateCommand()).Return (commandMock);
      connectionStub.Stub (stub => stub.State).Return (ConnectionState.Open);
      connectionStub.Replay();

      var storageNameProvider = new ReflectionBasedStorageNameProvider();
      var providerPartialMock = MockRepository.GeneratePartialMock<RdbmsProvider> (
          _definition, storageNameProvider, SqlDialect.Instance, NullPersistenceListener.Instance, CommandFactory);
      providerPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, typeof (RdbmsProvider), "CreateConnection"))
          .Return (new TracingDbConnection (connectionStub, NullPersistenceListener.Instance));
      providerPartialMock.Replay();

      providerPartialMock.Connect();
      try
      {
        providerPartialMock.CreateDbCommand();
        Assert.Fail ("Expected ApplicationException");
      }
      catch (ApplicationException)
      {
        // ok
      }

      providerPartialMock.VerifyAllExpectations();
      commandMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Connect must be called before a command can be created.")]
    public void CreateDbCommand_NotConnected ()
    {
      _providerWithSqlConnection.CreateDbCommand();
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void CreateDbCommand_ChecksDisposed ()
    {
      _providerWithSqlConnection.Dispose();
      _providerWithSqlConnection.CreateDbCommand();
    }

    [Test]
    public void SetTimestamp ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order2);
      var timestamp1 = new object();
      var timestamp2 = new object();

      var storageProviderCommandStub =
          MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>>();
      storageProviderCommandStub
          .Stub (stub => stub.Execute (_providerWithSqlConnection))
          .Return (
              new[]
              {
                  new ObjectLookupResult<object> (DomainObjectIDs.Order1, timestamp1),
                  new ObjectLookupResult<object> (DomainObjectIDs.Order2, timestamp2)
              });

      _commandFactoryMock
          .Expect (
              mock =>
              mock.CreateForMultiTimestampLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 })))
          .Return (storageProviderCommandStub);
      _commandFactoryMock.Replay();

      _providerWithSqlConnection.SetTimestamp (new DataContainerCollection (new[] { dataContainer1, dataContainer2 }, true));

      _commandFactoryMock.VerifyAllExpectations();
      Assert.That (dataContainer1.Timestamp, Is.SameAs (timestamp1));
      Assert.That (dataContainer2.Timestamp, Is.SameAs (timestamp2));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "No timestamp found for object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void SetTimestamp_ObjectIDCannotBeFound ()
    {
      var timestamp = new object();
      var storageProviderCommandStub =
          MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext>>();
      storageProviderCommandStub
          .Stub (stub => stub.Execute (_providerWithSqlConnection))
          .Return (new[] { new ObjectLookupResult<object> (DomainObjectIDs.Order2, timestamp) });

      _commandFactoryMock
          .Expect (mock => mock.CreateForMultiTimestampLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1 })))
          .Return (storageProviderCommandStub);
      _commandFactoryMock.Replay();

      _providerWithSqlConnection.SetTimestamp (new DataContainerCollection (new[] { DataContainer.CreateNew (DomainObjectIDs.Order1) }, true));
    }

    [Test]
    public void StatementDelimiter_UsesDialect ()
    {
      _dialectStub.Stub (stub => stub.StatementDelimiter).Return ("&");
      _dialectStub.Replay();

      var result = _providerWithSqlConnection.StatementDelimiter;
      Assert.That (result, Is.EqualTo ("&"));
    }

    [Test]
    public void DelimitIdentifier_UsesDialect ()
    {
      _dialectStub.Stub (stub => stub.DelimitIdentifier ("xy")).Return ("!xy!");
      _dialectStub.Replay();

      var result = _providerWithSqlConnection.DelimitIdentifier ("xy");
      Assert.That (result, Is.EqualTo ("!xy!"));
    }

    [Test]
    public void GetParameterName_UsesDialect ()
    {
      _dialectStub.Stub (stub => stub.GetParameterName ("xy")).Return ("#1");
      _dialectStub.Replay();

      var result = _providerWithSqlConnection.GetParameterName ("xy");
      Assert.That (result, Is.EqualTo ("#1"));
    }

    [Test]
    public void ExecutesCollectionQuery ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order2);

      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { dataContainer1, dataContainer2 });
      }
      _mockRepository.ReplayAll();

      var result = _provider.ExecuteCollectionQuery (queryStub);

      Assert.That (result, Is.EqualTo (new[] { dataContainer1, dataContainer2 }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "A database query returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed.")]
    public void ExecutesCollectionQuery_DuplicatedIDs ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order1);

      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { dataContainer1, dataContainer2 });
      }
      _mockRepository.ReplayAll();

      _provider.ExecuteCollectionQuery (queryStub);
    }

    [Test]
    public void ExecutesCollectionQuery_DuplicatedNullValues ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery>();
      queryStub.Stub (stub => stub.StorageProviderDefinition).Return (TestDomainStorageProviderDefinition);
      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();

      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForDataContainerQuery (queryStub))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new DataContainer[] { null, null });
      }
      _mockRepository.ReplayAll();

      var result = _provider.ExecuteCollectionQuery (queryStub);

      Assert.That (result, Is.EqualTo (new DataContainer[] { null, null }));
    }

    [Test]
    public void LoadDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var fakeResult = new ObjectLookupResult<DataContainer> (objectID, DataContainer.CreateNew (objectID));

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSingleIDLookup (objectID))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (fakeResult);
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainer (objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    [Test]
    public void LoadDataContainer_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      _mockRepository.ReplayAll();

      Assert.That (
          () => _provider.LoadDataContainer (objectID),
          Throws.ArgumentException.With.Message.EqualTo (
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'TestDomain'.\r\nParameter name: id"));
    }

    [Test]
    public void LoadDataContainer_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainer (objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order2;
      var objectID3 = DomainObjectIDs.Order3;

      var lookupResult1 = new ObjectLookupResult<DataContainer> (objectID1, DataContainer.CreateNew (objectID1));
      var lookupResult2 = new ObjectLookupResult<DataContainer> (objectID2, DataContainer.CreateNew (objectID2));
      var lookupResult3 = new ObjectLookupResult<DataContainer> (objectID3, DataContainer.CreateNew (objectID3));

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSortedMultiIDLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { objectID1, objectID2, objectID3 })))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { lookupResult1, lookupResult2, lookupResult3 });
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainers (new[] { objectID1, objectID2, objectID3 }).ToArray();

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { lookupResult1, lookupResult2, lookupResult3 }));
    }

    [Test]
    public void LoadDataContainers_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainers (new[] { objectID }),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainers_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext>>();
      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _commandFactoryMock
          .Expect (mock => mock.CreateForSortedMultiIDLookup (Arg<IEnumerable<ObjectID>>.List.Equal (new[] { objectID })))
          .Return (commandMock);
      _mockRepository.ReplayAll();

      Assert.That (
          () => _provider.LoadDataContainers (new[] { objectID }),
          Throws.ArgumentException.With.Message.EqualTo (
              "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
              + "StorageProvider's ID 'TestDomain'.\r\nParameter name: ids"));
    }

    [Test]
    public void LoadDataContainersByRelatedID ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult });
      }
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { fakeResult }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = "A relation lookup returned a NULL ID, which is not allowed.")]
    public void LoadDataContainersByRelatedID_NullContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock =
          _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult, null });
      }
      _mockRepository.ReplayAll();

      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage =
        "A relation lookup returned duplicates of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', which is not allowed.")]
    public void LoadDataContainersByRelatedID_Duplicates ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      var sortExpression = new SortExpressionDefinition (
          new[] { new SortedPropertySpecification (GetPropertyDefinition (typeof (Official), "Name"), SortOrder.Ascending) });
      var fakeResult = DataContainer.CreateNew (objectID);

      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _commandFactoryMock
          .Expect (mock => mock.CreateForRelationLookup (relationEndPointDefinition, objectID, sortExpression))
          .Return (commandMock);
      commandMock.Expect (mock => mock.Execute (_provider)).Return (new[] { fakeResult, fakeResult });

      _mockRepository.ReplayAll();

      _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, sortExpression, objectID);
    }

    [Test]
    public void LoadDataContainersByRelatedID_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }

    [Test]
    public void LoadDataContainersByRelatedID_ClassDefinitionWithDifferentStorageProviderDefinition ()
    {
      var providerWithDifferentStorageProvider = new SqlProvider (
          new RdbmsProviderDefinition ("Test", new SqlStorageObjectFactory(), TestDomainConnectionString),
          StorageNameProvider,
          NullPersistenceListener.Instance,
          CommandFactory);
      var objectID = DomainObjectIDs.Order1;
      var relationEndPointDefinition = (RelationEndPointDefinition) GetEndPointDefinition (typeof (Order), "Official");

      _mockRepository.ReplayAll();

      Assert.That (
          () => providerWithDifferentStorageProvider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID),
          Throws.Exception.TypeOf<ArgumentException>().With.Message.EqualTo (
              "The StorageProviderID 'TestDomain' of the provided ClassDefinition does not match with this StorageProvider's ID 'Test'.\r\nParameter name: classDefinition"));
    }

    [Test]
    public void LoadDataContainersByRelatedID_StorageClassTransaction ()
    {
      var objectID = DomainObjectIDs.Order1;
      var propertyDefinition = PropertyDefinitionFactory.Create (
          objectID.ClassDefinition, StorageClass.Transaction, typeof (Order).GetProperty ("Official"));
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, true);

      _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
      _mockRepository.ReplayAll();

      var result = _provider.LoadDataContainersByRelatedID (relationEndPointDefinition, null, objectID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.Empty);
    }

    [Test]
    public void Save ()
    {
      var dataContainer1 = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var dataContainer2 = DataContainer.CreateNew (DomainObjectIDs.Order2);

      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<IRdbmsProviderCommandExecutionContext>>();
      using (_mockRepository.Ordered())
      {
        _connectionCreatorMock.Expect (mock => mock.CreateConnection()).Return (_connectionStub);
        _commandFactoryMock
            .Expect (mock => mock.CreateForSave (Arg<DataContainer[]>.Is.Equal (new[] { dataContainer1, dataContainer2 })))
            .Return (commandMock);
        commandMock.Expect (mock => mock.Execute (_provider));
      }
      _mockRepository.ReplayAll();

      _provider.Save (new DataContainerCollection (new[] { dataContainer1, dataContainer2 }, true));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Save_Disposed ()
    {
      _mockRepository.ReplayAll();

      _provider.Dispose();

      Assert.That (
          () => _provider.Save (new DataContainerCollection()),
          Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
              "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }
  }
}