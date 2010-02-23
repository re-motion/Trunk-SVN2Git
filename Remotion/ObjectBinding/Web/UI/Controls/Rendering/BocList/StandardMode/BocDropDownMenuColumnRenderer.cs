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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocDropDownMenuColumnDefinition"/> columns.
  /// </summary>
  public class BocDropDownMenuColumnRenderer : BocColumnRendererBase<BocDropDownMenuColumnDefinition>
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocDropDownMenuColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain instances of this class.
    /// </remarks>
    public BocDropDownMenuColumnRenderer (HttpContextBase context, IBocList list, BocDropDownMenuColumnDefinition column, CssClassContainer cssClasses)
        : base (context, list, column, cssClasses)
    {
    }

    /// <summary>
    /// Renders a <see cref="DropDownMenu"/> with the options for the current row.
    /// <seealso cref="BocColumnRendererBase{TBocColumnDefinition}.RenderCellContents"/>
    /// </summary>
    /// <remarks>
    /// The menu title is generated from the <see cref="DropDownMenu.TitleText"/> and <see cref="DropDownMenu.TitleText"/> properties of
    /// the column definition in <see cref="BocColumnRendererBase{TBocColumnDefinition}.Column"/>, and populated with the menu items in
    /// the <see cref="IBocList.RowMenus"/> property of <see cref="BocListRendererBase.List"/>.
    /// </remarks>
    protected override void RenderCellContents (
        HtmlTextWriter writer, 
        BocListDataRowRenderEventArgs dataRowRenderEventArgs,
        int rowIndex,
        bool showIcon)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      if (List.RowMenus == null || List.RowMenus.Length < rowIndex || List.RowMenus[rowIndex] == null)
      {
        writer.Write (c_whiteSpace);
        return;
      }

      DropDownMenu dropDownMenu = List.RowMenus[rowIndex].Item3;

      if (List.HasClientScript)
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin div

      dropDownMenu.Enabled = !List.EditModeController.IsRowEditModeActive;

      dropDownMenu.TitleText = Column.MenuTitleText;
      dropDownMenu.TitleIcon = Column.MenuTitleIcon;
      dropDownMenu.RenderControl (writer);

      writer.RenderEndTag(); // End div
    }
  }
}
