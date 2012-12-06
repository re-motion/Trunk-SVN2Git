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
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Development.RhinoMocks.UnitTesting
{
  /// <summary>
  /// Provides functionality for testing decorator methods that do nothing else but forward to the equivalent methods on a decorated object.
  /// </summary>
  /// <typeparam name="TInterface">The type of the interface.</typeparam>
  public class DecoratorTestHelper<TInterface> 
      where TInterface : class
  {
    private readonly TInterface _decorator;
    private readonly TInterface _decoratedMock;

    public DecoratorTestHelper (TInterface decorator, TInterface decoratedMock)
    {
      ArgumentUtility.CheckNotNull ("decorator", decorator);
      ArgumentUtility.CheckNotNull ("decoratedMock", decoratedMock);

      _decorator = decorator;
      _decoratedMock = decoratedMock;
    }

    public void CheckDelegation<TR> (Func<TInterface, TR> action, TR fakeResult)
    {
      CheckDelegation (action, fakeResult, result => Assert.That (result, Is.EqualTo (fakeResult)));
    }

    public void CheckDelegation<TR> (Func<TInterface, TR> action, TR fakeResult, Action<TR> decoratorResultChecker)
    {
      _decoratedMock.Expect (mock => action (mock)).Return (fakeResult);
      _decoratedMock.Replay ();

      var result = action (_decorator);

      _decoratedMock.VerifyAllExpectations ();
      decoratorResultChecker (result);
    }

    public void CheckDelegation (Action<TInterface> action)
    {
      _decoratedMock.Expect (action);
      _decoratedMock.Replay ();

      action (_decorator);

      _decoratedMock.VerifyAllExpectations ();
    }

    public void CheckDelegationWithContinuation<TR> (Func<TInterface, TR> action, TR fakeResult, Action<MethodInvocation> whenCalled)
    {
      _decoratedMock.Expect (mock => action (mock)).Return (fakeResult).WhenCalled (whenCalled);
      _decoratedMock.Replay ();

      var result = action (_decorator);

      _decoratedMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (fakeResult));
    }

    public void CheckDelegationWithContinuation (Action<TInterface> action, Action<MethodInvocation> whenCalled)
    {
      _decoratedMock.Expect (action).WhenCalled (whenCalled);
      _decoratedMock.Replay ();

      action (_decorator);

      _decoratedMock.VerifyAllExpectations ();
    }
  }
}