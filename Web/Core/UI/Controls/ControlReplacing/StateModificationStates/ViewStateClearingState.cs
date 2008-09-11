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

namespace Remotion.Web.UI.Controls.ControlReplacing.StateModificationStates
{
  public class ViewStateClearingState : IViewStateModificationState
  {
    private readonly ControlReplacer _replacer;

    public ViewStateClearingState (ControlReplacer replacer)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);

      _replacer = replacer;
    }

    public void LoadViewState ()
    {
      bool enableViewStateBackup = _replacer.WrappedControl.EnableViewState;
      _replacer.WrappedControl.EnableViewState = false;
      _replacer.WrappedControl.Load += delegate { _replacer.WrappedControl.EnableViewState = enableViewStateBackup; };

      _replacer.State = new ViewStateCompletedState();
    }

    public ControlReplacer Replacer
    {
      get { return _replacer; }
    }
  }
}