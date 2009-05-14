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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
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
    public BocListMenuBlockRenderer (HtmlTextWriter writer, Controls.BocList list)
        : base (writer, list)
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

      RenderWrappedListMenu (menuBlockItemOffset);
    }

    private void RenderWrappedListMenu (string menuBlockItemOffset)
    {
      if (!List.HasListMenu)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, List.ClientID + "_Boc_ListMenu");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      RenderListMenu (List.ClientID + "_Boc_ListMenu");
      Writer.RenderEndTag();
    }

    private void RenderOptionsMenu (string menuBlockItemOffset)
    {
      if (!List.HasOptionsMenu)
        return;

      if (StringUtility.IsNullOrEmpty (List.OptionsTitle))
        List.OptionsMenu.TitleText = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.OptionsTitle);
      else
        List.OptionsMenu.TitleText = List.OptionsTitle;

      List.OptionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
      List.OptionsMenu.Enabled = !List.IsRowEditModeActive;
      List.OptionsMenu.IsReadOnly = List.IsReadOnly;
      List.OptionsMenu.RenderControl (Writer);
    }

    private void RenderAvailableViewsList (string menuBlockItemOffset)
    {
      if (!List.HasAvailableViewsList)
        return;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassAvailableViewsListLabel);

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
      if (ControlHelper.IsDesignMode ((Control) List))
        List.AvailableViewsList.Width = Unit.Point (c_designModeAvailableViewsListWidthInPoints);
      List.AvailableViewsList.Enabled = ! List.IsRowEditModeActive && ! List.IsListEditModeActive;
      List.AvailableViewsList.CssClass = List.CssClassAvailableViewsListDropDownList;
      List.AvailableViewsList.RenderControl (Writer);
      Writer.RenderEndTag();
    }

    private void RenderListMenu (string menuID)
    {
      if (!List.HasClientScript)
        return;

      WebMenuItem[] groupedListMenuItems = List.ListMenuItems.GroupMenuItems (false);

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);
      bool isFirstItem = true;
      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        // HACK: Required since ListMenuItems are not added to a ListMenu's WebMenuItemCollection.
        currentItem.OwnerControl = List;
        if (!currentItem.EvaluateVisible())
          continue;

        bool isLastItem = (idxItems == (groupedListMenuItems.Length - 1));
        bool isFirstCategoryItem = (isFirstItem || (groupedListMenuItems[idxItems - 1].Category != currentItem.Category));
        bool isLastCategoryItem = (isLastItem || (groupedListMenuItems[idxItems + 1].Category != currentItem.Category));
        bool hasAlwaysLineBreaks = (List.ListMenuLineBreaks == ListMenuLineBreaks.All);
        bool hasNoLineBreaks = (List.ListMenuLineBreaks == ListMenuLineBreaks.None);

        if (hasAlwaysLineBreaks || isFirstCategoryItem || (hasNoLineBreaks && isFirstItem))
        {
          Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, "contentMenuRow");
          Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        }
        RenderListMenuItem (currentItem, menuID, List.ListMenuItems.IndexOf (currentItem));
        if (hasAlwaysLineBreaks || isLastCategoryItem || (hasNoLineBreaks && isLastItem))
        {
          Writer.RenderEndTag();
          Writer.RenderEndTag();
        }

        if (isFirstItem)
          isFirstItem = false;
      }
      Writer.RenderEndTag();
    }

    private void RenderListMenuItem (WebMenuItem menuItem, string menuID, int index)
    {
      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, menuID + "_" + index);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.RenderBeginTag (HtmlTextWriterTag.A);
      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Src, UrlUtility.ResolveUrl (menuItem.Icon.Url));
        Writer.AddStyleAttribute ("vertical-align", "middle");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        Writer.RenderEndTag();
        if (showText)
          Writer.Write (c_whiteSpace);
      }
      if (showText)
        Writer.Write (menuItem.Text); // Do not HTML encode.
      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }
  }
}