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
  /// <summary>
  /// The <see cref="WxeReturnOptions"/> determine whether a <see cref="WxeFunction"/> executed with the 
  /// <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunctionExternalByRedirect"/> method will return to it's calling 
  /// <see cref="WxeFunction"/>. Use the <see cref="Null"/> value if you do not wish to return to the caller.
  /// </summary>
  [Serializable]
  public sealed class WxeReturnOptions : INullObject
  {
    public static readonly WxeReturnOptions Null = new WxeReturnOptions (false, null);

    private readonly bool _isReturning;
    private readonly NameValueCollection _callerUrlParameters;

    public WxeReturnOptions ()
      : this (true, new NameValueCollection())
    {
    }

    public WxeReturnOptions (NameValueCollection callerUrlParameters)
      : this (true, ArgumentUtility.CheckNotNull ("callerUrlParameters", callerUrlParameters))
    {      
    }

    private WxeReturnOptions (bool isReturning, NameValueCollection callerUrlParameters)
    {
      _isReturning = isReturning;
      _callerUrlParameters = callerUrlParameters;
    }

    public bool IsReturning
    {
      get { return _isReturning; }
    }

    public NameValueCollection CallerUrlParameters
    {
      get { return _callerUrlParameters; }
    }

    bool INullObject.IsNull
    {
      get { return !_isReturning; }
    }
  }
}