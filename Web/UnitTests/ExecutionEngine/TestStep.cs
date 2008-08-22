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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class TestStep: WxeStep
{
  public static new T GetStepByType<T> (WxeStep step)
    where T:WxeStep
  {
    return WxeStep.GetStepByType<T> (step);
  }

  private bool _isExecuteCalled;
  private bool _isAbortRecursiveCalled;
  private WxeContext _wxeContext;

  public TestStep()
	{
	}

  public override void Execute(WxeContext context)
  {
    _isExecuteCalled = true;
    _wxeContext = context;
  }

  protected override void AbortRecursive()
  {
    base.AbortRecursive ();
    _isAbortRecursiveCalled = true;
  }

  public bool IsExecuteCalled
  {
    get { return _isExecuteCalled; }
  }

  public bool IsAbortRecursiveCalled
  {
    get { return _isAbortRecursiveCalled; }
  }

  public WxeContext WxeContext
  {
    get { return _wxeContext; }
  }
}

}
