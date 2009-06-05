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
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="BocList"/> object.
  /// </summary>
  /// <remarks>Renders the outline of a <see cref="IBocList"/> object to an <see cref="HtmlTextWriter"/> and controls
  /// rendering of the various parts by delegating to specialized renderers.
  /// 
  /// This class should not be instantiated directly. Use a <see cref="BocListRendererFactory"/> to obtain an instance.</remarks>
  /// <include file='doc\include\UI\Controls\Rendering\QuirksMode\BocListRenderer.xml' path='BocListRenderer/Class'/>
  /// <seealso cref="BocListMenuBlockRenderer"/>
  /// <seealso cref="BocRowRenderer"/>
  /// <seealso cref="BocListNavigationBlockRenderer"/>
  public class BocListRenderer : BocListRendererBase, IBocListRenderer
  {
    private const string c_defaultMenuBlockWidth = "70pt";
    private const string c_defaultMenuBlockOffset = "5pt";

    private delegate void RenderMethodDelegate ();

    private readonly IBocListMenuBlockRenderer _menuBlockRenderer;
    private readonly IBocListNavigationBlockRenderer _navigationBlockRenderer;
    private readonly IBocListTableBlockRenderer _tableBlockRenderer;

    /// <summary>
    /// Initializes the renderer with the <see cref="BocList"/> to render and the <see cref="HtmlTextWriter"/> to render it to,
    /// as well as a <see cref="BocListRendererFactory"/> used to create detail renderers.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> object to render.</param>
    /// <param name="context">The <see cref="IHttpContext"/> which contains the response to render to.</param>
    /// <param name="writer">The target <see cref="HtmlTextWriter"/>.</param>
    /// <param name="serviceLocator">The <see cref="IServiceLocator"/> from which factory objects for specialised renderers
    /// can be obtained.</param>
    public BocListRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, IServiceLocator serviceLocator)
        : base (context, writer, list)
    {
      _menuBlockRenderer = serviceLocator.GetInstance<IBocListMenuBlockRendererFactory>().CreateRenderer (context, writer, list);
      _navigationBlockRenderer = serviceLocator.GetInstance<IBocListNavigationBlockRendererFactory>().CreateRenderer (context, writer, list);
      _tableBlockRenderer = serviceLocator.GetInstance<IBocListTableBlockRendererFactory>().CreateRenderer (context, writer, list, serviceLocator);

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

    private RenderMethodDelegate RenderTopLevelColumnGroup { get; set; }

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
    public virtual void Render ()
    {
      //  Render list block / menu block
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      RenderTopLevelColumnGroup();

      Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      //  List Block
      Writer.AddStyleAttribute ("vertical-align", "top");
      Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      TableBlockRenderer.Render();

      if (List.HasNavigator)
        NavigationBlockRenderer.Render();

      Writer.RenderEndTag();

      if (List.HasMenuBlock)
      {
        //  Menu Block
        Writer.AddStyleAttribute ("vertical-align", "top");
        Writer.RenderBeginTag (HtmlTextWriterTag.Td);
        MenuBlockRenderer.Render();
        Writer.RenderEndTag();
      }

      Writer.RenderEndTag(); //  TR

      Writer.RenderEndTag(); //  Table
    }

    private void RenderTopLevelColumnGroupForLegacyBrowser ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      //  Left: list block
      Writer.WriteBeginTag ("col"); //  Required because RenderBeginTag(); RenderEndTag();
      //  writes empty tags, which is not valid for col in HTML 4.01
      Writer.Write (">");

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        Writer.WriteBeginTag ("col");
        Writer.Write (" style=\"");

        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString();
        Writer.WriteStyleAttribute ("width", menuBlockWidth);

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString();
        Writer.WriteStyleAttribute ("padding-left", menuBlockOffset);

        Writer.Write ("\">");
      }

      Writer.RenderEndTag();
    }

    private void RenderTopLevelColumnGroupForXmlBrowser ()
    {
      Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      // Left: list block
      Writer.RenderBeginTag (HtmlTextWriterTag.Col);
      Writer.RenderEndTag();

      if (List.HasMenuBlock)
      {
        //  Right: menu block
        string menuBlockWidth = c_defaultMenuBlockWidth;
        if (!List.MenuBlockWidth.IsEmpty)
          menuBlockWidth = List.MenuBlockWidth.ToString();

        string menuBlockOffset = c_defaultMenuBlockOffset;
        if (!List.MenuBlockOffset.IsEmpty)
          menuBlockOffset = List.MenuBlockOffset.ToString();

        Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, menuBlockWidth);
        Writer.AddStyleAttribute (HtmlTextWriterStyle.PaddingLeft, menuBlockOffset);
        Writer.RenderBeginTag (HtmlTextWriterTag.Col);
        Writer.RenderEndTag();
      }

      Writer.RenderEndTag();
    }
  }
}