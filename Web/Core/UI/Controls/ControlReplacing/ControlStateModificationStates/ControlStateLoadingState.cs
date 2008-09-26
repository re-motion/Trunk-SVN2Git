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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  /// <summary>
  /// The <see cref="ControlStateLoadingState"/> represents the <see cref="ControlStateModificationStateBase.Replacer"/> during a regular page-lifecycle, 
  /// i.e. when the control state is to be loaded without modification. Executing the <see cref="LoadControlState"/> method will transation the 
  /// <see cref="ControlStateModificationStateBase.Replacer"/> into the <see cref="ControlStateCompletedState"/>.
  /// </summary>
  public class ControlStateLoadingState : ControlStateModificationStateBase
  {
    public ControlStateLoadingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    public override void LoadControlState (object savedState)
    {
      Replacer.ControlStateModificationState = new ControlStateCompletedState (Replacer, MemberCaller);
    }
  }
}