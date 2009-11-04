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

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu
{
  /// <summary>
  /// Exposes <see cref="DropDownMenu"/> properties relevant to rendering.
  /// </summary>
  public interface IDropDownMenu : IStyledControl
  {
    bool Enabled { get; }
    bool EnableGrouping { get; }
    WebMenuItemCollection MenuItems { get; }
    bool IsReadOnly { get; }
    bool IsDesignMode { get; }
    Action RenderHeadTitleMethod { get; }
    IconInfo TitleIcon { get; }
    string TitleText { get; }
    Unit Width { get; }
    string GetSelectionCount { get; }
    CssStyleCollection Style { get; }
    string MenuHeadClientID { get; }
    MenuMode Mode { get; }
    string GetBindOpenEventScript (string elementReference, string menuIDReference, bool moveToMousePosition);
  }
}
