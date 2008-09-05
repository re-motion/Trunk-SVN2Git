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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PreProcessingSubFunctionStateBaseTest : TestBase
  {
    private WxeStep _parentStep;
    private IWxePage _pageMock;

    public override void SetUp ()
    {
      base.SetUp();
      _parentStep = new WxePageStep ("page.aspx");
      _pageMock = MockRepository.StrictMock<IWxePage>();
      PostBackCollection.Add ("Key", "Value");
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

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, List.Contains ("Key"));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    [Test]
    public void PreProcessSubFunction_WithPermaUrl ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<PreparingRedirectToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((PreparingRedirectToSubFunctionState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, List.Contains ("Key"));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
            Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

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
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_parentStep, _pageMock, SubFunction, permaUrlOptions, WxeRepostOptions.Null));
    }
  }
}