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

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.WrapperClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassWrapper);
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
      AddStandardAttributesToRender();
      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute ("width", "100%");
        Writer.AddStyleAttribute ("height", "75%");
      }
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected virtual void RenderTabStrip ()
    {
      Control.TabStrip.CssClass = CssClassTabStrip;
      Control.TabStrip.RenderControl (Writer);
    }

    protected virtual void RenderActiveView ()
    {
      if (Control.IsDesignMode)
        Writer.AddStyleAttribute ("border", "solid 1px black");

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID);
      Control.ActiveViewStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewContentClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContentBorder);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
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
      Writer.RenderEndTag();
      Writer.RenderEndTag ();
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

    private void RenderPlaceHolder (Style style, PlaceHolder placeHolder, string defaultCssClass)
    {
      string cssClass = defaultCssClass;
      if (!string.IsNullOrEmpty (style.CssClass))
        cssClass = style.CssClass;

      if (placeHolder.Controls.Count == 0)
        cssClass += " " + CssClassEmpty;

      string backupCssClass = style.CssClass;
      style.CssClass = cssClass;
      style.AddAttributesToRender (Writer);
      style.CssClass = backupCssClass;

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      placeHolder.RenderControl (Writer);

      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiView</c>. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "tabbedMultiView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTabStrip</c>. </para>
    /// </remarks>
    public virtual string CssClassTabStrip
    {
      get { return "tabbedMultiViewTabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.ActiveViewStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassActiveView
    {
      get { return "tabbedMultiViewActiveView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.TopControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassTopControls
    {
      get { return "tabbedMultiViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBottomControls
    {
      get { return "tabbedMultiViewBottomControls"; }
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
    ///     Applied in addition to the regular CSS-Class. Use <c>td.tabbedMultiViewTopControls.emtpy</c> or 
    ///     <c>td.tabbedMultiViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    public virtual string CssClassEmpty
    {
      get { return "empty"; }
    }

    public string CssClassWrapper
    {
      get { return "wrapper"; }
    }

    public string CssClassContentBorder
    {
      get { return "contentBorder"; }
    }

    #endregion
  }
}
