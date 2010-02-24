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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.SingleView.StandardMode
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for standard mode rendering of <see cref="SingleView"/> controls.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewRenderer : RendererBase<ISingleView>
  {
    public SingleViewRenderer (HttpContextBase context, ISingleView control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control.Page);

      string keyStyle = typeof (ISingleView).FullName + "_Style";
      string keyScript = typeof (ISingleView).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (keyStyle))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ISingleView), ResourceType.Html, ResourceTheme, "SingleView.css");
        htmlHeadAppender.RegisterStylesheetLink (keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

        string scriptUrl = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (ISingleView), ResourceType.Html, "ViewLayout.js");
        htmlHeadAppender.RegisterJavaScriptInclude (keyScript, scriptUrl);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (Control, htmlHeadAppender);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
  
      RegisterAdjustViewScript ();

      AddStandardAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
      if (Control.IsDesignMode)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "75%");
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.WrapperClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.WrapperClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassWrapper);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderTopControls (writer);
      RenderView (writer);
      RenderBottomControls (writer);
      
      writer.RenderEndTag();
      writer.RenderEndTag ();
    }

    public string CssClassWrapper
    {
      get { return "wrapper"; }
    }

    protected virtual void RenderTopControls (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Style style = Control.TopControlsStyle;
      PlaceHolder placeHolder = Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Style style = Control.BottomControlsStyle;
      PlaceHolder placeHolder = Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (writer, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (HtmlTextWriter writer, Style style, PlaceHolder placeHolder, string defaultCssClass)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + placeHolder.ClientID);

      string cssClass = defaultCssClass;
      if (!string.IsNullOrEmpty (style.CssClass))
        cssClass = style.CssClass;

      if (placeHolder.Controls.Count == 0)
        cssClass += " " + CssClassEmpty;

      string backupCssClass = style.CssClass;
      style.CssClass = cssClass;
      style.AddAttributesToRender (writer);
      style.CssClass = backupCssClass;

      writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      placeHolder.RenderControl (writer);

      writer.RenderEndTag ();
      writer.RenderEndTag ();
    }

    private void RenderView (HtmlTextWriter writer)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.ViewClientID);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      Control.ViewStyle.AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ViewContentClientID);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContentBorder);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      Control.View.RenderControl (writer);

      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag ();
    }

    private void RegisterAdjustViewScript ()
    {
      ScriptUtility.Instance.RegisterResizeOnElement (Control, string.Format ("'#{0}'", Control.ClientID), "ViewLayout.AdjustSingleView");

      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (SingleViewRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("ViewLayout.AdjustSingleView ($('#{0}'));", Control.ClientID));
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
