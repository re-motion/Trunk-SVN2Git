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

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a <see cref="MenuTab"/>.
/// </summary>
public delegate void MenuTabClickEventHandler (object sender, MenuTabClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class MenuTabClickEventArgs: WebTabClickEventArgs
{

  /// <summary> Initializes an instance. </summary>
  public MenuTabClickEventArgs (MenuTab tab)
    : base (tab)
  {
  }

  /// <summary> The <see cref="Command"/> that caused the event. </summary>
  public Command Command
  {
    get { return Tab.Command; }
  }

  /// <summary> The <see cref="MenuTab"/> that was clicked. </summary>
  public new MenuTab Tab
  {
    get { return (MenuTab) base.Tab; }
  }
}

}
