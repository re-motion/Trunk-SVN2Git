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
  public class WxePermaUrlOptions : IWxeCallArguments
  {
    private readonly bool _useParentPermaUrl;
    private readonly NameValueCollection _urlParameters;

    public WxePermaUrlOptions ()
        : this (false, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl)
        : this (useParentPermaUrl, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      _useParentPermaUrl = useParentPermaUrl;
      _urlParameters = urlParameters;
    }

    public bool UseParentPermaUrl
    {
      get { return _useParentPermaUrl; }
    }

    public NameValueCollection UrlParameters
    {
      get { return _urlParameters; }
    }

    public void Call (IWxePage page, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("function", function);

      page.ExecuteFunction (function, true, UseParentPermaUrl, UrlParameters);
    }
  }
}