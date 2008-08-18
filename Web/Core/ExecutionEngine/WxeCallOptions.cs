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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeCallOptions"/> type represents the most generic of options for executing a <see cref="WxeFunction"/>, namely it only controls
  /// whether the useer should be presented with a perma-URL to the <see cref="WxeFunction"/>.
  /// Use the derived types if you require additional control over the <see cref="WxeFunction"/>'s execution.
  /// </summary>
  public class WxeCallOptions
  {
    private readonly WxePermaUrlOptions _permaUrlOptions;

    public WxeCallOptions ()
        : this (WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptions (WxePermaUrlOptions permaUrlOptions)
    {
      ArgumentUtility.CheckNotNull ("permaUrlOptions", permaUrlOptions);

      _permaUrlOptions = permaUrlOptions;
    }

    public virtual void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      executor.ExecuteFunction (function, _permaUrlOptions);
    }

    public WxePermaUrlOptions PermaUrlOptions
    {
      get { return _permaUrlOptions; }
    }
  }
}