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
