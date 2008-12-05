// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
