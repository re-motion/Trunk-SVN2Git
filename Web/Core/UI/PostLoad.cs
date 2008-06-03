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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{

/// <summary>
///   Calls <see cref="ISupportsPostLoadControl.OnPostLoad"/> on all controls that support the interface.
/// </summary>
/// <remarks>
///   Children are called after their parents.
/// </remarks>
public class PostLoadInvoker
{
  public static void InvokePostLoad (Control control)
  {
    if (control is ISupportsPostLoadControl)
      ((ISupportsPostLoadControl)control).OnPostLoad ();

    ControlCollection controls = control.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control childControl = controls[i];
      InvokePostLoad (childControl);
    }
  }

  private Control _control;
  private bool _invoked;

  public PostLoadInvoker (Control control)
  {
    _control = control;
    _invoked = false;
  }

  public void EnsurePostLoadInvoked ()
  {
    if (! _invoked)
    {
      InvokePostLoad (_control);
      _invoked = true;
    }
  }

}

}
