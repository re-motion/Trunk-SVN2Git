// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView
{
  /// <summary>
  /// Exposes <see cref="TabbedMultiView"/> properties relevant to rendering.
  /// </summary>
  public interface ITabbedMultiView : IStyledControl
  {
    bool IsDesignMode { get; }
    string ActiveViewClientID { get; }
    Style ActiveViewStyle { get; }
    Style TopControlsStyle { get; }
    PlaceHolder TopControl { get; }
    Style BottomControlsStyle { get; }
    PlaceHolder BottomControl { get; }
    IWebTabStrip TabStrip { get; }
    string ActiveViewContentClientID { get; }
    string WrapperClientID { get; }
    Control GetActiveView ();
  }
}
