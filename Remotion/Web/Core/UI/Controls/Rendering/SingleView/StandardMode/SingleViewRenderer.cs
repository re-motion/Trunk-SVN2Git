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

namespace Remotion.Web.UI.Controls.Rendering.SingleView.StandardMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="SingleView"/> control in standard mode.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewRenderer : RendererBase<ISingleView>, ISingleViewRenderer
  {
    public SingleViewRenderer (IHttpContext context, HtmlTextWriter writer, ISingleView control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);
      AddStandardAttributesToRender();
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "75%");
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.WrapperClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassWrapper);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderTopControls();
      RenderView ();
      RenderBottomControls();
      
      Writer.RenderEndTag();
      Writer.RenderEndTag ();
    }

    public string CssClassWrapper
    {
      get { return "wrapper"; }
    }

    private void RenderTopControls ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.TopControl.ClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTopControls);
      Control.TopControlsStyle.AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Control.TopControl.RenderControl (Writer);

      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    private void RenderBottomControls ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.BottomControl.ClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBottomControls);
      Control.BottomControlsStyle.AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Control.BottomControl.RenderControl (Writer);

      Writer.RenderEndTag();
      Writer.RenderEndTag();
    }

    private void RenderView ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      Control.ViewStyle.AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewContentClientID);
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContentBorder);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Control.View.RenderControl (Writer);

      Writer.RenderEndTag();
      Writer.RenderEndTag();
      Writer.RenderEndTag ();
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

    public virtual string CssClassContentBorder
    {
      get { return "contentBorder"; }
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