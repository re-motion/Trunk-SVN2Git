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
  public class WxePermaUrlOptions : INullObject
  {
    private sealed class NullPermaUrlOptions : WxePermaUrlOptions, INullObject
    {
      public NullPermaUrlOptions ()
        : base (false, null)
      {
      }

      public override bool UsePermaUrl
      {
        get { return false; }
      }

      bool INullObject.IsNull
      {
        get { return true; }
      }
    }

    public static WxePermaUrlOptions Null = new NullPermaUrlOptions();

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

    public virtual bool UsePermaUrl
    {
      get { return true; }
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
      get { return false; }
    }
  }
}