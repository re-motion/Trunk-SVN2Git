// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeRepostOptions"/> determine if and how the calling page is notified after a <see cref="WxeFunction"/> executed with the 
  /// <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunction"/> or <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunctionNoRepost"/> 
  /// methods has returned to the caller.
  /// </summary>
  [Serializable]
  public class WxeRepostOptions : INullObject
  {
    public static readonly WxeRepostOptions Null = new WxeRepostOptions();

    private readonly Control _sender;
    private readonly bool _usesEventTarget;
    private readonly bool _suppressRepost;

    public WxeRepostOptions (Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!usesEventTarget && !(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
      {
        throw new ArgumentException (
            "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
      }

      _sender = sender;
      _usesEventTarget = usesEventTarget;
      _suppressRepost = true;
    }

    private WxeRepostOptions ()
    {
      _suppressRepost = false;
    }

    public Control Sender
    {
      get { return _sender; }
    }

    public bool UsesEventTarget
    {
      get { return _usesEventTarget; }
    }

    public bool SuppressRepost
    {
      get { return _suppressRepost; }
    }

    bool INullObject.IsNull
    {
      get { return !_suppressRepost; }
    }
  }
}
