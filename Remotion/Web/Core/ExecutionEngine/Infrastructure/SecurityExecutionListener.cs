// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
