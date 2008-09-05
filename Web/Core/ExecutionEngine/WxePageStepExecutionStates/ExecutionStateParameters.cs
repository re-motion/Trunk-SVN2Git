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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  [Serializable]
  public class ExecutionStateParameters : IExecutionStateParameters
  {
    private readonly WxeFunction _subFunction;
    private readonly NameValueCollection _postBackCollection;

    public ExecutionStateParameters (WxeFunction subFunction, NameValueCollection postBackCollection)
    {
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("postBackCollection", postBackCollection);

      _subFunction = subFunction;
      _postBackCollection = postBackCollection;
    }

    public WxeFunction SubFunction
    {
      get { return _subFunction; }
    }

    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }
  }
}