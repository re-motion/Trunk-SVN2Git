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
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  /// <summary>
  /// The <see cref="ClearingStateSelectionStrategy"/> type is used when the state of a <see cref="ControlReplacer"/>'s control tree should be reset.
  /// </summary>
  public class ClearingStateSelectionStrategy:IStateModificationStrategy
  {
    public IViewStateModificationState CreateViewStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ViewStateClearingState (replacer, memberCaller);
    }

    public IControlStateModificationState CreateControlStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ControlStateClearingState (replacer, memberCaller);
    }

    public void LoadControlState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      memberCaller.ClearChildControlState (replacer);
    }

    public void LoadViewState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      bool enableViewStateBackup = replacer.ControlToWrap.EnableViewState;
      replacer.ControlToWrap.EnableViewState = false;
      replacer.ControlToWrap.Load += delegate (object sender, EventArgs args ){ ((Control)sender).EnableViewState = enableViewStateBackup; };
    }
  }
}