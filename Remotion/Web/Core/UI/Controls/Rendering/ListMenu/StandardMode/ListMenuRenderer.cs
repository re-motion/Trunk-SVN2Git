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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.ListMenu.StandardMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="ListMenu"/> control in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuRenderer : RendererBase<IListMenu>, IListMenuRenderer
  {
    protected const string c_whiteSpace = "&nbsp;";

    public ListMenuRenderer (HttpContextBase context, IListMenu control)
        : base (context, control)
    {
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      WebMenuItem[] groupedListMenuItems = Control.MenuItems.GroupMenuItems (false);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);
      bool isFirstItem = true;
      for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      {
        WebMenuItem currentItem = groupedListMenuItems[idxItems];
        if (!currentItem.EvaluateVisible())
          continue;

        bool isLastItem = (idxItems == (groupedListMenuItems.Length - 1));
        bool isFirstCategoryItem = (isFirstItem || (groupedListMenuItems[idxItems - 1].Category != currentItem.Category));
        bool isLastCategoryItem = (isLastItem || (groupedListMenuItems[idxItems + 1].Category != currentItem.Category));
        bool hasAlwaysLineBreaks = (Control.LineBreaks == ListMenuLineBreaks.All);
        bool hasCategoryLineBreaks = (Control.LineBreaks == ListMenuLineBreaks.BetweenGroups);

        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isFirstCategoryItem) || isFirstItem)
        {
          writer.RenderBeginTag (HtmlTextWriterTag.Tr);
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
          writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuRow");
          writer.RenderBeginTag (HtmlTextWriterTag.Td);
        }
        RenderListMenuItem (writer, currentItem, Control.ClientID, Control.MenuItems.IndexOf (currentItem));
        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isLastCategoryItem) || isLastItem)
        {
          writer.RenderEndTag();
          writer.RenderEndTag();
        }

        if (isFirstItem)
          isFirstItem = false;
      }
      writer.RenderEndTag();
    }

    private void RenderListMenuItem (HtmlTextWriter writer, WebMenuItem menuItem, string menuID, int index)
    {
      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      writer.AddAttribute (HtmlTextWriterAttribute.Id, menuID + "_" + index);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuItem");
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (!menuItem.IsDisabled)
      {
        menuItem.Command.RenderBegin (
            writer, Control.Page.ClientScript.GetPostBackClientHyperlink (Control, index.ToString()), new[] { index.ToString() }, "", null);
      }
      else
      {
        writer.RenderBeginTag (HtmlTextWriterTag.A);
      }

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, UrlUtility.ResolveUrl (menuItem.Icon.Url));
        writer.AddAttribute (HtmlTextWriterAttribute.Alt, StringUtility.NullToEmpty (menuItem.Icon.AlternateText));
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
        if (showText)
          writer.Write (c_whiteSpace);
      }
      if (showText)
        writer.Write (menuItem.Text); // Do not HTML encode.
      writer.RenderEndTag();
      writer.RenderEndTag();
    }
  }
}
