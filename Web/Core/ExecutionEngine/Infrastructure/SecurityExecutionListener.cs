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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class SecurityExecutionListener : IWxeFunctionExecutionListener
  {
    private readonly WxeFunction _function;
    private readonly IWxeFunctionExecutionListener _innerListener;

    public SecurityExecutionListener (WxeFunction function, IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      _function = function;
      _innerListener = innerListener;
    }

    public WxeFunction Function
    {
      get { return _function; }
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }

    /// <summary>
    /// Gets a value indicating whether the object is a "Null Object".
    /// </summary>
    public bool IsNull
    {
      get { return false; }
    }

    /// <summary>Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is invoked (first and subsequent calls).</summary>
    public void OnExecutionPlay (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (!_function.IsExecutionStarted)
      {
        IWxeSecurityAdapter wxeSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter>();
        if (wxeSecurityAdapter != null)
          wxeSecurityAdapter.CheckAccess (_function);
      }

      _innerListener.OnExecutionPlay (context);
    }

    /// <summary>Stop is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is completed successfully.</summary>
    public void OnExecutionStop (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _innerListener.OnExecutionStop (context);
    }

    /// <summary>
    /// Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is exited by a <see cref="ThreadAbortException"/>,
    /// i.e. the execution is paused.
    /// </summary>
    public void OnExecutionPause (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _innerListener.OnExecutionPause (context);
    }

    /// <summary>Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method fails.</summary>
    public void OnExecutionFail (WxeContext context, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _innerListener.OnExecutionFail (context, exception);
    }
  }
}