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
  public class WxeCallOptionsNoRepost: WxeCallOptions
  {
    private readonly bool? _usesEventTarget;

    public WxeCallOptionsNoRepost ()
        : this (null, null)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget)
        : this (usesEventTarget, null)
    {
    }

    public WxeCallOptionsNoRepost (WxePermaUrlOptions usePermaUrl)
        : this (null, usePermaUrl)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget, WxePermaUrlOptions usePermaUrl)
        : base (usePermaUrl)
    {
      _usesEventTarget = usesEventTarget;
    }

    public override void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("handler", handler);

      if (UsePermaUrl != null)
      {
        if (_usesEventTarget != null)
          page.ExecuteFunctionNoRepost (function, handler.Sender, _usesEventTarget.Value, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
        else
          page.ExecuteFunctionNoRepost (function, handler.Sender, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
      }
      else
      {
        if (_usesEventTarget != null)
          page.ExecuteFunctionNoRepost (function, handler.Sender, _usesEventTarget.Value);
        else
          page.ExecuteFunctionNoRepost (function, handler.Sender);
      }
    }

    public bool? UsesEventTarget 
    { 
      get { return _usesEventTarget; }
    }
  }
}