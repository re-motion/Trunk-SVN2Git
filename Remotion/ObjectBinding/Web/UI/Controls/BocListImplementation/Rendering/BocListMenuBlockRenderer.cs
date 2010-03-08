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
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the menu block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListMenuBlockRenderer : IBocListMenuBlockRenderer
  {
    private const string c_whiteSpace = "&nbsp;";
    protected const string c_defaultMenuBlockItemOffset = "5pt";
    protected const int c_designModeAvailableViewsListWidthInPoints = 40;

    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly CssClassContainer _cssClasses;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain an instance of this class.
    /// </remarks>
    public BocListMenuBlockRenderer (HttpContextBase context, IBocList list, CssClassContainer cssClasses)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _context = context;
      _list = list;
      _cssClasses = cssClasses;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary> Renders the menu block of the control. </summary>
    /// <remarks> Contains the drop down list for selcting a column configuration and the options menu.  </remarks> 
    public void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      string menuBlockItemOffset = c_defaultMenuBlockItemOffset;
      if (! List.MenuBlockItemOffset.IsEmpty)
        menuBlockItemOffset = List.MenuBlockItemOffset.ToString();

      RenderAvailableViewsList (writer, menuBlockItemOffset);

      RenderOptionsMenu (writer, menuBlockItemOffset);

      RenderListMenu (writer, menuBlockItemOffset);
    }

    private void RenderListMenu (HtmlTextWriter writer, string menuBlockItemOffset)
    {
      if (!List.HasListMenu)
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      List.ListMenu.RenderControl (writer);
      writer.RenderEndTag();
    }

    private void RenderOptionsMenu (HtmlTextWriter writer, string menuBlockItemOffset)
    {
      if (!List.HasOptionsMenu)
        return;

      List.OptionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
      List.OptionsMenu.RenderControl (writer);
    }

    private void RenderAvailableViewsList (HtmlTextWriter writer, string menuBlockItemOffset)
    {
      if (!List.HasAvailableViewsList)
        return;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.AvailableViewsListLabel);

      writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string availableViewsListTitle;
      if (StringUtility.IsNullOrEmpty (List.AvailableViewsListTitle))
        availableViewsListTitle = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.AvailableViewsListTitle);
      else
        availableViewsListTitle = List.AvailableViewsListTitle;
      // Do not HTML encode.
      writer.Write (availableViewsListTitle);
      writer.RenderEndTag();

      writer.Write (c_whiteSpace);
      if (List.IsDesignMode)
        List.AvailableViewsList.Width = Unit.Point (c_designModeAvailableViewsListWidthInPoints);
      List.AvailableViewsList.Enabled = !List.EditModeController.IsRowEditModeActive && !List.EditModeController.IsListEditModeActive;
      List.AvailableViewsList.CssClass = CssClasses.AvailableViewsListDropDownList;
      List.AvailableViewsList.RenderControl (writer);
      writer.RenderEndTag();
    }
  }
}