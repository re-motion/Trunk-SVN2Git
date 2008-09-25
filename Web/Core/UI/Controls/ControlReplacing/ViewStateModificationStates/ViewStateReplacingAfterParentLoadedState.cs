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
  public class ViewStateReplacingAfterParentLoadedState : ViewStateModificationStateBase
  {
    private readonly object _viewState;

    public ViewStateReplacingAfterParentLoadedState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller, object viewState)
        : base (replacer, memberCaller)
    {
      _viewState = viewState;
    }

    public object ViewState
    {
      get { return _viewState; }
    }

    public override void LoadViewState (object savedState)
    {
      throw new NotSupportedException();
    }

    public override void AddedControl (Control control, int index, Action<Control, int> baseCall)
    {
      ArgumentUtility.CheckNotNull ("baseCall", baseCall);

      bool enableViewStateBackup = control.EnableViewState;
      control.EnableViewState = false;

      baseCall (control, index);

      control.EnableViewState = enableViewStateBackup;

      Replacer.ViewStateModificationState = new ViewStateLoadingState (Replacer, MemberCaller);

      MemberCaller.LoadViewStateRecursive (Replacer, _viewState);
      // Causes a nested call of LoadViewState, this time on ViewStateLoadingState.
    }
  }
}