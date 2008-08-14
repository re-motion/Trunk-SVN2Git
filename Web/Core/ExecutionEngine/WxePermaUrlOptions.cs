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

namespace Remotion.Web.ExecutionEngine
{
  public sealed class WxePermaUrlOptions : INullObject
  {
    public static readonly WxePermaUrlOptions Null = new WxePermaUrlOptions (false, false, null);

    private readonly bool _usePermaUrl;
    private readonly bool _useParentPermaUrl;
    private readonly NameValueCollection _urlParameters;

    public WxePermaUrlOptions ()
        : this (true, false, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl)
        : this (true, useParentPermaUrl, null)
    {
    }

    public WxePermaUrlOptions (bool useParentPermaUrl, NameValueCollection urlParameters)
        : this (true, useParentPermaUrl, urlParameters)
    {
    }

    private WxePermaUrlOptions (bool usePermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      _usePermaUrl = usePermaUrl;
      _useParentPermaUrl = useParentPermaUrl;
      _urlParameters = urlParameters;
    }

    public bool UsePermaUrl
    {
      get { return _usePermaUrl; }
    }

    public bool UseParentPermaUrl
    {
      get { return _useParentPermaUrl; }
    }

    public NameValueCollection UrlParameters
    {
      get { return _urlParameters; }
    }

    bool INullObject.IsNull
    {
      get { return !_usePermaUrl; }
    }
  }
}