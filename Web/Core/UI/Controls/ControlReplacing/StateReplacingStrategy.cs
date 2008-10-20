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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  /// <summary>
  /// The <see cref="StateReplacingStrategy"/> type is used when the state of a <see cref="ControlReplacer"/>'s control tree should be 
  /// restored to reflect a previously used state.
  /// </summary>
  public class StateReplacingStrategy : IStateModificationStrategy
  {
    private readonly IDictionary _controlState;
    private readonly object _viewState;

    public StateReplacingStrategy (string serializedState)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("serializedState", serializedState);

      var formatter = new LosFormatter ();
      var state = (Pair) formatter.Deserialize (serializedState);

      _controlState = (IDictionary) state.First;
      _viewState = state.Second;
    }

    public IDictionary ControlState
    {
      get { return _controlState; }
    }

    public object ViewState
    {
      get { return _viewState; }
    }

    public void LoadControlState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      memberCaller.SetChildControlState (replacer, _controlState);
    }

    public void LoadViewState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      memberCaller.LoadViewStateRecursive (replacer, _viewState);
    }
  }
}