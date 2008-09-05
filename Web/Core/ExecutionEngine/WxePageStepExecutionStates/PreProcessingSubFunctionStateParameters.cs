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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  [Serializable]
  public class PreProcessingSubFunctionStateParameters : IExecutionStateParameters
  {
    private readonly IWxePage _page;
    private readonly WxeFunction _subFunction;
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public PreProcessingSubFunctionStateParameters (
        IWxePage page, WxeFunction subFunction, WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("page", page);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      _page = page;
      _subFunction = subFunction;
      _permaUrlOptions = permaUrlOptions;
    }

    public IWxePage Page
    {
      get { return _page; }
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}