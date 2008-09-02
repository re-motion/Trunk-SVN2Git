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
  /// The <see cref="WxeCallArgumentsBase"/> type is the default implementation of <see cref="IWxeCallArguments"/> and acts as the base type for
  /// the <see cref="WxeCallArguments"/> and <see cref="WxePermaUrlCallArguments"/> types.
  /// </summary>
  /// <remarks>
  /// <note type="inotes">Override the <see cref="Dispatch"/> method to control the execution of the <see cref="WxeFunction"/>.</note>
  /// </remarks>
  [Serializable]
  public class WxeCallArgumentsBase : IWxeCallArguments
  {
    private readonly WxeCallOptions _options;

    protected internal WxeCallArgumentsBase (WxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull ("options", options);

      _options = options;
    }

    public WxeCallOptions Options
    {
      get { return _options; }
    }

    protected virtual void Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);

      _options.Dispatch (executor, function, null);
    }

    void IWxeCallArguments.Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      Dispatch (executor, function);
    }
  }
}