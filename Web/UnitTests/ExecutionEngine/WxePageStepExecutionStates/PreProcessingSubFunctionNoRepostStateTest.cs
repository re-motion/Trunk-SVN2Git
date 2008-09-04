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
using Remotion.Web.Utilities;
using Rhino.Mocks;
using ExecutingSubFunctionState_WithoutPermaUrl =
    Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithoutPermaUrl.ExecutingSubFunctionState;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates
{
  [TestFixture]
  public class PreProcessingSubFunctionNoRepostStateTest : TestBase
  {
    private const string c_senderUniqueID = "TheUnqiueID";
    private WxeStep _parentStep;
    private IWxePage _pageMock;
    private PreProcessingSubFunctionStateParameters _parameters;

    public override void SetUp ()
    {
      base.SetUp();

      _parentStep = new WxePageStep ("page.aspx");
      _pageMock = MockRepository.StrictMock<IWxePage>();
      _pageMock.Stub (stub => stub.GetPostBackCollection()).Return (PostBackCollection);

      PostBackCollection.Add ("Key", "Value");
      PostBackCollection.Add (c_senderUniqueID, "Value");
      PostBackCollection.Add (ControlHelper.PostEventSourceID, "TheEventSource");
      PostBackCollection.Add (ControlHelper.PostEventArgumentID, "TheEventArgument");

      _parameters = new PreProcessingSubFunctionStateParameters (_parentStep, _pageMock, SubFunction, WxePermaUrlOptions.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.")]
    public void Inititalize_IsNot_IPostBackDataHandler_Or_IPostBackDataHandler ()
    {
      Control senderMock = MockRepository.PartialMock<Control>();
      new PreProcessingSubFunctionNoRepostState (ExecutionStateContextMock, _parameters, senderMock, false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Parameter name: sender", MatchType = MessageMatch.Contains)]
    public void Inititalize_NotUsesEventTarget_NotSuppressSender ()
    {
      new PreProcessingSubFunctionNoRepostState (ExecutionStateContextMock, _parameters, null, false);
    }

    [Test]
    public void PreProcessSubFunction_SuppressSender_IPostBackEventHandler ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackEventHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = CreateExecutionState (senderMock);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    [Test]
    public void PreProcessSubFunction_SuppressSender_IPostBackDataHandler ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackDataHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = CreateExecutionState (senderMock);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection, Is.Not.SameAs (PostBackCollection));
            Assert.That (
                nextState.Parameters.PostBackCollection.AllKeys,
                Is.EquivalentTo (new[] { "Key", ControlHelper.PostEventSourceID, ControlHelper.PostEventArgumentID }));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    [Test]
    public void PreProcessSubFunction_UsesEventTarget ()
    {
      IExecutionState executionState = CreateExecutionState (true);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    [Test]
    public void PreProcessSubFunction_UsesEventTarget_SuppressSender_SenderRemains ()
    {
      Control senderMock = MockRepository.PartialMultiMock<Control> (typeof (IPostBackDataHandler));
      senderMock.Stub (stub => stub.UniqueID).Return (c_senderUniqueID).Repeat.Any();

      IExecutionState executionState = new PreProcessingSubFunctionNoRepostState (
          ExecutionStateContextMock,
          new PreProcessingSubFunctionStateParameters (_parentStep, _pageMock, SubFunction, WxePermaUrlOptions.Null),
          senderMock,
          true);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<ExecutingSubFunctionState_WithoutPermaUrl>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((ExecutingSubFunctionState_WithoutPermaUrl) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.PostBackCollection.AllKeys, Is.EquivalentTo (new[] { "Key", c_senderUniqueID }));
          });

      MockRepository.ReplayAll();

      executionState.PreProcessSubFunction();

      MockRepository.VerifyAll();
    }

    private PreProcessingSubFunctionNoRepostState CreateExecutionState (Control sender)
    {
      return new PreProcessingSubFunctionNoRepostState (ExecutionStateContextMock, _parameters, sender, false);
    }

    private PreProcessingSubFunctionNoRepostState CreateExecutionState (bool usesEventTarget)
    {
      return new PreProcessingSubFunctionNoRepostState (
          ExecutionStateContextMock, _parameters, MockRepository.Stub<Control>(), usesEventTarget);
    }
  }
}