// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Web.SessionState;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeFunctionStateManagerTest
  {
    private MockRepository _mockRepository;
    private IHttpSessionState _sessionMock;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _sessionMock = _mockRepository.StrictMock<IHttpSessionState>();

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

      IHttpSessionState session = _mockRepository.StrictMock<IHttpSessionState>();

      Expect.Call (session[GetSessionKeyForFunctionStates()]).Return (functionStates);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (session);

      _mockRepository.VerifyAll();
      Assert.AreEqual (lastAccess, functionStateManager.GetLastAccess (functionStateMetaData.FunctionToken));
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
      Assert.AreSame (_functionState, actual);
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
      Assert.IsFalse (functionStateManager.IsExpired (functionState.FunctionToken));
      Thread.Sleep (61000);
      Assert.IsTrue (functionStateManager.IsExpired (functionState.FunctionToken));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsExpired_WithUnknownFunctionToken ()
    {
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_sessionMock);
      Assert.IsTrue (functionStateManager.IsExpired (Guid.NewGuid().ToString()));

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

      Assert.IsTrue (functionStateManager.IsExpired (functionStateExpired.FunctionToken));
      Assert.IsFalse (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken));

      functionStateManager.CleanUpExpired();

      Assert.IsFalse (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void HasSessionAndGetCurrent ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.IsFalse (WxeFunctionStateManager.HasSession);
      Assert.IsNotNull (WxeFunctionStateManager.Current);
      Assert.IsTrue (WxeFunctionStateManager.HasSession);
    }

    [Test]
    public void GetCurrent_SameInstanceTwice ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.AreSame (WxeFunctionStateManager.Current, WxeFunctionStateManager.Current);
    }

    [Test]
    public void HasSessionAndGetCurrentInSeparateThreads ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.IsFalse (WxeFunctionStateManager.HasSession);
      Assert.IsNotNull (WxeFunctionStateManager.Current);
      ThreadRunner.Run (
          delegate
          {
            HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
            Assert.IsFalse (WxeFunctionStateManager.HasSession);
            Assert.IsNotNull (WxeFunctionStateManager.Current);
            Assert.IsTrue (WxeFunctionStateManager.HasSession);
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
