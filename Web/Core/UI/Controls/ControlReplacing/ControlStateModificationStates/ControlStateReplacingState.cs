/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  public class ControlStateReplacingState:ControlStateModificationStateBase
  {
    private readonly IDictionary _controlState;

    public ControlStateReplacingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller, IDictionary controlState)
        : base(replacer, memberCaller)
    {
      _controlState = controlState;
    }

    public IDictionary ControlState
    {
      get { return _controlState; }
    }

    public override void LoadControlState (object savedState)
    {
      MemberCaller.SetChildControlState (Replacer, _controlState);

      Replacer.ControlStateModificationState = new ControlStateCompletedState (Replacer, MemberCaller);
    }
  }
}