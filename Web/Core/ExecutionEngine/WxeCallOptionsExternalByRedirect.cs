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
  public class WxeCallOptionsExternalByRedirect : WxeCallOptions
  {
    private readonly bool _returnToCaller;
    private readonly NameValueCollection _callerUrlParameters;

    public WxeCallOptionsExternalByRedirect ()
        : this (new WxePermaUrlOptions (false, null), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (NameValueCollection urlParameters)
        : this (new WxePermaUrlOptions (false, urlParameters), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (WxePermaUrlOptions permaUrlOptions, bool returnToCaller, NameValueCollection callerUrlParameters)
        : base (permaUrlOptions)
    {
      _returnToCaller = returnToCaller;
      _callerUrlParameters = callerUrlParameters;
    }

    public override void Dispatch (WxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      executor.ExecuteFunctionExternalByRedirect (function, this);

      throw new WxeCallExternalException();
    }

    public bool ReturnToCaller
    {
      get { return _returnToCaller; }
    }

    public NameValueCollection CallerUrlParameters
    {
      get { return _callerUrlParameters; }
    }
  }
}