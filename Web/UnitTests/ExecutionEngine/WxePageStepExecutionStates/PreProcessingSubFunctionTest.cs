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
using Rhino.Mocks;
using PreparingRedirectToSubFunctionState_WithPermaUrl =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrl.PreparingRedirectToSubFunctionState;
using ExecutingSubFunctionState_WithoutPermaUrl =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithoutPermaUrl.ExecutingSubFunctionState;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates
{
  [TestFixture]
  public class PreProcessingSubFunctionTest : TestBase
  {
    private WxeStep _parentStep;
    private IWxePage _pageMock;

    public override void SetUp ()
    {
      base.SetUp();
      _parentStep = new WxePageStep ("page.aspx");
      _pageMock = MockRepository.StrictMock<IWxePage>();
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);
      Assert.That (executionState.IsExecuting, Is.False);
    }

    [Test]
    public void PreProcessSubFunction_WithoutPermaUrl ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.EquivalentTo (PostBackCollection));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction ();

      MockRepository.VerifyAll();
    }

    [Test]
    public void PreProcessSubFunction_WithPermaUrl ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState(permaUrlOptions);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<PreparingRedirectToSubFunctionState_WithPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((PreparingRedirectToSubFunctionState_WithPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.EquivalentTo (PostBackCollection));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
            Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction ();

      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ExecuteSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);
      executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PostProcessSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);
      executionState.PostProcessSubFunction (WxeContext);
    }

    private PreProcessingSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock, new PreProcessingSubFunctionStateParameters (_parentStep, _pageMock, SubFunction, permaUrlOptions));
    }
  }
}