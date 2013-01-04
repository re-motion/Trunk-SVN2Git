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
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeFunctionStateManagerTest
  {
    private MockRepository _mockRepository;
    private HttpSessionStateBase _sessionMock;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _sessionMock = _mockRepository.StrictMock<HttpSessionStateBase>();

      Expect.Call (_sessionMock[GetSessionKeyForFunctionStates()]).Return (null);
      _sessionMock["key"] = null;
      LastCall.Constraints (
          Rhino.Mocks.Constraints.Is.Equal (GetSessionKeyForFunctionStates()),
          Rhino.Mocks.Constraints.Is.TypeOf (typeof (Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData>)));

      _functionState = new WxeFunctionState (new TestFunction(), 1, true);
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      SafeContext.Instance.SetData (typeof (WxeFunctionStateManager).AssemblyQualifiedName, null);
    }

    [Test]
    public void InitializeFromExistingSession ()
    {
      _mockRepository.BackToRecordAll();
      DateTime lastAccess = DateTime.Now;
      WxeFunctionStateManager.WxeFunctionStateMetaData functionStateMetaData =
          new WxeFunctionStateManager.WxeFunctionStateMetaData (Guid.NewGuid().ToString(), 1, lastAccess);
      Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData> functionStates =
          new Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData>();
      functionStates.Add (functionStateMetaData.FunctionToken, functionStateMetaData);

      var session = _mockRepository.StrictMock<HttpSessionStateBase>();

      Expect.Call (session[GetSessionKeyForFunctionStates()]).Return (functionStates);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (session);

      _mockRepository.VerifyAll();
      Assert.That (functionStateManager.GetLastAccess (functionStateMetaData.FunctionToken), Is.EqualTo (lastAccess));
    }

    [Test]
    public void Add ()
    {
      _sessionMock.Add (GetSessionKeyForFunctionState(), _functionState);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      functionStateManager.Add (_functionState);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetItem ()
    {
      Expect.Call (_sessionMock.Mode).Return (SessionStateMode.InProc);
      Expect.Call (_sessionMock[GetSessionKeyForFunctionState()]).Return (_functionState);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      WxeFunctionState actual = functionStateManager.GetItem (_functionState.FunctionToken);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (_functionState));
    }

    [Test]
    public void Abort ()
    {
      _sessionMock.Remove (GetSessionKeyForFunctionState());
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      functionStateManager.Abort (_functionState);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Touch ()
    {
      SetupResult.For (_sessionMock[GetSessionKeyForFunctionState()]).Return (_functionState);
      _sessionMock.Add (GetSessionKeyForFunctionState (), _functionState);
      _mockRepository.ReplayAll ();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      functionStateManager.Add (_functionState);
      DateTime lastAccess = functionStateManager.GetLastAccess (_functionState.FunctionToken);
      Thread.Sleep (1000);
      functionStateManager.Touch (_functionState.FunctionToken);
      Assert.Greater (functionStateManager.GetLastAccess (_functionState.FunctionToken), lastAccess);

      _mockRepository.VerifyAll();
    }

    [Test]
    [Explicit]
    public void IsExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionState = new WxeFunctionState (new TestFunction(), 1, true);
      SetupResult.For (_sessionMock[GetSessionKeyForFunctionState (functionState.FunctionToken)]).Return (functionState);
      _sessionMock[GetSessionKeyForFunctionState (functionState.FunctionToken)] = functionState;
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      functionStateManager.Add (functionState);
      Assert.That (functionStateManager.IsExpired (functionState.FunctionToken), Is.False);
      Thread.Sleep (61000);
      Assert.That (functionStateManager.IsExpired (functionState.FunctionToken), Is.True);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsExpired_WithUnknownFunctionToken ()
    {
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      Assert.That (functionStateManager.IsExpired (Guid.NewGuid().ToString()), Is.True);

      _mockRepository.VerifyAll();
    }

    [Test]
    [Explicit]
    public void CleanupExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionStateExpired = new WxeFunctionState (new TestFunction(), 1, true);
      SetupResult.For (_sessionMock[GetSessionKeyForFunctionState (functionStateExpired.FunctionToken)]).Return (functionStateExpired);
      _sessionMock[GetSessionKeyForFunctionState (functionStateExpired.FunctionToken)] = functionStateExpired;

      WxeFunctionState functionStateNotExpired = new WxeFunctionState (new TestFunction(), 10, true);
      SetupResult.For (_sessionMock[GetSessionKeyForFunctionState (functionStateNotExpired.FunctionToken)]).Return (functionStateNotExpired);
      _sessionMock[GetSessionKeyForFunctionState (functionStateNotExpired.FunctionToken)] = functionStateNotExpired;

      _sessionMock.Remove (GetSessionKeyForFunctionState (functionStateExpired.FunctionToken));

      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      functionStateManager.Add (functionStateExpired);
      functionStateManager.Add (functionStateNotExpired);

      Thread.Sleep (61000);

      Assert.That (functionStateManager.IsExpired (functionStateExpired.FunctionToken), Is.True);
      Assert.That (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken), Is.False);

      functionStateManager.CleanUpExpired();

      Assert.That (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken), Is.False);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void HasSessionAndGetCurrent ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.That (WxeFunctionStateManager.HasSession, Is.False);
      Assert.That (WxeFunctionStateManager.Current, Is.Not.Null);
      Assert.That (WxeFunctionStateManager.HasSession, Is.True);
    }

    [Test]
    public void GetCurrent_SameInstanceTwice ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.That (WxeFunctionStateManager.Current, Is.SameAs (WxeFunctionStateManager.Current));
    }

    [Test]
    public void HasSessionAndGetCurrentInSeparateThreads ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.That (WxeFunctionStateManager.HasSession, Is.False);
      Assert.That (WxeFunctionStateManager.Current, Is.Not.Null);
      ThreadRunner.Run (
          delegate
          {
            HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
            Assert.That (WxeFunctionStateManager.HasSession, Is.False);
            Assert.That (WxeFunctionStateManager.Current, Is.Not.Null);
            Assert.That (WxeFunctionStateManager.HasSession, Is.True);
          });
    }


    private string GetSessionKeyForFunctionState ()
    {
      string functionToken = _functionState.FunctionToken;
      return GetSessionKeyForFunctionState (functionToken);
    }

    private string GetSessionKeyForFunctionState (string functionToken)
    {
      return typeof (WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionState|" + functionToken;
    }

    private string GetSessionKeyForFunctionStates ()
    {
      return typeof (WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionStates";
    }
  }
}
