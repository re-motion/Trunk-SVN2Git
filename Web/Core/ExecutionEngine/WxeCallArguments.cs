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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public sealed class WxeCallArguments : WxeCallArgumentsBase
  {
    public static readonly IWxeCallArguments Default = new WxeCallArgumentsBase (new WxeCallOptions (WxePermaUrlOptions.Null));

    private readonly Control _sender;

    public WxeCallArguments (Control sender, WxeCallOptions options)
        : base (options)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      _sender = sender;
    }

    public Control Sender
    {
      get { return _sender; }
    }

    protected override void Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      Options.Dispatch (executor, function, this);
    }
  }
}