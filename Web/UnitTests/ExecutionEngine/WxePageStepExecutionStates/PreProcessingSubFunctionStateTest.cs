/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.Utilities;
using Rhino.Mocks;
using PreparingRedirectToSubFunctionState_WithPermaUrl =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrl.PreparingRedirectToSubFunctionState;
using ExecutingSubFunctionState_WithoutPermaUrl =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithoutPermaUrl.ExecutingSubFunctionState;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates
{
  [TestFixture]
  public class PreProcessingSubFunctionStateTest : TestBase
  {
    private WxeStep _parentStep;
    private IWxePage _pageMock;

    public override void SetUp ()
    {
      base.SetUp();
      _parentStep = new WxePageStep ("page.aspx");
      _pageMock = MockRepository.StrictMock<IWxePage>();
      PostBackCollection.Add ("Key", "Value");
      PostBackCollection.Add (ControlHelper.PostEventSourceID, "TheEventSource");
      PostBackCollection.Add (ControlHelper.PostEventArgumentID, "TheEventArgument");
      PostBackCollection.Add ("TheUnqiueID", "Value");
    }

    [Test]
    public void PreProcessSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, List.Contains ("Key"));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    private PreProcessingSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock, new PreProcessingSubFunctionStateParameters (_parentStep, _pageMock, SubFunction, permaUrlOptions));
    }
  }
}