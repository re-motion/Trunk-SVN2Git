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

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="PreparingRedirectToSubFunctionStateParameters"/> group the parameters necessary for setting up the the redirect to a sub-function URL.
  /// </summary>
  [Serializable]
  public class PreparingRedirectToSubFunctionStateParameters : ExecutionStateParameters
  {
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public PreparingRedirectToSubFunctionStateParameters (
        WxeFunction subFunction, NameValueCollection postBackCollection, WxePermaUrlOptions permaUrlOptions)
        : base(subFunction, postBackCollection)
    {
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);
      _permaUrlOptions = permaUrlOptions;
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}