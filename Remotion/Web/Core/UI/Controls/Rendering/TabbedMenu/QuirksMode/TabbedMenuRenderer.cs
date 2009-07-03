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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMenu.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="TabbedMenu"/> control in quirks mode.
  /// <seealso cref="ITabbedMenu"/>
  /// </summary>
  public class TabbedMenuRenderer : RendererBase<ITabbedMenu>, ITabbedMenuRenderer
  {
    public TabbedMenuRenderer (IHttpContext context, HtmlTextWriter writer, ITabbedMenu control)
        : base (context, writer, control)
    {
    }

    public void Render ()
    {
      AddAttributesToRender();
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin main menu row

      Writer.AddAttribute (HtmlTextWriterAttribute.Colspan, "2");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassMainMenuCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin main menu cell
      Control.MainMenuTabStrip.CssClass = CssClassMainMenu;
      Control.MainMenuTabStrip.Width = Unit.Percentage (100);
      Control.MainMenuTabStrip.RenderControl (Writer);
      Writer.RenderEndTag(); // End main menu cell

      Writer.RenderEndTag(); // End main menu row

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin sub menu row

      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSubMenuCell);
      if (!Control.SubMenuBackgroundColor.IsEmpty)
      {
        string backGroundColor = ColorTranslator.ToHtml (Control.SubMenuBackgroundColor);
        Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, backGroundColor);
      }
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin sub menu cell
      Control.SubMenuTabStrip.Style["width"] = "auto";
      Control.SubMenuTabStrip.CssClass = CssClassSubMenu;
      Control.SubMenuTabStrip.RenderControl (Writer);
      Writer.RenderEndTag(); // End sub menu cell

      Control.StatusStyle.AddAttributesToRender (Writer);
      if (string.IsNullOrEmpty (Control.StatusStyle.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassStatusCell);
      Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell

      if (string.IsNullOrEmpty (Control.StatusText))
        Writer.Write ("&nbsp;");
      else
        Writer.Write (Control.StatusText); // Do not HTML encode

      Writer.RenderEndTag(); // End status cell
      Writer.RenderEndTag(); // End sub menu row
      Writer.RenderEndTag(); // End table
    }

    protected void AddAttributesToRender ()
    {
      AddStandardAttributesToRender();

      if (Control.IsDesignMode)
        Writer.AddStyleAttribute ("width", "100%");
      if (StringUtility.IsNullOrEmpty (Control.CssClass) && StringUtility.IsNullOrEmpty (Control.Attributes["class"]))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "tabbedMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenu
    {
      get { return "tabbedMainMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenu
    {
      get { return "tabbedSubMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenuCell
    {
      get { return "tabbedMainMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenuCell
    {
      get { return "tabbedSubMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the status cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMenuStatusCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassStatusCell
    {
      get { return "tabbedMenuStatusCell"; }
    }

    #endregion
  }
}