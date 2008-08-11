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
  public class WxeCallArguments : IWxeCallArguments
  {
    private class DefaultArguments : IWxeCallArguments
    {
      public void Call (IWxePage page, WxeFunction function)
      {
        ArgumentUtility.CheckNotNull ("page", page);
        ArgumentUtility.CheckNotNull ("function", function);

        page.ExecuteFunction (function);
      }
    }

    private static readonly IWxeCallArguments s_default = new DefaultArguments ();

    public static IWxeCallArguments Default
    {
      get { return s_default; }
    }

    private readonly Control _sender;
    private readonly WxeCallOptions _options;

    public WxeCallArguments (Control sender, WxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNull ("options", options);

      _options = options;
      _sender = sender;
    }

    public void Call (IWxePage page, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);

      _options.Call (page, function, this);
    }

    public Control Sender
    {
      get { return _sender; }
    }

    public WxeCallOptions Options
    {
      get { return _options; }
    }
  }
}
