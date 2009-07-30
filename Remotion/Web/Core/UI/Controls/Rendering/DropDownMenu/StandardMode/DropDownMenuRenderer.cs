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

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.StandardMode
{
  /// <summary>
  /// Responsible for rendering <see cref="DropDownMenu"/> controls in standard mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>, IDropDownMenuRenderer
  {
    private const string c_whiteSpace = "&nbsp;";

    public DropDownMenuRenderer (IHttpContext context, HtmlTextWriter writer, IDropDownMenu control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderTitleDiv();

      Writer.RenderEndTag();
    }

    private void RenderTitleDiv ()
    {
      string cssClass = CssClassHead;
      if (!Control.Enabled)
        cssClass += " " + CssClassDisabled;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (Control.RenderHeadTitleMethod != null)
        Control.RenderHeadTitleMethod ();
      else
        RenderDefaultTitle();

      RenderDropdownButton ();

      Writer.RenderEndTag ();
    }

    private void RenderDefaultTitle ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
      Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (Control.TitleIcon != null && !string.IsNullOrEmpty (Control.TitleIcon.Url))
        Control.TitleIcon.Render (Writer);

      if (!string.IsNullOrEmpty (Control.TitleText))
      {
        Writer.Write (Control.TitleText);
        Writer.Write (c_whiteSpace);
      }
      Writer.RenderEndTag ();
    }

    private void RenderDropdownButton ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownButton);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      IconInfo.Spacer.Render (Writer);
      
      Writer.RenderEndTag();
    }

    protected string CssClassDropDownButton
    {
      get { return "DropDownMenuButton"; }
    }

    private void AddAttributesToRender ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddStandardAttributesToRender();
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      if (Control.ControlStyle.Width.IsEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
    }

    protected string CssClassBase
    {
      get { return "DropDownMenuContainer"; }
    }

    protected string CssClassHead
    {
      get { return "DropDownMenuSelect"; }
    }

    protected string CssClassList
    {
      get { return "DropDownMenuOptions"; }
    }

    protected string CssClassDisabled
    {
      get { return "disabled"; }
    }
  }
}