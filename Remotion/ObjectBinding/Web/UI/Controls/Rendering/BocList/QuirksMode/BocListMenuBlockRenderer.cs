// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  /// <summary>
  /// Responsible for rendering the menu block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListMenuBlockRenderer : BocListRendererBase, IBocListMenuBlockRenderer
  {
    protected const string c_defaultMenuBlockItemOffset = "5pt";
    protected const int c_designModeAvailableViewsListWidthInPoints = 40;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain an instance of this class.
    /// </remarks>
    public BocListMenuBlockRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, CssClassContainer cssClasses)
        : base (context, writer, list, cssClasses)
    {
    }

    /// <summary> Renders the menu block of the control. </summary>
    /// <remarks> Contains the drop down list for selcting a column configuration and the options menu.  </remarks> 
    public void Render ()
    {
      string menuBlockItemOffset = c_defaultMenuBlockItemOffset;
      if (! List.MenuBlockItemOffset.IsEmpty)
        menuBlockItemOffset = List.MenuBlockItemOffset.ToString();

      RenderAvailableViewsList (menuBlockItemOffset);

      RenderOptionsMenu (menuBlockItemOffset);

      RenderListMenu (menuBlockItemOffset);
    }

    private void RenderListMenu (string menuBlockItemOffset)
    {
      if (!List.HasListMenu)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, List.ClientID + "_Boc_ListMenu");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      Control.ListMenu.RenderControl (Writer);
      Writer.RenderEndTag();
    }

    private void RenderOptionsMenu (string menuBlockItemOffset)
    {
      if (!List.HasOptionsMenu)
        return;

      List.OptionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
      List.OptionsMenu.RenderControl (Writer);
    }

    private void RenderAvailableViewsList (string menuBlockItemOffset)
    {
      if (!List.HasAvailableViewsList)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.AvailableViewsListLabel);

      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string availableViewsListTitle;
      if (StringUtility.IsNullOrEmpty (List.AvailableViewsListTitle))
        availableViewsListTitle = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.AvailableViewsListTitle);
      else
        availableViewsListTitle = List.AvailableViewsListTitle;
      // Do not HTML encode.
      Writer.Write (availableViewsListTitle);
      Writer.RenderEndTag();

      Writer.Write (c_whiteSpace);
      if (List.IsDesignMode)
        List.AvailableViewsList.Width = Unit.Point (c_designModeAvailableViewsListWidthInPoints);
      List.AvailableViewsList.Enabled = !List.EditModeController.IsRowEditModeActive && !List.EditModeController.IsListEditModeActive;
      List.AvailableViewsList.CssClass = CssClasses.AvailableViewsListDropDownList;
      List.AvailableViewsList.RenderControl (Writer);
      Writer.RenderEndTag();
    }
  }
}