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
  /// <summary>
  /// The <see cref="ViewStateClearingAfterParentLoadedState"/> represents the <see cref="ViewStateModificationStateBase.Replacer"/> during a 
  /// page-lifecycle in which the control tree will be replaced with new version, requiring the reset of the view state for this instance as well.
  /// However, the entire <see cref="ViewStateModificationStateBase.Replacer"/> will be added during the load-phase of the
  /// page-lifecycle. This state will be set while the <see cref="ViewStateModificationStateBase.Replacer"/>'s <see cref="Control.LoadViewState"/>
  /// is executing. Executing the <see cref="AddedControl"/> method will transation the <see cref="ViewStateModificationStateBase.Replacer"/> 
  /// into the <see cref="ViewStateCompletedState"/>. 
  /// Executing the <see cref="LoadViewState"/> method will result in a <see cref="NotSupportedException"/> being thrown.
  /// </summary>
  public class ViewStateClearingAfterParentLoadedState : ViewStateClearingStateBase
  {
    public ViewStateClearingAfterParentLoadedState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    public override void LoadViewState (object savedState)
    {
      throw new NotSupportedException();
    }

    public override void AddedControl (Control control, int index, Action<Control, int> baseCall)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("baseCall", baseCall);

      ClearChildState ();

      baseCall (control, index);
    }
  }
}