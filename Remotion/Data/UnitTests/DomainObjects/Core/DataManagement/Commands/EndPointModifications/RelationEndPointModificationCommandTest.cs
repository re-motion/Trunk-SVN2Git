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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
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
    
    private IEndPoint _endPointMock;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransactionMock ();
      _objectID = DomainObjectIDs.Order1;

      _endPointMock = MockRepository.GenerateMock<IEndPoint> ();
      _endPointMock.Stub (mock => mock.ClientTransaction).Return (_transaction);
      _endPointMock.Stub (mock => mock.ObjectID).Return (_objectID);

      _oldRelatedObject = _transaction.Execute (() => OrderTicket.NewObject ());
      _newRelatedObject = _transaction.Execute (() => OrderTicket.NewObject ());
    }
    
    [Test]
    public void NotifyClientTransactionOfBegin_SetsCurrentTransaction ()
    {
      var commandMock = CreateCommandPartialMock ();

      commandMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedNotifyClientTransactionOfBegin"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      commandMock.Replay ();

      commandMock.NotifyClientTransactionOfBegin ();

      commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_SetsCurrentTransaction ()
    {
      var commandMock = CreateCommandPartialMock ();

      commandMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedNotifyClientTransactionOfEnd"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      commandMock.Replay ();

      commandMock.NotifyClientTransactionOfEnd ();

      commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void Begin_SetsCurrentTransaction ()
    {
      var commandMock = CreateCommandPartialMock ();

      commandMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedBegin"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      commandMock.Replay ();

      commandMock.Begin ();

      commandMock.VerifyAllExpectations ();
    }

    [Test]
    public void End_SetsCurrentTransaction ()
    {
      var commandMock = CreateCommandPartialMock ();

      commandMock
          .Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ScopedEnd"))
          .WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (_transaction)));
      commandMock.Replay ();

      commandMock.End ();

      commandMock.VerifyAllExpectations ();
    }

    private RelationEndPointModificationCommand CreateCommandPartialMock ()
    {
      return MockRepository.GeneratePartialMock<RelationEndPointModificationCommand> (_endPointMock, _oldRelatedObject, _newRelatedObject);
    }
  }
}