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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PreProcessingSubFunctionStateTest : TestBase
  {
    private const string c_senderUniqueID = "TheUnqiueID";
    private WxeStep _parentStep;
    private IWxePage _pageMock;

    public override void SetUp ()
    {
      base.SetUp();

      _parentStep = new WxePageStep ("page.aspx");
      ExecutionStateContextMock.Stub (stub => stub.CurrentStep).Return (_parentStep).Repeat.Any();

      _pageMock = MockRepository.StrictMock<IWxePage>();
      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      PostBackCollection.Add ("Key", "Value");
      PostBackCollection.Add (c_senderUniqueID, "Value");
      PostBackCollection.Add (ControlHelper.PostEventSourceID, "TheEventSource");
      PostBackCollection.Add (ControlHelper.PostEventArgumentID, "TheEventArgument");
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithoutPermaUrl ()
    {
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null);

      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", c_senderUniqueID, ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl ()
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
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", c_senderUniqueID, ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.SameAs (_parentStep));
            Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_SuppressSender_IPostBackEventHandler ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackEventHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = CreateExecutionState (senderMock);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_SuppressSender_IPostBackDataHandler ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackDataHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = CreateExecutionState (senderMock);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_UsesEventTarget ()
    {
      IExecutionState executionState = CreateExecutionState (true);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_UsesEventTarget_SuppressSender_SenderRemains ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackDataHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = new PreProcessingSubFunctionState (
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, WxePermaUrlOptions.Null),
          new WxeRepostOptions (senderMock, true));

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionWithoutPermaUrlState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionWithoutPermaUrlState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    private PreProcessingSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, permaUrlOptions),
          WxeRepostOptions.Null);
    }

    private PreProcessingSubFunctionState CreateExecutionState (Control sender)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, WxePermaUrlOptions.Null),
          new WxeRepostOptions (sender, false));
    }

    private PreProcessingSubFunctionState CreateExecutionState (bool usesEventTarget)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, WxePermaUrlOptions.Null),
          new WxeRepostOptions (MockRepository.Stub<Control>(), usesEventTarget));
    }
  }
}