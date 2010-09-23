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
using Microsoft.Practices.ServiceLocation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for standard mode rendering of <see cref="WebTabStrip"/> controls.
  /// <seealso cref="IWebTabStrip"/>
  /// </summary>
  public class WebTabStripRenderer : RendererBase<IWebTabStrip>
  {
    public WebTabStripRenderer (HttpContextBase context, IWebTabStrip control, IResourceUrlFactory resourceUrlFactory)
      : base (context, control, resourceUrlFactory)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (WebTabStripRenderer).FullName + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (WebTabStripRenderer), ResourceType.Html, "TabStrip.css");
      htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Render (new WebTabStripRenderingContext (Context, writer, Control));
    }

    public void Render (WebTabStripRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      var visibleTabs = renderingContext.Control.GetVisibleTabs ();

      RenderBeginTabsPane (renderingContext);
      for (int i = 0; i < visibleTabs.Count; i++)
      {
        bool isLast = i == (visibleTabs.Count - 1);
        var tab = visibleTabs[i];
        RenderTab (renderingContext, tab, isLast);
      }
      RenderEndTabsPane (renderingContext);
      RenderClearingPane (renderingContext);
      renderingContext.Writer.RenderEndTag ();
    }

    protected void AddAttributesToRender (WebTabStripRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddStandardAttributesToRender (renderingContext);

      if (string.IsNullOrEmpty (renderingContext.Control.CssClass) && string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    private void RenderBeginTabsPane (WebTabStripRenderingContext renderingContext)
    {
      bool isEmpty = renderingContext.Control.Tabs.Count == 0;

      string cssClass = CssClassTabsPane;
      if (isEmpty)
        cssClass += " " + CssClassTabsPaneEmpty;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Div

      if (renderingContext.Control.IsDesignMode)
      {
        renderingContext.Writer.AddStyleAttribute ("list-style", "none");
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        renderingContext.Writer.AddStyleAttribute ("display", "inline");
      }
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Ul); // Begin List
    }

    private void RenderEndTabsPane (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderEndTag (); // End List
      renderingContext.Writer.RenderEndTag (); // End Div
    }

    private void RenderClearingPane (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassClearingPane);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderTab (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isLast)
    {
      var tabRenderer = tab.GetRenderer (renderingContext.HttpContext, renderingContext.Control);

      bool isEnabled = !tab.IsSelected || renderingContext.Control.EnableSelectedTab;
      WebTabStyle style = tab.IsSelected ? renderingContext.Control.SelectedTabStyle : renderingContext.Control.TabStyle;
      tabRenderer.Render (renderingContext.Writer, isEnabled, isLast, style);

      renderingContext.Writer.WriteLine ();
    }

    #region public virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "tabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the pane of <see cref="WebTab"/> items. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    /// </remarks>
    public virtual string CssClassTabsPane
    {
      get { return "tabStripTabsPane"; }
    }

    /// <summary> Gets the CSS-Class applied to the wrapper around each <see cref="WebTab"/> item. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabWrapper</c>. </para>
    /// </remarks>
    public virtual string CssClassTabWrapper
    {
      get { return "tabStripTabWrapper"; }
    }

    /// <summary> Gets the CSS-Class applied to a pane of <see cref="WebTab"/> items if no items are present. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>div.tabStripTabsPane.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string CssClassTabsPaneEmpty
    {
      get { return "empty"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTab</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.TabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTab
    {
      get { return "tabStripTab"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSelected</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.SelectedTabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTabSelected
    {
      get { return "tabStripTabSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the inside of the anchor element. </summary>
    /// <remarks> 
    ///   <para> Class: <c>anchorBody</c>. </para>
    /// </remarks>
    public virtual string CssClassTabAnchorBody
    {
      get { return "anchorBody"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for clearing the space after the last tab. </summary>
    /// <remarks> 
    ///   <para> Class: <c>last</c>. </para>
    /// </remarks>
    public virtual string CssClassTabLast
    {
      get { return "last"; }
    }

    /// <summary> Gets the CSS-Class applied to a separator. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
    /// </remarks>
    public virtual string CssClassSeparator
    {
      get { return "tabStripTabSeparator"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTab"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.tabStripTab.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

    /// <summary> Gets the CSS-Class applied to the div used to clear the flow-layout at the end of the tab-strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>clearingPane</c>. </para>
    /// </remarks>
    public virtual string CssClassClearingPane
    {
      get { return "clearingPane"; }
    }

    #endregion
  }
}