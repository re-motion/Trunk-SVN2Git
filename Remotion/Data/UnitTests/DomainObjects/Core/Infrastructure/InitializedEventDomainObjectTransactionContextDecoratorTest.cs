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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class InitializedEventDomainObjectTransactionContextDecoratorTest
  {
    [Test]
    public void AllowedMembers ()
    {
      CheckAllowed (ctx => ctx.ClientTransaction, ClientTransaction.CreateRootTransaction());
      CheckAllowed (ctx => ctx.IsInvalid, true);
      CheckAllowed (ctx => ctx.Execute ((obj, tx) => 10), 20);
      CheckAllowed (ctx => ctx.Execute ((obj, tx) => { }));
    }

    [Test]
    public void ForbiddenMembers ()
    {
      CheckForbidden (ctx => Dev.Null = ctx.State);
      CheckForbidden (ctx => Dev.Null = ctx.Timestamp);
      CheckForbidden (ctx => ctx.MarkAsChanged ());
      CheckForbidden (ctx => ctx.EnsureDataAvailable ());
    }

    private void CheckAllowed<T> (Func<IDomainObjectTransactionContext, T> func, T mockResult)
    {
      var contextMock = MockRepository.GenerateMock<IDomainObjectTransactionContext> ();
      contextMock.Expect (mock => func (mock)).Return (mockResult);
      contextMock.Replay ();

      var result = func (new InitializedEventDomainObjectTransactionContextDecorator (contextMock));

      contextMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (result));
    }

    private void CheckAllowed (Action<IDomainObjectTransactionContext> action)
    {
      var contextMock = MockRepository.GenerateMock<IDomainObjectTransactionContext> ();
      contextMock.Expect (action);
      contextMock.Replay ();

      action (new InitializedEventDomainObjectTransactionContextDecorator (contextMock));

      contextMock.VerifyAllExpectations ();
    }

    private void CheckForbidden (Action<IDomainObjectTransactionContext> action)
    {
      var contextMock = MockRepository.GenerateMock<IDomainObjectTransactionContext> ();

      try
      {
        action (new InitializedEventDomainObjectTransactionContextDecorator (contextMock));
        Assert.Fail ("Expected exception.");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (
            ex.Message, 
            Is.EqualTo ("While the OnReferenceInitializing event is executing, this member cannot be used."), "Invalid exception message.");
      }

      contextMock.AssertWasNotCalled (action);
    }
  }
}