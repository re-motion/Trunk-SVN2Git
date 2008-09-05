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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [Serializable]
  public class PreparingSubFunctionStateParameters : ExecutionStateParameters
  {
    private readonly WxePermaUrlOptions _permaUrlOptions;
    private readonly WxeReturnOptions _returnOptions;

    public PreparingSubFunctionStateParameters (WxeFunction subFunction, NameValueCollection postBackCollection, WxePermaUrlOptions permaUrlOptions, WxeReturnOptions returnOptions)
        : base(subFunction, postBackCollection)
    {
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);
    
      _permaUrlOptions = permaUrlOptions;
      _returnOptions = returnOptions;
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }

    public WxeReturnOptions ReturnOptions
    {
      get { return _returnOptions; }
    }
  }
}