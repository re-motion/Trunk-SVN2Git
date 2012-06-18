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

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public abstract class ForwardingEventListenerTestBase<TEventListener> : StandardMappingTest
  {
    private ClientTransactionEventSinkWithMock _eventSinkWithMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _eventSinkWithMock = ClientTransactionEventSinkWithMock.CreateWithStrictMock (ClientTransaction.CreateRootTransaction ());
    }

    protected abstract TEventListener EventListener { get; }

    public ClientTransactionEventSinkWithMock EventSinkWithMock
    {
      get { return _eventSinkWithMock; }
    }

    protected void CheckEventDelegation (Action<TEventListener> action, Action<ClientTransaction, IClientTransactionListener> expectedEvent)
    {
      _eventSinkWithMock.ExpectMock (mock => expectedEvent (_eventSinkWithMock.ClientTransaction, mock));
      _eventSinkWithMock.ReplayMock ();

      action (EventListener);

      _eventSinkWithMock.VerifyMock ();
    }
  }
}