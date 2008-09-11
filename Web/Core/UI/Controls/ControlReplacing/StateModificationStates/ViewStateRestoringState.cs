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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.StateModificationStates
{
  public class ViewStateRestoringState:IViewStateModificationState
  {
    private readonly ControlReplacer _replacer;
    private readonly object _viewState;
    private readonly IInternalControlMemberCaller _memberCaller;

    public ViewStateRestoringState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller, object viewState)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      _replacer = replacer;
      _viewState = viewState;
      _memberCaller = memberCaller;
    }

    public void LoadViewState ()
    {
      bool enableViewStateBackup = _replacer.WrappedControl.EnableViewState;
      _replacer.WrappedControl.EnableViewState = true;

      _replacer.State = new ViewStateCompletedState();

      _memberCaller.LoadViewStateRecursive (_replacer, _viewState);
      
      _replacer.WrappedControl.EnableViewState = false;
      _replacer.WrappedControl.Load += delegate { _replacer.WrappedControl.EnableViewState = enableViewStateBackup; };
    }
  }
}