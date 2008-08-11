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
  public class WxeCallOptionsExternal : WxeCallOptions
  {
    private readonly string _target;
    private readonly string _features;
    private readonly bool _returningPostback;

    public WxeCallOptionsExternal (string target, string features, bool returningPostback)
        : this (target, features, returningPostback, null)
    {
    }

    public WxeCallOptionsExternal (string target, string features)
        : this (target, features, true, null)
    {
    }

    public WxeCallOptionsExternal (string target)
        : this (target, null, true, null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback, WxePermaUrlOptions usePermaUrl)
        : base (usePermaUrl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("target", target);

      _target = target;
      _features = features;
      _returningPostback = returningPostback;
    }

    public override void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      if (UsePermaUrl != null)
        page.ExecuteFunctionExternal (function, _target, _features, handler.Sender, _returningPostback, true, UsePermaUrl.UseParentPermaUrl, UsePermaUrl.UrlParameters);
      else
        page.ExecuteFunctionExternal (function, _target, _features, handler.Sender, _returningPostback);

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