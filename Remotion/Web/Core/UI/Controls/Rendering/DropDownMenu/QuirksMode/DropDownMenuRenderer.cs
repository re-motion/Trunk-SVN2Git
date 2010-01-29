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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="DropDownMenu"/> control in quirks mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>, IDropDownMenuRenderer
  {
    private const string c_dropDownIcon = "DropDownMenuArrow.gif";

    public DropDownMenuRenderer (IHttpContext context, HtmlTextWriter writer, IDropDownMenu control)
        : base(context, writer, control)
    {
    }

    public void Render ()
    {
      AddStandardAttributesToRender();
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Menu-Div filling the control's div is required to apply internal css attributes
      //  for position, width and height. This allows the Head and th popup-div to align themselves
      Writer.AddStyleAttribute ("position", "relative");
      Writer.AddAttribute ("id", Control.MenuHeadClientID);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

      RenderHead ();

      Writer.RenderEndTag (); // End Menu-Div
      Writer.RenderEndTag (); // End outer div
    }

    private void RenderHead ()
    {
      //  Head-Div is used to group the title and the button, providing a single point of reference
      //  for the popup-div.
      Writer.AddStyleAttribute ("position", "relative");
      Writer.AddAttribute ("id", Control.ClientID + "_HeadDiv");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
      Writer.AddAttribute ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      Writer.AddAttribute ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.AddStyleAttribute ("display", "inline");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin Drop Down Button table
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderHeadTitle (Writer);
      RenderHeadButton (Writer);

      Writer.RenderEndTag ();
      Writer.RenderEndTag (); // End Drop Down Button table

      Writer.RenderEndTag (); // End Drop Down Head-Div
    }

    private void RenderHeadTitle (HtmlTextWriter writer)
    {
      bool hasHeadTitleContents = true;
      if (Control.RenderHeadTitleMethod == null)
      {
        bool hasTitleText = !string.IsNullOrEmpty (Control.TitleText);
        bool hasTitleIcon = Control.TitleIcon != null && !string.IsNullOrEmpty (Control.TitleIcon.Url);
        hasHeadTitleContents = hasTitleText || hasTitleIcon;

        if (hasHeadTitleContents)
        {
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%"); //"100%");
          writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
          writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

          if (Control.Enabled)
            writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin title tag
          else
          {
            writer.AddStyleAttribute (HtmlTextWriterStyle.Color, "GrayText");
            writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin title tag
          }
          RenderIcon (writer, Control.TitleIcon);
          writer.Write (Control.TitleText);
          writer.RenderEndTag (); // End title tag

          writer.RenderEndTag (); // End td
        }
      }
      else
        Control.RenderHeadTitleMethod();

      if (hasHeadTitleContents)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        writer.AddStyleAttribute ("padding-right", "0.3em");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        writer.RenderEndTag ();
      }
    }

    private void RenderIcon (HtmlTextWriter writer, IconInfo icon)
    {
      if (icon == null || string.IsNullOrEmpty (icon.Url))
        return;

      writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
      if (!icon.Width.IsEmpty && !icon.Height.IsEmpty)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString ());
        writer.AddAttribute (HtmlTextWriterAttribute.Height, icon.Height.ToString ());
      }
      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      writer.AddStyleAttribute ("margin-right", "0.3em");
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag ();
    }

    private void RenderHeadButton (HtmlTextWriter writer)
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.AddStyleAttribute ("text-align", "center");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor

      writer.AddStyleAttribute ("vertical-align", "middle");
      writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IDropDownMenu), ResourceType.Image, ResourceTheme, c_dropDownIcon);
      writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag (); // End img

      writer.RenderEndTag (); // End anchor

      writer.RenderEndTag (); // End td
    }

    protected virtual string CssClassHead
    {
      get { return "dropDownMenuHead"; }
    }

    protected virtual string CssClassHeadFocus
    {
      get { return "dropDownMenuHeadFocus"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
    /// <remarks> Class: <c></c> </remarks>
    protected virtual string CssClassHeadTitle
    {
      get { return "dropDownMenuHeadTitle"; }
    }

    protected virtual string CssClassHeadTitleFocus
    {
      get { return "dropDownMenuHeadTitleFocus"; }
    }

    protected virtual string CssClassHeadButton
    {
      get { return "dropDownMenuHeadButton"; }
    }

    protected virtual string CssClassMenuButtonFocus
    {
      get { return "dropDownMenuButtonFocus"; }
    }

    protected virtual string CssClassPopUp
    {
      get { return "dropDownMenuPopUp"; }
    }

    protected virtual string CssClassItem
    {
      get { return "dropDownMenuItem"; }
    }

    protected virtual string CssClassItemFocus
    {
      get { return "dropDownMenuItemFocus"; }
    }

    protected virtual string CssClassItemTextPane
    {
      get { return "dropDownMenuItemTextPane"; }
    }

    protected virtual string CssClassItemIconPane
    {
      get { return "dropDownMenuItemIconPane"; }
    }
  }
}
