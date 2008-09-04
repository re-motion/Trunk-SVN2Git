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
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [TestFixture]
  public class PreparingRedirectToSubFunctionStateTest : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (RootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (SubFunction.GetType(), "~/sub.wxe"));

      Uri uri = new Uri ("http://localhost/root.wxe");

      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/sub.wxe")).Return ("~/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("~/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/session/root.wxe")).Return ("~/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();

      RequestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      RequestMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions(), WxeReturnOptions.Null);
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PreProcessSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions (), WxeReturnOptions.Null);
      executionState.PreProcessSubFunction ();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_DoNotReturnToCaller_GoesToRedirectingToSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions(), WxeReturnOptions.Null);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.SubFunction.ReturnUrl, Is.EqualTo ("DefaultReturn.html"));
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + SubFunction.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_ReturnToCaller_GoesToRedirectingToSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions(), new WxeReturnOptions());
      ExecutionStateContextMock.Stub (stub => stub.ParentFunction).Return (RootFunction).Repeat.Any();

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.SubFunction.ReturnUrl, Is.EqualTo ("~/session/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + SubFunction.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_ReturnToCaller_WithCallerUrlParameters_GoesToRedirectingToSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions, new WxeReturnOptions (new NameValueCollection { { "Key", "Value" } }));
      ExecutionStateContextMock.Stub (stub => stub.ParentFunction).Return (RootFunction).Repeat.Any();

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (
                nextState.Parameters.SubFunction.ReturnUrl,
                Is.EqualTo ("~/session/root.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken));
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("~/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + SubFunction.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithCustumUrlParamters_DoNotReturnToCaller_GoesToRedirectingToSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (false, new NameValueCollection { { "Key", "NewValue" } });
      IExecutionState executionState = CreateExecutionState (permaUrlOptions, WxeReturnOptions.Null);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.SubFunction.ReturnUrl, Is.EqualTo ("DefaultReturn.html"));
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("~/session/sub.wxe?Key=NewValue&WxeFunctionToken=" + SubFunction.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithParentPermaUrl_DoNotReturnToCaller_GoesToRedirectingToSubFunction ()
    {
      WxeContext.QueryString.Add ("Key", "NewValue");

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (true);
      IExecutionState executionState = CreateExecutionState (permaUrlOptions, WxeReturnOptions.Null);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .Do (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (nextState.Parameters.SubFunction.ReturnUrl, Is.EqualTo ("DefaultReturn.html"));
            
            string destinationUrl = UrlUtility.AddParameters (
                "~/session/sub.wxe",
                new NameValueCollection
                {
                    { "Parameter1", "OtherValue" },
                    { WxeHandler.Parameters.WxeFunctionToken, SubFunction.FunctionToken },
                    { WxeHandler.Parameters.ReturnUrl, "/root.wxe?Key=NewValue" }
                },
                Encoding.Default);
            Assert.That (nextState.Parameters.DestinationUrl, Is.EqualTo (destinationUrl));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void PostProcessSubFunction ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions(), WxeReturnOptions.Null);
      executionState.PostProcessSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'PreparingRedirectToSubFunctionState' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.\r\n"
        + "Parameter name: parameters",
        MatchType = MessageMatch.Contains)]
    public void Initialize_WithoutPermaUrl ()
    {
      CreateExecutionState (WxePermaUrlOptions.Null, WxeReturnOptions.Null);
    }

    private PreparingRedirectToSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions, WxeReturnOptions returnOptions)
    {
      return new PreparingRedirectToSubFunctionState (
          ExecutionStateContextMock, new PreparingSubFunctionStateParameters (SubFunction, PostBackCollection, permaUrlOptions, returnOptions));
    }
  }
}