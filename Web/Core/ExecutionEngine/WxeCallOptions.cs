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
  public class WxeCallOptions 
  {
    private readonly WxePermaUrlOptions _usePermaUrl;

    public WxeCallOptions (WxePermaUrlOptions usePermaUrl)
    {
      _usePermaUrl = usePermaUrl;
    }

    public WxeCallOptions ()
        : this (null)
    {
    }

    public virtual void Call (IWxePage page, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);

      if (_usePermaUrl != null)
        _usePermaUrl.Call (page, function);
      else
        page.ExecuteFunction (function);
    }

    public WxePermaUrlOptions UsePermaUrl
    {
      get { return _usePermaUrl; }
    }
  }
}