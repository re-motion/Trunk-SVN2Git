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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  /// <summary>
  /// The <see cref="RedirectingToSubFunctionStateParameters"/> type provides all state required to redirect to and return from a sub-function URL.
  /// </summary>
  [Serializable]
  public class RedirectingToSubFunctionStateParameters : ExecutionStateParameters
  {
    private readonly string _destinationUrl;
    private readonly string _resumeUrl;

    public RedirectingToSubFunctionStateParameters (
        WxeFunction subFunction, NameValueCollection postBackCollection, string destinationUrl, string resumeUrl)
        : base (subFunction, postBackCollection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("destinationUrl", destinationUrl);
      ArgumentUtility.CheckNotNullOrEmpty ("resumeUrl", resumeUrl);

      _destinationUrl = destinationUrl;
      _resumeUrl = resumeUrl;
    }

    public string DestinationUrl
    {
      get { return _destinationUrl; }
    }

    public string ResumeUrl
    {
      get { return _resumeUrl; }
    }
  }
}