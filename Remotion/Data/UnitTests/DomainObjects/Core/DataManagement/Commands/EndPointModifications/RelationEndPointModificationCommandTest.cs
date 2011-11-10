// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointModificationCommandTest : StandardMappingTest
  {
    private ClientTransactionMock _transaction;
    private ObjectID _objectID;
    
    private IRelationEndPoint _endPointMock;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;
    private RelationEndPointModificationCommand _command;
    private RelationEndPointModificationCommand _commandPartialMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransactionMock ();
      _objectID = DomainObjectIDs.Order1;

      _endPointMock = MockRepository.GenerateMock<IRelationEndPoint> ();
      _endPointMock.Stub (mock => mock.ClientTransaction).Return (_transaction);
      _endPointMock.Stub (mock => mock.ObjectID).Return (_objectID);

      _oldRelatedObject = _transaction.Execute (() => OrderTicket.NewObject ());
      _newRelatedObject = _transaction.Execute (() => OrderTicket.NewObject ());

      _command = CreateTestableCommand ();
      _commandPartialMock = CreateCommandPartialMock ();
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions (), Is.Empty);
    }
    
    [Test]
    public void NotifyClientTransactionOfBegin_SetsCurrentTransaction ()
    {
      _commandPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedNotifyClientTransactionOfBegin"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      _commandPartialMock.Replay ();

      _commandPartialMock.NotifyClientTransactionOfBegin ();

      _commandPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_SetsCurrentTransaction ()
    {
      _commandPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedNotifyClientTransactionOfEnd"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      _commandPartialMock.Replay ();

      _commandPartialMock.NotifyClientTransactionOfEnd ();

      _commandPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void Begin_SetsCurrentTransaction ()
    {
      _commandPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedBegin"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      _commandPartialMock.Replay ();

      _commandPartialMock.Begin ();

      _commandPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void End_SetsCurrentTransaction ()
    {
      _commandPartialMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedEnd"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      _commandPartialMock.Replay ();

      _commandPartialMock.End ();

      _commandPartialMock.VerifyAllExpectations ();
    }

    private RelationEndPointModificationCommand CreateCommandPartialMock ()
    {
      return MockRepository.GeneratePartialMock<RelationEndPointModificationCommand> (_endPointMock, _oldRelatedObject, _newRelatedObject);
    }

    private RelationEndPointModificationCommand CreateTestableCommand ()
    {
      return new TestableRelationEndPointModificationCommand (_endPointMock, _oldRelatedObject, _newRelatedObject);
    }
  }
}