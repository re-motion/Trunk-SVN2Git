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
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.SingleView.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="SingleView"/> control in quirks mode.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewRenderer : RendererBase<ISingleView>, ISingleViewRenderer
  {
    public SingleViewRenderer (IHttpContext context, HtmlTextWriter writer, ISingleView control)
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
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopControls();
      RenderView();
      RenderBottomControls();

      Writer.RenderEndTag();

      Writer.RenderEndTag();
    }

    protected void AddAttributesToRender ()
    {
      AddStandardAttributesToRender();

      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute ("width", "100%");
        Writer.AddStyleAttribute ("height", "75%");
      }

      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected virtual void RenderView ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr

      if (Control.IsDesignMode)
        Writer.AddStyleAttribute ("border", "solid 1px black");
      Control.ViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewClientID);
      Control.ViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID + "_View_Content");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      //_viewTemplateContainer.RenderControl (Writer);
      Control.View.RenderControl (Writer);

      Writer.RenderEndTag (); // end content div
      Writer.RenderEndTag (); // end body div
      Writer.RenderEndTag (); // end outer div

      Writer.RenderEndTag (); // end td
      Writer.RenderEndTag (); // end tr
    }

    protected virtual void RenderTopControls ()
    {
      Style style = Control.TopControlsStyle;
      PlaceHolder placeHolder = Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls ()
    {
      Style style = Control.BottomControlsStyle;
      PlaceHolder placeHolder = Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (Style style, PlaceHolder placeHolder, string cssClass)
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
      if (string.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + CssClassEmpty);
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (style.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (Writer);

      Writer.RenderEndTag (); // end content div
      Writer.RenderEndTag (); // end outer div

      Writer.RenderEndTag (); // end td
      Writer.RenderEndTag (); // end tr
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleView</c>. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "singleView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.ViewStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassView
    {
      get { return "singleViewView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.TopControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassTopControls
    {
      get { return "singleViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBottomControls
    {
      get { return "singleViewBottomControls"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> wrapping the content and the border elements. </summary>
    /// <remarks> 
    ///   <para> Class: <c>body</c>. </para>
    /// </remarks>
    public virtual string CssClassViewBody
    {
      get { return "body"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    public virtual string CssClassContent
    {
      get { return "content"; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>empty</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>td.singleViewTopControls.emtpy</c> or 
    ///     <c>td.singleViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    public virtual string CssClassEmpty
    {
      get { return "empty"; }
    }

    #endregion

    
  }
}
