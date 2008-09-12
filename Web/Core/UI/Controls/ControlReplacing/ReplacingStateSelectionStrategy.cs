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
using System.Collections;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;
using Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  public class ReplacingStateSelectionStrategy : IModificationStateSelectionStrategy
  {
    private readonly IDictionary _controlState;
    private readonly object _viewState;

    public ReplacingStateSelectionStrategy (string serializedState)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serializedState", serializedState);

      var formatter = new LosFormatter ();
      var state = (Pair) formatter.Deserialize (serializedState);

      _controlState = (IDictionary) state.First;
      _viewState = state.Second;
    }

    public IViewStateModificationState CreateViewStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ViewStateReplacingState (replacer, memberCaller, _viewState);
    }

    public IControlStateModificationState CreateControlStateModificationState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      return new ControlStateReplacingState (replacer, memberCaller, _controlState);
    }

    public IDictionary ControlState
    {
      get { return _controlState; }
    }

    public object ViewState
    {
      get { return _viewState; }
    }
  }
}