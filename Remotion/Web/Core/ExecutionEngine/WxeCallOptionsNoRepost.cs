// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxeCallOptionsNoRepost"/> type if you whish to execute a <see cref="WxeFunction"/> as a sub function and suppressing 
  /// the re-post to the postback-handler after the execution has returned to the caller function.
  /// </summary>
  [Serializable]
  public class WxeCallOptionsNoRepost : WxeCallOptions
  {
    private readonly bool? _usesEventTarget;

    public WxeCallOptionsNoRepost ()
        : this (null, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget)
        : this (usesEventTarget, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (WxePermaUrlOptions permaUrlOptions)
        : this (null, permaUrlOptions)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget, WxePermaUrlOptions permaUrlOptions)
        : base (permaUrlOptions)
    {
      _usesEventTarget = usesEventTarget;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("handler", handler);

      executor.ExecuteFunctionNoRepost (function, handler.Sender, this);
    }

    public bool? UsesEventTarget
    {
      get { return _usesEventTarget; }
    }
  }
}
