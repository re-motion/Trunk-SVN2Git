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
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The <see cref="WxeRepostOptions"/> determine if and how the calling page is notified after a <see cref="WxeFunction"/> executed with the 
  /// <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunction"/> or <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunctionNoRepost"/> 
  /// methods has returned to the caller.
  /// </summary>
  [Serializable]
  public class WxeRepostOptions:INullObject
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
      _suppressRepost = false;
    }

    private WxeRepostOptions ()
    {
      _suppressRepost = true;
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
      get { return _suppressRepost; }
    }
  }
}