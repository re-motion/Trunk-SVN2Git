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

namespace Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  /// <summary>
  /// The <see cref="IViewStateModificationState"/> interface defines a state-pattern for loading the view state of a control tree owned by a 
  /// <see cref="ControlReplacer"/> object.
  /// </summary>
  public interface IViewStateModificationState
  {
    /// <summary>
    /// This method should be invoked by the control's <see cref="Control.LoadViewState"/> method.
    /// </summary>
    void LoadViewState (object savedState);

    /// <summary>
    /// This method should be invoked by the control's <see cref="Control.AddedControl"/> method and acts as a decorator for the <paramref name="baseCall"/>.
    /// </summary>
    void AddedControl (Control control, int index, Action<Control, int> baseCall);
  }
}