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
using Microsoft.Practices.ServiceLocation;
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.StandardMode
{
  /// <summary>
  /// Responsible for rendering <see cref="WebTabStrip"/> controls in quirks mode.
  /// <seealso cref="IWebTabStrip"/>
  /// </summary>
  public class WebTabStripRenderer : RendererBase<IWebTabStrip>, IWebTabStripRenderer
  {
    public WebTabStripRenderer (HttpContextBase context, HtmlTextWriter writer, IWebTabStrip control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      var visibleTabs = Control.GetVisibleTabs();

      RenderBeginTabsPane();
      for (int i = 0; i < visibleTabs.Count; i++)
      {
        bool isLast = i == (visibleTabs.Count - 1);
        var tab = visibleTabs[i];
        RenderTab (tab, isLast);
      }
      RenderEndTabsPane();
      Writer.RenderEndTag();
    }

    protected void AddAttributesToRender ()
    {
      AddStandardAttributesToRender();

      if (string.IsNullOrEmpty (Control.CssClass) && string.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    private void RenderBeginTabsPane ()
    {
      bool isEmpty = Control.Tabs.Count == 0;

      string cssClass = CssClassTabsPane;
      if (isEmpty)
        cssClass += " " + CssClassTabsPaneEmpty;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);

      Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Div

      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute ("list-style", "none");
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        Writer.AddStyleAttribute ("display", "inline");
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.Ul); // Begin List
    }

    private void RenderEndTabsPane ()
    {
      Writer.RenderEndTag(); // End List
      Writer.RenderEndTag(); // End Div
    }

    private void RenderTab (IWebTab tab, bool isLast)
    {
      var tabRenderer = tab.GetRenderer (ServiceLocator.Current, Context, Writer, Control);

      bool isEnabled = !tab.IsSelected || Control.EnableSelectedTab;
      WebTabStyle style = tab.IsSelected ? Control.SelectedTabStyle : Control.TabStyle;
      tabRenderer.Render (isEnabled, isLast, style);

      Writer.WriteLine ();
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

    #endregion
  }
}
