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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using System.Web;

namespace Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IRenderer"/> for standard mode rendering of <see cref="TabbedMenu"/> controls.
  /// <seealso cref="ITabbedMenu"/>
  /// </summary>
  public class TabbedMenuRenderer : RendererBase<ITabbedMenu>
  {
    public TabbedMenuRenderer (HttpContextBase context, ITabbedMenu control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      string key = typeof (TabbedMenuRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (TabbedMenuRenderer), ResourceType.Html, ResourceTheme, "TabbedMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.Table);

      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin main menu row

      writer.AddAttribute (HtmlTextWriterAttribute.Colspan, "2");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassMainMenuCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin main menu cell
      Control.MainMenuTabStrip.CssClass = CssClassMainMenu;
      Control.MainMenuTabStrip.Width = Unit.Percentage (100);
      Control.MainMenuTabStrip.RenderControl (writer);
      writer.RenderEndTag(); // End main menu cell

      writer.RenderEndTag(); // End main menu row

      writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin sub menu row

      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSubMenuCell);
      if (!Control.SubMenuBackgroundColor.IsEmpty)
      {
        string backGroundColor = ColorTranslator.ToHtml (Control.SubMenuBackgroundColor);
        writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, backGroundColor);
      }
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin sub menu cell
      Control.SubMenuTabStrip.Style["width"] = "auto";
      Control.SubMenuTabStrip.CssClass = CssClassSubMenu;
      Control.SubMenuTabStrip.RenderControl (writer);
      writer.RenderEndTag(); // End sub menu cell

      Control.StatusStyle.AddAttributesToRender (writer);
      if (string.IsNullOrEmpty (Control.StatusStyle.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassStatusCell);
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell

      if (string.IsNullOrEmpty (Control.StatusText))
        writer.Write ("&nbsp;");
      else
        writer.Write (Control.StatusText); // Do not HTML encode

      writer.RenderEndTag(); // End status cell
      writer.RenderEndTag(); // End sub menu row
      writer.RenderEndTag(); // End table
    }

    protected void AddAttributesToRender (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddStandardAttributesToRender (writer);

      if (Control.IsDesignMode)
        writer.AddStyleAttribute ("width", "100%");
      if (StringUtility.IsNullOrEmpty (Control.CssClass) && StringUtility.IsNullOrEmpty (Control.Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
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