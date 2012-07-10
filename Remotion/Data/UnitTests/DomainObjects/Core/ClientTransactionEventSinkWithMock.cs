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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public class ClientTransactionEventSinkWithMock : IClientTransactionEventSink
  {
    public static ClientTransactionEventSinkWithMock CreateWithStrictMock (ClientTransaction clientTransaction = null, MockRepository mockRepository = null)
    {
      var mock = mockRepository != null ? mockRepository.StrictMock<IClientTransactionListener>() : MockRepository.GenerateStrictMock<IClientTransactionListener> ();
      return new ClientTransactionEventSinkWithMock (mock, clientTransaction);
    }

    public static ClientTransactionEventSinkWithMock CreateWithDynamicMock (ClientTransaction clientTransaction = null, MockRepository mockRepository = null)
    {
      var mock = mockRepository != null ? mockRepository.DynamicMock<IClientTransactionListener> () : MockRepository.GenerateMock<IClientTransactionListener> ();
      return new ClientTransactionEventSinkWithMock (mock, clientTransaction);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _mock;

    public ClientTransactionEventSinkWithMock (IClientTransactionListener mock, ClientTransaction clientTransaction = null)
    {
      ArgumentUtility.CheckNotNull ("mock", mock);

      _clientTransaction = clientTransaction ?? ClientTransactionObjectMother.Create();
      _mock = mock;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionListener Mock
    {
      get { return _mock; }
    }

    public void RaiseEvent (Action<ClientTransaction, IClientTransactionListener> action)
    {
      action (_clientTransaction, _mock);
    }

    public void ReplayMock()
    {
      _mock.Replay();
    }

    public void VerifyMock ()
    {
      _mock.VerifyAllExpectations();
    }

    public void BackToRecordMock ()
    {
      _mock.BackToRecord();
    }

    public MockRepository GetMockRepository ()
    {
      return _mock.GetMockRepository();
    }

    public IMethodOptions<R> ExpectMock<R> (Function<IClientTransactionListener,R> func)
    {
      return _mock.Expect (func);
    }

    public IMethodOptions<RhinoMocksExtensions.VoidType> ExpectMock (Action<IClientTransactionListener> action)
    {
      return _mock.Expect (action);
    }

    public IMethodOptions<R> StubMock<R> (Function<IClientTransactionListener, R> func)
    {
      return _mock.Stub (func);
    }

    public IMethodOptions<object> StubMock (Action<IClientTransactionListener> action)
    {
      return _mock.Stub (action);
    }

    public void AssertWasCalledMock (Func<IClientTransactionListener, object> action)
    {
      _mock.AssertWasCalled (action);
    }

    public void AssertWasCalledMock (Action<IClientTransactionListener> action)
    {
      _mock.AssertWasCalled (action);
    }

    public void AssertWasNotCalledMock (Func<IClientTransactionListener, object> action)
    {
      _mock.AssertWasNotCalled (action);
    }

    public void AssertWasNotCalledMock (Action<IClientTransactionListener> action)
    {
      _mock.AssertWasNotCalled (action);
    }
  }
}