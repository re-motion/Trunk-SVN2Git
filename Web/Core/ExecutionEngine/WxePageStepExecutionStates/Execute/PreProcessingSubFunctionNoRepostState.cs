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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.Execute
{
  [Serializable]
  public sealed class PreProcessingSubFunctionNoRepostState : PreProcessingSubFunctionStateBase<PreProcessingSubFunctionStateParameters>
  {
    private readonly Control _sender;
    private readonly bool _usesEventTarget;

    public PreProcessingSubFunctionNoRepostState (
        IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters, Control sender, bool usesEventTarget)
        : base (executionStateContext, parameters)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!usesEventTarget && !(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
      {
        throw new ArgumentException (
            "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
      }

      _sender = sender;
      _usesEventTarget = usesEventTarget;
    }

    public Control Sender
    {
      get { return _sender; }
    }

    public bool UsesEventTarget
    {
      get { return _usesEventTarget; }
    }

    protected override NameValueCollection BackupPostBackCollection ()
    {
      var postBackCollection = base.BackupPostBackCollection();

      if (_usesEventTarget)
      {
        postBackCollection.Remove (ControlHelper.PostEventSourceID);
        postBackCollection.Remove (ControlHelper.PostEventArgumentID);
      }
      else
        postBackCollection.Remove (_sender.UniqueID);

      return postBackCollection;
    }
  }
}