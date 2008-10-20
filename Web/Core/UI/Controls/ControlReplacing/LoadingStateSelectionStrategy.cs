/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  /// <summary>
  /// The <see cref="LoadingStateSelectionStrategy"/> type is used when the state of a <see cref="ControlReplacer"/>'s control tree should be 
  /// loaded during a regular page-lifecycle, i.e. when the view state is to be loaded without modification
  /// </summary>
  public class LoadingStateSelectionStrategy : IStateModificationStrategy
  {
    public IViewStateModificationState CreateViewStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ViewStateLoadingState (replacer, memberCaller);
    }

    public IControlStateModificationState CreateControlStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ControlStateLoadingState (replacer, memberCaller);
    }

    public void LoadControlState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      //NOP
    }

    public void LoadViewState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      //NOP
    }
  }
}