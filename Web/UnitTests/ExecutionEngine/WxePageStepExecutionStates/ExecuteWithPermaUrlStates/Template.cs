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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  [TestFixture][Ignore]
  public class Template:TestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RedirectToSubFunction ()
    {
      IWxePageStepExecutionState executionState = new WxePageStepExecutionState();
      executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ExecuteSubFunction ()
    {
      IWxePageStepExecutionState executionState = new WxePageStepExecutionState ();
      executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ReturnFromSubFunction ()
    {
      IWxePageStepExecutionState executionState = new WxePageStepExecutionState ();
      executionState.ReturnFromSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void PostProcessSubFunction ()
    {
      IWxePageStepExecutionState executionState = new WxePageStepExecutionState ();
      executionState.PostProcessSubFunction (WxeContext);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Cleanup ()
    {
      IWxePageStepExecutionState executionState = new WxePageStepExecutionState ();
      executionState.Cleanup (WxeContext);
    }
  }
}