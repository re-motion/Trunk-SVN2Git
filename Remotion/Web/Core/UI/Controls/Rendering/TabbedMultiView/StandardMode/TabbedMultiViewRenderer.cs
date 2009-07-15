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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  /// <summary>
  /// Responsible for rendering <see cref="TabbedMultiView"/> controls in standard mode.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewRenderer : RendererBase<ITabbedMultiView>, ITabbedMultiViewRenderer
  {
    public TabbedMultiViewRenderer (IHttpContext context, HtmlTextWriter writer, ITabbedMultiView control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderTopControls();
      RenderTabStrip();
      RenderActiveView();
      RenderBottomControls();

      Writer.RenderEndTag();
      Writer.RenderEndTag ();
    }

    protected void AddAttributesToRender ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddStandardAttributesToRender();
      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute ("width", "100%");
        Writer.AddStyleAttribute ("height", "75%");
      }
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassBase);
    }

    protected virtual void RenderTabStrip ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.TabStripContainerClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassTabStrip);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Control.TabStrip.CssClass = Control.CssClassTabStrip;
      Control.TabStrip.RenderControl (Writer);

      Writer.RenderEndTag();
    }

    protected virtual void RenderActiveView ()
    {
      if (Control.IsDesignMode)
        Writer.AddStyleAttribute ("border", "solid 1px black");

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID);
      Control.ActiveViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassActiveView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      var view = Control.GetActiveView();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = view.Controls[i];
          control.RenderControl (Writer);
        }
      }

      Writer.RenderEndTag();
    }

    protected virtual void RenderTopControls ()
    {
      Style style = Control.TopControlsStyle;
      PlaceHolder placeHolder = Control.TopControl;
      string cssClass = Control.CssClassTopControls;
      RenderPlaceHolder (style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls ()
    {
      Style style = Control.BottomControlsStyle;
      PlaceHolder placeHolder = Control.BottomControl;
      string cssClass = Control.CssClassBottomControls;
      RenderPlaceHolder (style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (Style style, PlaceHolder placeHolder, string defaultCssClass)
    {
      string cssClass = defaultCssClass;
      if (!string.IsNullOrEmpty (style.CssClass))
        cssClass = style.CssClass;

      if (placeHolder.Controls.Count == 0)
        cssClass += " " + Control.CssClassEmpty;

      string backupCssClass = style.CssClass;
      style.CssClass = cssClass;
      style.AddAttributesToRender (Writer);
      style.CssClass = backupCssClass;

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      placeHolder.RenderControl (Writer);

      Writer.RenderEndTag();
    }
  }
}