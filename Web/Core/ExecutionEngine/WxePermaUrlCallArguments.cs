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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxePermaUrlCallArguments : IWxeCallArguments
  {
    private readonly WxeCallOptions _options;

    public WxePermaUrlCallArguments ()
        : this (new WxeCallOptions (new WxePermaUrlOptions (false, null)))
    {
    }

    public WxePermaUrlCallArguments (bool useParentPermaUrl)
        : this (new WxeCallOptions (new WxePermaUrlOptions (useParentPermaUrl, null)))
    {
    }

    public WxePermaUrlCallArguments (NameValueCollection urlParameters)
        : this (new WxeCallOptions (new WxePermaUrlOptions (false, urlParameters)))
    {
    }

    public WxePermaUrlCallArguments (bool useParentPermaUrl, NameValueCollection urlParameters)
        : this (new WxeCallOptions (new WxePermaUrlOptions (useParentPermaUrl, urlParameters)))
    {
    }

    protected internal WxePermaUrlCallArguments (WxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull ("options", options);

      _options = options;
    }

    public void Dispatch (WxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      _options.Dispatch (executor, function, null);
    }

    public WxeCallOptions Options
    {
      get { return _options; }
    }
  }
}