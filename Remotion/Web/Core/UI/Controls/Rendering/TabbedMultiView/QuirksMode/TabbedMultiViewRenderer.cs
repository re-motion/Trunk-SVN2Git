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

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="TabbedMultiView"/> controls in quirks mode.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewRenderer : RendererBase<ITabbedMultiView>, ITabbedMultiViewRenderer
  {
    public TabbedMultiViewRenderer (IHttpContext context, HtmlTextWriter writer, ITabbedMultiView control)
        : base(context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (!string.IsNullOrEmpty (Control.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClass);
      else if (!string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.Attributes["class"]);
      else
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassBase);

      Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTopControls();
      Writer.RenderEndTag ();

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTabStrip();
      Writer.RenderEndTag ();

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderActiveView();
      Writer.RenderEndTag ();

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderBottomControls();
      Writer.RenderEndTag ();

      Writer.RenderEndTag ();
      Writer.RenderEndTag();
    }

    protected void AddAttributesToRender ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddStandardAttributesToRender ();
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
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassTabStrip);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Control.TabStrip.CssClass = Control.CssClassTabStrip;
      Control.TabStrip.RenderControl (Writer);

      Writer.RenderEndTag (); // end td
    }

    protected virtual void RenderActiveView ()
    {
      if (Control.IsDesignMode)
        Writer.AddStyleAttribute ("border", "solid 1px black");
      Control.ActiveViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassActiveView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID);
      Control.ActiveViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassActiveView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassViewBody);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID + "_Content");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      var view = Control.GetActiveView ();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = view.Controls[i];
          control.RenderControl (Writer);
        }
      }

      Writer.RenderEndTag (); // end content div
      Writer.RenderEndTag (); // end body div
      Writer.RenderEndTag (); // end outer div

      Writer.RenderEndTag (); // end td
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

    private void RenderPlaceHolder (Style style, PlaceHolder placeHolder, string cssClass)
    {
      if (string.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + Control.CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + Control.CssClassEmpty);
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (style.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (Writer);

      Writer.RenderEndTag (); // end content div
      Writer.RenderEndTag (); // end outer div

      Writer.RenderEndTag (); // end td
    }
  }
}