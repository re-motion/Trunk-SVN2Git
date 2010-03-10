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
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for quirks mode rendering of <see cref="TabbedMultiView"/> controls.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewQuirksModeRenderer : RendererBase<ITabbedMultiView>
  {
    public TabbedMultiViewQuirksModeRenderer (HttpContextBase context, ITabbedMultiView control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (TabbedMultiViewQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (TabbedMultiViewQuirksModeRenderer), ResourceType.Html, "TabbedMultiView.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (Control, htmlHeadAppender);
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

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTopControls (writer);
      writer.RenderEndTag();

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTabStrip (writer);
      writer.RenderEndTag();

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderActiveView (writer);
      writer.RenderEndTag();

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderBottomControls (writer);
      writer.RenderEndTag();

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

    protected virtual void RenderTabStrip (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabStrip);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      Control.TabStrip.CssClass = CssClassTabStrip;
      Control.TabStrip.RenderControl (writer);

      writer.RenderEndTag (); // end td
    }

    protected virtual void RenderActiveView (HtmlTextWriter writer)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.ActiveViewClientID + " > *:first");

      if (Control.IsDesignMode)
        writer.AddStyleAttribute ("border", "solid 1px black");
      Control.ActiveViewStyle.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID);
      Control.ActiveViewStyle.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.ActiveViewStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ActiveViewClientID + "_Content");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      var view = Control.GetActiveView();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = view.Controls[i];
          control.RenderControl (writer);
        }
      }

      writer.RenderEndTag(); // end content div
      writer.RenderEndTag(); // end body div
      writer.RenderEndTag(); // end outer div

      writer.RenderEndTag(); // end td
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
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + placeHolder.ClientID + " > *:first");

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

    #endregion
  }
}