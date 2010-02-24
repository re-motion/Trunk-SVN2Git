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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.SingleView.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="SingleView"/> control in quirks mode.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewRenderer : RendererBase<ISingleView>, ISingleViewRenderer
  {
    public SingleViewRenderer (HttpContextBase context, ISingleView control)
        : base(context, control)
    {
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (!string.IsNullOrEmpty (Control.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClass);
      else if (!string.IsNullOrEmpty (Control.Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.Attributes["class"]);
      else
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopControls (writer);
      RenderView (writer);
      RenderBottomControls (writer);

      writer.RenderEndTag();

      writer.RenderEndTag();
    }

    protected void AddAttributesToRender (HtmlTextWriter writer)
    {
      AddStandardAttributesToRender (writer);

      if (Control.IsDesignMode)
      {
        writer.AddStyleAttribute ("width", "100%");
        writer.AddStyleAttribute ("height", "75%");
      }

      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected virtual void RenderView (HtmlTextWriter writer)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr

      if (Control.IsDesignMode)
        writer.AddStyleAttribute ("border", "solid 1px black");
      Control.ViewStyle.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.ViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewClientID);
      Control.ViewStyle.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.ViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID + "_View_Content");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      //_viewTemplateContainer.RenderControl (writer);
      Control.View.RenderControl (writer);

      writer.RenderEndTag(); // end content div
      writer.RenderEndTag(); // end body div
      writer.RenderEndTag(); // end outer div

      writer.RenderEndTag(); // end td
      writer.RenderEndTag(); // end tr
    }

    protected virtual void RenderTopControls (HtmlTextWriter writer)
    {
      Style style = Control.TopControlsStyle;
      PlaceHolder placeHolder = Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (HtmlTextWriter writer)
    {
      Style style = Control.BottomControlsStyle;
      PlaceHolder placeHolder = Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (HtmlTextWriter writer, Style style, PlaceHolder placeHolder, string cssClass)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
      if (string.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + CssClassEmpty);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (style.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (writer);

      writer.RenderEndTag(); // end content div
      writer.RenderEndTag(); // end outer div

      writer.RenderEndTag(); // end td
      writer.RenderEndTag(); // end tr
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
