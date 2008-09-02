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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxeCallOptionsNoRepost"/> type if you whish to execute a <see cref="WxeFunction"/> as a sub function and suppressing 
  /// the re-post to the postback-handler after the execution has returned to the caller function.
  /// </summary>
  [Serializable]
  public class WxeCallOptionsNoRepost : WxeCallOptions
  {
    private readonly bool? _usesEventTarget;

    public WxeCallOptionsNoRepost ()
        : this (null, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget)
        : this (usesEventTarget, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (WxePermaUrlOptions permaUrlOptions)
        : this (null, permaUrlOptions)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget, WxePermaUrlOptions permaUrlOptions)
        : base (permaUrlOptions)
    {
      _usesEventTarget = usesEventTarget;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("handler", handler);

      executor.ExecuteFunctionNoRepost (function, handler.Sender, this);
    }

    public bool? UsesEventTarget
    {
      get { return _usesEventTarget; }
    }
  }
}