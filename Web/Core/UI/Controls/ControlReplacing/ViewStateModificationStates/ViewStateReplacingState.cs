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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  public class ViewStateReplacingState : ViewStateModificationStateBase
  {
    private readonly object _viewState;
    private readonly IInternalControlMemberCaller _memberCaller;

    public ViewStateReplacingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller, object viewState)
        : base (replacer)
    {
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _viewState = viewState;
      _memberCaller = memberCaller;
    }

    public override void LoadViewState (object savedState)
    {
      bool enableViewStateBackup = Replacer.WrappedControl.EnableViewState;
      Replacer.WrappedControl.EnableViewState = true;

      Replacer.ViewStateModificationState = new ViewStateLoadingState (Replacer);

      _memberCaller.LoadViewStateRecursive (Replacer, _viewState);
      // Causes a nested call of LoadViewState, this time on ViewStateLoadingState.

      Replacer.WrappedControl.EnableViewState = false;
      Replacer.WrappedControl.Load += delegate { Replacer.WrappedControl.EnableViewState = enableViewStateBackup; };
    }

    //protected override void AddedControl (Control control, int index)
    //{
    //    if (_isViewStateLoaded)
    //    {
    //      object viewStateBackup = ViewStateBackup;
    //      ViewStateBackup = null;
    //      ControlHelper.LoadViewStateRecursive (this, viewStateBackup);

    //      bool enableViewStateBackup = control.EnableViewState;
    //      control.EnableViewState = false;
    //      Controls[0].Load += delegate { Controls[0].EnableViewState = enableViewStateBackup; };
    //    }

    //    base.AddedControl (control, index);
    //}
  }
}