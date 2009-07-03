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
    private const string c_dropDownIcon = "DropDownMenuArrow.gif";

    public DropDownMenuRenderer (IHttpContext context, HtmlTextWriter writer, IDropDownMenu control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderTitleDiv();
      RenderList();

      Writer.RenderEndTag();
    }

    private void RenderList ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassList);
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "none");
      Writer.RenderBeginTag (HtmlTextWriterTag.Ul);
      Writer.RenderEndTag();
    }

    private void RenderTitleDiv ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (Control.RenderHeadTitleMethod != null)
        Control.RenderHeadTitleMethod ();
      else
        RenderDefaultTitle();

      Writer.RenderEndTag ();
    }

    private void RenderDefaultTitle ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      if (Control.TitleIcon != null && !string.IsNullOrEmpty (Control.TitleIcon.Url))
        Control.TitleIcon.Render (Writer);

      if (string.IsNullOrEmpty (Control.TitleText))
        Writer.Write ("&nbsp;");
      else
        Writer.Write (Control.TitleText);
      Writer.RenderEndTag();

      Writer.AddStyleAttribute ("vertical-align", "middle");
      Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (IDropDownMenu), ResourceType.Image, c_dropDownIcon);
      Writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      Writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
      Writer.RenderBeginTag (HtmlTextWriterTag.Img);
      Writer.RenderEndTag();
    }

    private void AddAttributesToRender ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddStandardAttributesToRender();
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      if (Control.Enabled)
      {
        string script = Control.GetOpenDropDownMenuEventReference (null);
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
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
  }
}