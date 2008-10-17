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

namespace Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  public class ControlStateNullModificationState : IControlStateModificationState
  {
    public ControlStateNullModificationState ()
    {
    }

    /// <summary>
    /// This method should be invoked by the control's <see cref="Control.LoadControlState"/> method.
    /// </summary>
    public void LoadControlState (object savedState)
    {
    }

    /// <summary>
    /// This method should be invoked by the control's <see cref="Control.AddedControl"/> method and acts as a decorator for the <paramref name="baseCall"/>.
    /// </summary>
    public void AddedControl (Control control, int index, Action<Control, int> baseCall)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("baseCall", baseCall);

      baseCall (control, index);
    }
  }
}