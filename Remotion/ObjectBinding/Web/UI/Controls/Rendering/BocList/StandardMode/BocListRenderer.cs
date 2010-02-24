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
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="BocList"/> object.
  /// </summary>
  /// <remarks>Renders the outline of a <see cref="IBocList"/> object to an <see cref="HtmlTextWriter"/> and controls
  /// rendering of the various parts by delegating to specialized renderers.
  /// 
  /// This class should not be instantiated directly. Use a <see cref="BocRowRenderer"/> to obtain an instance.</remarks>
  /// <include file='doc\include\UI\Controls\Rendering\QuirksMode\BocListRenderer.xml' path='BocListRenderer/Class'/>
  /// <seealso cref="BocListNavigationBlockRenderer"/>
  /// <seealso cref="BocListRendererFactory"/>
  /// <seealso cref="BocListMenuBlockRenderer"/>
  public class BocListRenderer : BocListRendererBase, IBocListRenderer
  {
    private const string c_defaultMenuBlockWidth = "70pt";
    private const string c_defaultMenuBlockOffset = "5pt";

    private readonly IBocListMenuBlockRenderer _menuBlockRenderer;
    private readonly IBocListNavigationBlockRenderer _navigationBlockRenderer;
    private readonly IBocListTableBlockRenderer _tableBlockRenderer;

    /// <summary>
    /// Initializes the renderer with the <see cref="BocList"/> to render and the <see cref="HtmlTextWriter"/> to render it to,
    /// as well as a <see cref="BocListRendererFactory"/> used to create detail renderers.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> object to render.</param>
    /// <param name="context">The <see cref="HttpContextBase"/> which contains the response to render to.</param>
    /// <param name="cssClasses">The <see cref="CssClassContainer"/> containing the CSS classes to apply to the rendered elements.</param>
    /// <param name="serviceLocator">The <see cref="IServiceLocator"/> from which factory objects for specialised renderers
    /// can be obtained.</param>
    public BocListRenderer (HttpContextBase context, IBocList list, CssClassContainer cssClasses, IServiceLocator serviceLocator)
        : base (context, list, cssClasses)
    {
      _menuBlockRenderer = serviceLocator.GetInstance<IBocListMenuBlockRendererFactory>().CreateRenderer (context, list);
      _navigationBlockRenderer = serviceLocator.GetInstance<IBocListNavigationBlockRendererFactory>().CreateRenderer (context, list);
      _tableBlockRenderer = serviceLocator.GetInstance<IBocListTableBlockRendererFactory>().CreateRenderer (context, list, serviceLocator);

      RenderTopLevelColumnGroup = RenderTopLevelColumnGroupForLegacyBrowser;

      if (!ControlHelper.IsDesignMode (List))
      {
        bool isXmlRequired = (Context != null) && ControlHelper.IsXmlConformResponseTextRequired (Context);
        if (isXmlRequired)
          RenderTopLevelColumnGroup = RenderTopLevelColumnGroupForXmlBrowser;
      }
    }

    private IBocListMenuBlockRenderer MenuBlockRenderer
    {
      get { return _menuBlockRenderer; }
    }

    private IBocListNavigationBlockRenderer NavigationBlockRenderer
    {
      get { return _navigationBlockRenderer; }
    }

    private IBocListTableBlockRenderer TableBlockRenderer
    {
      get { return _tableBlockRenderer; }
    }

    private Action<HtmlTextWriter> RenderTopLevelColumnGroup { get; set; }

    /// <summary>
    /// Renders the <see cref="BocList"/> in the <see cref="BocListRendererBase.List"/> property 
    /// to the <see cref="HtmlTextWriter"/> in the Writer property.
    /// </summary>
    /// <remarks>
    /// This method provides the outline table of the <see cref="BocList"/>, creating three areas:
    /// <list type="bullet">
    /// <item><description>A table block displaying the title and data rows. See <see cref="IBocListTableBlockRenderer.Render"/>.</description></item>
    /// <item><description>A menu block containing the available commands. See <see cref="IBocListMenuBlockRenderer.Render"/></description></item>
    /// <item><description>A navigation block to browse through pages of data rows. See <see cref="IBocListNavigationBlockRenderer.Render"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BocListMenuBlockRenderer"/>
    /// <seealso cref="BocListNavigationBlockRenderer"/>
    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Render list block / menu block
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopLevelColumnGroup (writer);

      writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      //  List Block
      writer.AddStyleAttribute ("vertical-align", "top");
      writer.RenderBeginTag (HtmlTextWriterTag.Td);

      TableBlockRenderer.Render (writer);

      if (List.HasNavigator)
        NavigationBlockRenderer.Render (writer);

      writer.RenderEndTag();

      if (List.HasMenuBlock)
      {
        //  Menu Block
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.MenuBlock);
        writer.AddStyleAttribute ("vertical-align", "top");
        writer.RenderBeginTag (HtmlTextWriterTag.Td);
        MenuBlockRenderer.Render (writer);
        writer.RenderEndTag();
      }

      writer.RenderEndTag(); //  TR
      writer.RenderEndTag(); //  Table
      writer.RenderEndTag(); //  div
    }

    private void RenderTopLevelColumnGroupForLegacyBrowser (HtmlTextWriter writer)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      //  Left: list block
      writer.WriteBeginTag ("col"); //  Required because RenderBeginTag(); RenderEndTag();
      //  writes empty tags, which is not valid for col in HTML 4.01
      writer.Write (">");

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        writer.WriteBeginTag ("col");
        writer.Write (" style=\"");

        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString();
        writer.WriteStyleAttribute ("width", menuBlockWidth);

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString();
        writer.WriteStyleAttribute ("padding-left", menuBlockOffset);

        writer.Write ("\">");
      }

      writer.RenderEndTag();
    }

    private void RenderTopLevelColumnGroupForXmlBrowser (HtmlTextWriter writer)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      // Left: list block
      writer.RenderBeginTag (HtmlTextWriterTag.Col);
      writer.RenderEndTag();

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString();

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString();

        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);
        writer.AddStyleAttribute (HtmlTextWriterStyle.PaddingLeft, menuBlockOffset);
        writer.RenderBeginTag (HtmlTextWriterTag.Col);
        writer.RenderEndTag();
      }

      writer.RenderEndTag();
    }
  }
}
