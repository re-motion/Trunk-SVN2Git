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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.StandardMode
{
  /// <summary>
  /// Responsible for rendering <see cref="WebTab"/> controls in quirks mode.
  /// </summary>
  public class WebTabRenderer : IWebTabRenderer
  {
    private readonly HttpContextBase _context;
    private readonly IWebTabStrip _control;
    private readonly IWebTab _tab;

    public WebTabRenderer (HttpContextBase context, IWebTabStrip control, IWebTab tab)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("tab", tab);

      _context = context;
      _control = control;
      _tab = tab;
    }

    public void Render (HtmlTextWriter writer, bool isEnabled, bool isLast, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("style", style);

      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + TabClientID + " > *:first");

      RenderTabBegin (writer);
      RenderSeperator (writer);
      RenderWrapperBegin (writer);

      RenderBeginTagForCommand (writer, isEnabled, style);
      RenderContents (writer);
      RenderEndTagForCommand (writer);

      writer.RenderEndTag (); // End tab span
      writer.RenderEndTag (); // End tab wrapper span

      if (isLast)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabLast);
        writer.RenderBeginTag (HtmlTextWriterTag.Span);
        writer.RenderEndTag ();
      }

      writer.RenderEndTag (); // End list item
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IWebTabStrip Control
    {
      get { return _control; }
    }

    public IWebTab Tab
    {
      get { return _tab; }
    }

    private string TabClientID
    {
      get { return Control.ClientID + "_" + Tab.ItemID; }
    }

    private void RenderWrapperBegin (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Id, TabClientID);
      string cssClass;
      if (Tab.IsSelected)
        cssClass = CssClassTabSelected;
      else
        cssClass = CssClassTab;
      if (!Tab.EvaluateEnabled ())
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin tab span
    }

    private void RenderTabBegin (HtmlTextWriter writer)
    {
      if (Control.IsDesignMode)
      {
        writer.AddStyleAttribute ("float", "left");
        writer.AddStyleAttribute ("display", "block");
        writer.AddStyleAttribute ("white-space", "nowrap");
      }

      writer.RenderBeginTag (HtmlTextWriterTag.Li); // Begin list item

      writer.AddAttribute (HtmlTextWriterAttribute.Class, "tabStripTabWrapper");
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin tab wrapper span
    }

    private void RenderSeperator (HtmlTextWriter writer)
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSeparator);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      writer.RenderEndTag ();
      writer.RenderEndTag ();
    }

    protected virtual void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("style", style);

      if (isEnabled && !Tab.IsDisabled)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, Tab.GetPostBackClientEvent ());
      }
      style.AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
    }

    protected virtual void RenderEndTagForCommand (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.RenderEndTag ();
    }

    protected virtual void RenderContents (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabAnchorBody);
      writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin anchor body span

      bool hasIcon = Tab.Icon != null && !string.IsNullOrEmpty (Tab.Icon.Url);
      bool hasText = !string.IsNullOrEmpty (Tab.Text);
      if (hasIcon)
        Tab.Icon.Render (writer);
      else
        IconInfo.RenderInvisibleSpacer (writer);
      if (hasIcon && hasText)
        writer.Write ("&nbsp;");
      if (hasText)
        writer.Write (Tab.Text); // Do not HTML encode
      if (!hasIcon && !hasText)
        writer.Write ("&nbsp;");

      writer.RenderEndTag (); // End anchor body span

    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the inside of the anchor element. </summary>
    /// <remarks> 
    ///   <para> Class: <c>anchorBody</c>. </para>
    /// </remarks>
    public virtual string CssClassTabAnchorBody
    {
      get { return "anchorBody"; }
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

    /// <summary> Gets the CSS-Class applied to a separator. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
    /// </remarks>
    public virtual string CssClassSeparator
    {
      get { return "tabStripTabSeparator"; }
    }


    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for clearing the space after the last tab. </summary>
    /// <remarks> 
    ///   <para> Class: <c>last</c>. </para>
    /// </remarks>
    public virtual string CssClassTabLast
    {
      get { return "last"; }
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
  }
}
