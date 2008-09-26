/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  /// <summary>
  /// The <see cref="ControlStateClearingState"/> represents the <see cref="ControlStateModificationStateBase.Replacer"/> during a page-lifecycle in which 
  /// the control tree will be replaced with new version, requiring the reset of the view state for this instance as well.
  /// Executing the <see cref="LoadControlState"/> method will transation the <see cref="ControlStateModificationStateBase.Replacer"/> into the 
  /// <see cref="ControlStateCompletedState"/> if the <see cref="ControlStateModificationStateBase.Replacer"/> was initialized along with the rest of the 
  /// page's controls and the <see cref="ControlStateClearingAfterParentLoadedState"/> if the <see cref="ControlStateModificationStateBase.Replacer"/>
  /// was initialized later in the page-lifecycle, i.e. during the loading-phase.
  /// </summary>
  public class ControlStateClearingState : ControlStateClearingStateBase
  {
    public ControlStateClearingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    public override void LoadControlState (object savedState)
    {
      if (Replacer.WrappedControl != null)
        ClearChildState();
      else
        Replacer.ControlStateModificationState = new ControlStateClearingAfterParentLoadedState (Replacer, MemberCaller);
    }
  }
}