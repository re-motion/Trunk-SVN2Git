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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="WebTab"/> controls in quirks mode.
  /// </summary>
  public class WebTabRenderer : RendererBase<IWebTabStrip>, IWebTabRenderer
  {
    private readonly IWebTab _tab;

    public WebTabRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control, IWebTab tab)
        : base(context, writer, control)
    {
      _tab = tab;
    }

    public void Render (bool isEnabled, bool isLast, WebTabStyle style)
    {
      RenderTabBegin ();
      RenderSeperator ();
      RenderWrapperBegin ();

      RenderBeginTagForCommand (isEnabled, style);
      RenderContents ();
      RenderEndTagForCommand ();

      Writer.RenderEndTag (); // End tab span
      Writer.RenderEndTag (); // End tab wrapper span

      if (isLast)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabLast);
        Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        Writer.RenderEndTag ();
      }

      Writer.RenderEndTag (); // End list item
    }

    public IWebTab Tab
    {
      get { return _tab; }
    }

    private void RenderWrapperBegin ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID + "_" + Tab.ItemID);
      string cssClass;
      if (Tab.IsSelected)
        cssClass = CssClassTabSelected;
      else
        cssClass = CssClassTab;
      if (!Tab.EvaluateEnabled ())
        cssClass += " " + CssClassDisabled;
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin tab span
    }

    private void RenderTabBegin ()
    {
      if (Control.IsDesignMode)
      {
        Writer.AddStyleAttribute ("float", "left");
        Writer.AddStyleAttribute ("display", "block");
        Writer.AddStyleAttribute ("white-space", "nowrap");
      }

      Writer.RenderBeginTag (HtmlTextWriterTag.Li); // Begin list item

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, "tabStripTabWrapper");
      Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin tab wrapper span
    }

    private void RenderSeperator ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSeparator);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      Writer.RenderEndTag ();
      Writer.RenderEndTag ();
    }

    protected virtual void RenderBeginTagForCommand (bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("style", style);
      if (isEnabled && !Tab.IsDisabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, Tab.GetPostBackClientEvent ());
      }
      style.AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
    }

    protected virtual void RenderEndTagForCommand ()
    {
      Writer.RenderEndTag ();
    }

    protected virtual void RenderContents ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabAnchorBody);
      Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin anchor body span

      bool hasIcon = Tab.Icon != null && !string.IsNullOrEmpty (Tab.Icon.Url);
      bool hasText = !string.IsNullOrEmpty (Tab.Text);
      if (hasIcon)
        Tab.Icon.Render (Writer);
      else
        IconInfo.RenderInvisibleSpacer (Writer);
      if (hasIcon && hasText)
        Writer.Write ("&nbsp;");
      if (hasText)
        Writer.Write (Tab.Text); // Do not HTML encode
      if (!hasIcon && !hasText)
        Writer.Write ("&nbsp;");

      Writer.RenderEndTag (); // End anchor body span

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