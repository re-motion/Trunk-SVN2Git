// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  /// Use the <see cref="WxeCallOptionsExternal"/> type if you whish to execute a <see cref="WxeFunction"/> as a new root function, 
  /// typically in a new window. The <see cref="WxeFunction"/> will be initialized on the server and then opened via a Javascript call.
  /// </summary>
  [Serializable]
  public class WxeCallOptionsExternal : WxeCallOptions
  {
    private readonly string _target;
    private readonly string _features;
    private readonly bool _returningPostback;

    public WxeCallOptionsExternal (string target)
        : this (target, null, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features)
        : this (target, features, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback)
        : this (target, features, returningPostback, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features, bool returningPostback, WxePermaUrlOptions permaUrlOptions)
        : base (permaUrlOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("target", target);

      _target = target;
      _features = features;
      _returningPostback = returningPostback;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, WxeCallArguments handler)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("handler", handler);

      executor.ExecuteFunctionExternal (function, handler.Sender, this);

      throw new WxeCallExternalException();
    }

    public string Target
    {
      get { return _target; }
    }

    public string Features
    {
      get { return _features; }
    }

    public bool ReturningPostback
    {
      get { return _returningPostback; }
    }
  }
}
