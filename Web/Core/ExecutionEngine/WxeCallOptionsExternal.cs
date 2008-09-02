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
  /// Use the <see cref="WxeCallOptionsExternal"/> type if you whish to execute a <see cref="WxeFunction"/> as a new root function, 
  /// typically in a new window. The <see cref="WxeFunction"/> will be initialized on the server and then opened via a Javascript call.
  /// </summary>
  [Serializable]
  public class WxeCallOptionsExternal : WxeCallOptions
  {
    private readonly string _target;
    private readonly string _features;
    private readonly bool _returningPostback;

    public WxeCallOptionsExternal (string target)
        : this (target, null, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features)
        : this (target, features, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback)
        : this (target, features, returningPostback, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback, WxePermaUrlOptions permaUrlOptions)
        : base (permaUrlOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("target", target);

      _target = target;
      _features = features;
      _returningPostback = returningPostback;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("handler", handler);

      executor.ExecuteFunctionExternal (function, handler.Sender, this);

      throw new WxeCallExternalException();
    }

    public string Target
    {
      get { return _target; }
    }

    public string Features
    {
      get { return _features; }
    }

    public bool ReturningPostback
    {
      get { return _returningPostback; }
    }
  }
}