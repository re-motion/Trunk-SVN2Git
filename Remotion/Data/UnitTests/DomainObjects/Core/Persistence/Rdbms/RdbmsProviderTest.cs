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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderTest : StandardMappingTest
  {
    private TestRdbmsProvider _provider;
    private RdbmsProviderDefinition _definition;

    public override void SetUp ()
    {
      base.SetUp ();
      _definition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (TestRdbmsProvider), TestDomainConnectionString);
      _provider = new TestRdbmsProvider (_definition, NullPersistenceListener.Instance);
    }

    public override void TearDown ()
    {
      base.TearDown ();
      _provider.Dispose ();
    }

    [Test]
    public void CreateDbCommand_CreatesCommand ()
    {
      _provider.Connect ();
      _provider.BeginTransaction ();

      using (var command = _provider.CreateDbCommand ())
      {
        Assert.That (command.WrappedInstance.Connection, Is.SameAs (_provider.Connection.WrappedInstance));
        Assert.That (command.WrappedInstance.Transaction, Is.SameAs (_provider.Transaction.WrappedInstance));
      }
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenCreateDbCommandThrows_InSetConnection ()
    {
      var commandMock = MockRepository.GenerateMock<IDbCommand> ();
      commandMock.Expect (mock => mock.Connection = Arg<IDbConnection>.Is.Anything).WhenCalled (mi => { throw new ApplicationException ("Test"); });
      commandMock.Expect (mock => mock.Dispose ());
      commandMock.Replay ();

      var connectionStub = MockRepository.GenerateStub<IDbConnection> ();
      connectionStub.Stub (stub => stub.CreateCommand ()).Return (commandMock);
      connectionStub.Stub (stub => stub.State).Return (ConnectionState.Open);
      connectionStub.Replay ();

      var providerPartialMock = MockRepository.GeneratePartialMock<RdbmsProvider> (_definition, NullPersistenceListener.Instance);
      providerPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, typeof (RdbmsProvider), "CreateConnection"))
          .Return (new TracingDbConnection (connectionStub, NullPersistenceListener.Instance));
      providerPartialMock.Replay ();

      providerPartialMock.Connect ();
      try
      {
        providerPartialMock.CreateDbCommand ();
        Assert.Fail ("Expected ApplicationException");
      }
      catch (ApplicationException)
      {
        // ok
      }

      providerPartialMock.VerifyAllExpectations ();
      commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateDbCommand_DisposesCommand_WhenCreateDbCommandThrows_InSetTransaction ()
    {
      var commandMock = MockRepository.GenerateMock<IDbCommand> ();
      commandMock.Expect (mock => mock.Connection = Arg<IDbConnection>.Is.Anything);
      commandMock.Expect (mock => mock.Transaction = Arg<IDbTransaction>.Is.Anything).WhenCalled (mi => { throw new ApplicationException ("Test"); });
      commandMock.Expect (mock => mock.Dispose ());
      commandMock.Replay ();

      var connectionStub = MockRepository.GenerateStub<IDbConnection> ();
      connectionStub.Stub (stub => stub.CreateCommand ()).Return (commandMock);
      connectionStub.Stub (stub => stub.State).Return (ConnectionState.Open);
      connectionStub.Replay ();

      var providerPartialMock = MockRepository.GeneratePartialMock<RdbmsProvider> (_definition, NullPersistenceListener.Instance);
      providerPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, typeof (RdbmsProvider), "CreateConnection"))
          .Return (new TracingDbConnection (connectionStub, NullPersistenceListener.Instance));
      providerPartialMock.Replay ();

      providerPartialMock.Connect ();
      try
      {
        providerPartialMock.CreateDbCommand ();
        Assert.Fail ("Expected ApplicationException");
      }
      catch (ApplicationException)
      {
        // ok
      }

      providerPartialMock.VerifyAllExpectations ();
      commandMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Connect must be called before a command can be created.")]
    public void CreateDbCommand_NotConnected ()
    {
      _provider.CreateDbCommand ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void CreateDbCommand_ChecksDisposed ()
    {
      _provider.Dispose ();
      _provider.CreateDbCommand ();
    }
  }
}