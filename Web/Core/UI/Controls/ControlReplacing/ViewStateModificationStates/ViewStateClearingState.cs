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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  public class ViewStateClearingState : ViewStateModificationStateBase
  {
    private bool _isViewStateLoaded;

    public ViewStateClearingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    public override void LoadViewState (object savedState)
    {
      if (Replacer.WrappedControl != null)
        ClearViewState ();

      _isViewStateLoaded = true;
    }

    public override void AddedControlBegin ()
    {
      if (_isViewStateLoaded)
        ClearViewState ();
    }

    public override void AddedControlCompleted ()
    {
    }

    private void ClearViewState ()
    {
      var wrappedControl = Replacer.WrappedControl;
      Assertion.IsNotNull (wrappedControl);

      bool enableViewStateBackup = wrappedControl.EnableViewState;
      wrappedControl.EnableViewState = false;
      wrappedControl.Load += delegate { wrappedControl.EnableViewState = enableViewStateBackup; };
       
      Replacer.ViewStateModificationState = new ViewStateCompletedState (Replacer, MemberCaller);
    }
  }
}