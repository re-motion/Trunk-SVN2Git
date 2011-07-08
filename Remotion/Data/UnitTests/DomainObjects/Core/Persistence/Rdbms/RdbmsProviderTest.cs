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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

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

      _commandFactoryMock = _mockRepository.StrictMock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>> ();

      _connectionStub = _mockRepository.Stub<IDbConnection> ();

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

      _connectionCreatorMock = _mockRepository.StrictMock<TestableRdbmsProvider.IConnectionCreator> ();

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
      base.TearDown ();
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
    public void StatementDelimiter_UsesDialect ()
    {
      _dialectStub.Stub (stub => stub.StatementDelimiter).Return ("&");
      _dialectStub.Replay ();

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
      _dialectStub.Replay ();

      var result = _providerWithSqlConnection.GetParameterName ("xy");
      Assert.That (result, Is.EqualTo ("#1"));
    }

    [Test]
    public void LoadDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var fakeResult = new DataContainerLookupResult(objectID, DataContainer.CreateNew (objectID));

      var commandMock = _mockRepository.StrictMock<IStorageProviderCommand<DataContainerLookupResult, IRdbmsProviderCommandExecutionContext>> ();
      using (_mockRepository.Ordered ())
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
      Assert.That (result.LocatedDataContainer, Is.SameAs (fakeResult.LocatedDataContainer));
    }

    [Test]
    public void LoadDataContainer_InvalidID ()
    {
      var objectID = DomainObjectIDs.Official1;
      _mockRepository.ReplayAll ();

      Assert.That (() => _provider.LoadDataContainer (objectID), Throws.ArgumentException.With.Message.EqualTo (
          "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this "
           + "StorageProvider's ID 'TestDomain'.\r\nParameter name: id"));
    }

    [Test]
    public void LoadDataContainer_Disposed ()
    {
      var objectID = DomainObjectIDs.Order1;
      _mockRepository.ReplayAll ();

      _provider.Dispose();

      Assert.That (() => _provider.LoadDataContainer (objectID), Throws.Exception.TypeOf<ObjectDisposedException>().With.Message.EqualTo (
          "A disposed StorageProvider cannot be accessed.\r\nObject name: 'StorageProvider'."));
    }
  }
}