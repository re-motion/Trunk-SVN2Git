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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
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
      ExecutionStateContextMock.Stub (stub => stub.CurrentStep).Return (_parentStep).Repeat.Any ();

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
      IExecutionState executionState = CreateExecutionState (WxePermaUrlOptions.Null, WxeReturnOptions.Null);
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      WxeReturnOptions returnOptions = new WxeReturnOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions, returnOptions);

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
            Assert.That (nextState.Parameters.SubFunction.ParentStep, Is.Null);
            Assert.That (nextState.Parameters.PermaUrlOptions, Is.SameAs (permaUrlOptions));
            Assert.That (nextState.ReturnOptions, Is.SameAs (returnOptions));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    private PreProcessingSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions, WxeReturnOptions returnOptions)
    {
      return new PreProcessingSubFunctionState (
          ExecutionStateContextMock, new PreProcessingSubFunctionStateParameters (_pageMock, SubFunction, permaUrlOptions), returnOptions);
    }
  }
}