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
using System.Threading;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// The <see cref="IWxeFunctionExecutionListener"/> interface defines hooks for interacting with a <see cref="WxeFunction2"/>'s execution,
  /// including it's re-entry model.
  /// </summary>
  public interface IWxeFunctionExecutionListener
  {
    /// <summary>Play is invoked when the function's <see cref="WxeFunction2.Execute(WxeContext)"/> method is invoked (first and subsequent calls).</summary>
    void OnExecutionPlay (WxeContext context);

    /// <summary>Stop is invoked when the function's <see cref="WxeFunction2.Execute(WxeContext)"/> method is completed successfully.</summary>
    void OnExecutionStop (WxeContext context);

    /// <summary>
    /// Play is invoked when the function's <see cref="WxeFunction2.Execute(WxeContext)"/> method is exited by a <see cref="ThreadAbortException"/>,
    /// i.e. the execution is paused.
    /// </summary>
    void OnExecutionPause (WxeContext context);

    /// <summary>Play is invoked when the function's <see cref="WxeFunction2.Execute(WxeContext)"/> method fails.</summary>
    void OnExecutionFail (WxeContext context, Exception exception);
  }
}