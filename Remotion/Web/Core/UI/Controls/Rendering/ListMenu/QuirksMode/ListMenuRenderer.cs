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
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.ListMenu.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="ListMenu"/> control in quirks mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  public class ListMenuRenderer : RendererBase<IListMenu>, IListMenuRenderer
  {
    protected const string c_whiteSpace = "&nbsp;";

    public ListMenuRenderer (IHttpContext context, HtmlTextWriter writer, IListMenu control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      WebMenuItem[] groupedListMenuItems = Control.MenuItems.GroupMenuItems (false);

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);
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
          Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuRow");
          Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        }
        RenderListMenuItem (currentItem, Control.ClientID, Control.MenuItems.IndexOf (currentItem));
        if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isLastCategoryItem) || isLastItem)
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
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuItem");
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (!menuItem.IsDisabled)
      {
        menuItem.Command.RenderBegin (
            Writer, Control.Page.ClientScript.GetPostBackClientHyperlink (Control, index.ToString ()), new[] { index.ToString () }, "", null);
      }
      else
      {
        Writer.RenderBeginTag (HtmlTextWriterTag.A);
      }

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Src, UrlUtility.ResolveUrl (menuItem.Icon.Url));
        Writer.AddStyleAttribute ("vertical-align", "middle");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        Writer.RenderBeginTag (HtmlTextWriterTag.Img);
        Writer.RenderEndTag ();
        if (showText)
          Writer.Write (c_whiteSpace);
      }
      if (showText)
        Writer.Write (menuItem.Text); // Do not HTML encode.
      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
    }
  }
}