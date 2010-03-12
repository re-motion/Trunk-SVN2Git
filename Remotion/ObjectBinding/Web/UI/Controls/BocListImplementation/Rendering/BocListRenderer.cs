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
using System.Web;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="BocList"/> object.
  /// </summary>
  /// <remarks>Renders the outline of a <see cref="IBocList"/> object to an <see cref="HtmlTextWriter"/> and controls
  /// rendering of the various parts by delegating to specialized renderers.
  /// 
  /// This class should not be instantiated directly. Use a <see cref="BocRowRenderer"/> to obtain an instance.</remarks>
  /// <seealso cref="BocListNavigationBlockRenderer"/>
  /// <seealso cref="BocListRendererFactory"/>
  /// <seealso cref="BocListMenuBlockRenderer"/>
  public class BocListRenderer : BocRendererBase<IBocList>
  {
    private readonly IBocListMenuBlockRenderer _menuBlockRenderer;
    private readonly IBocListNavigationBlockRenderer _navigationBlockRenderer;
    private readonly IBocListTableBlockRenderer _tableBlockRenderer;
    private readonly CssClassContainer _cssClasses;

    /// <summary>
    /// Initializes the renderer with the <see cref="BocList"/> to render and the <see cref="HtmlTextWriter"/> to render it to,
    /// as well as a <see cref="BocListRendererFactory"/> used to create detail renderers.
    /// </summary>
    /// <param name="list">The <see cref="BocList"/> object to render.</param>
    /// <param name="context">The <see cref="HttpContextBase"/> which contains the response to render to.</param>
    /// <param name="cssClasses">The <see cref="CssClassContainer"/> containing the CSS classes to apply to the rendered elements.</param>
    /// <param name="tableBlockRenderer">The <see cref="IBocListTableBlockRenderer"/> responsible for rendering the table-part of the <see cref="BocList"/>.</param>
    /// <param name="navigationBlockRenderer">The <see cref="IBocListNavigationBlockRenderer"/> responsible for rendering the navigation-part of the <see cref="BocList"/>.</param>
    /// <param name="menuBlockRenderer">The <see cref="IBocListMenuBlockRenderer"/> responsible for rendering the menu-part of the <see cref="BocList"/>.</param>
    public BocListRenderer (
        HttpContextBase context,
        IBocList list,
        CssClassContainer cssClasses,
        IBocListTableBlockRenderer tableBlockRenderer,
        IBocListNavigationBlockRenderer navigationBlockRenderer,
        IBocListMenuBlockRenderer menuBlockRenderer)
        : base (context, list)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("tableBlockRenderer", tableBlockRenderer);
      ArgumentUtility.CheckNotNull ("navigationBlockRenderer", navigationBlockRenderer);
      ArgumentUtility.CheckNotNull ("menuBlockRenderer", menuBlockRenderer);

      _cssClasses = cssClasses;
      _tableBlockRenderer = tableBlockRenderer;
      _navigationBlockRenderer = navigationBlockRenderer;
      _menuBlockRenderer = menuBlockRenderer;
    }

    public IBocListMenuBlockRenderer MenuBlockRenderer
    {
      get { return _menuBlockRenderer; }
    }

    public IBocListNavigationBlockRenderer NavigationBlockRenderer
    {
      get { return _navigationBlockRenderer; }
    }

    public IBocListTableBlockRenderer TableBlockRenderer
    {
      get { return _tableBlockRenderer; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary>Gets the <see cref="BocList"/> object that will be rendered.</summary>
    public IBocList List
    {
      get { return Control; }
    }

    public override sealed string CssClassBase
    {
      get { return CssClasses.Base; }
    }

    public override sealed string CssClassDisabled
    {
      get { return CssClasses.Disabled; }
    }

    public override sealed string CssClassReadOnly
    {
      get { return CssClasses.ReadOnly; }
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control);

      string styleFileKey = typeof (BocListRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        string url = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (BocListRenderer), ResourceType.Html, ResourceTheme, "BocList.css");
        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, url, HtmlHeadAppender.Priority.Library);
      }

      string scriptFileKey = typeof (BocListRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        string scriptUrl = ResourceUrlResolver.GetResourceUrl (Control, Context, typeof (BocListRenderer), ResourceType.Html, "BocList.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      Control.EditModeControlFactory.RegisterHtmlHeadContents (Context, htmlHeadAppender);
    }

    /// <summary>
    /// Renders the <see cref="BocList"/> in the <see cref="List"/> property 
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

      RegisterInitializeGlobalsScript();

      AddAttributesToRender (writer, false);
      writer.RenderBeginTag (HtmlTextWriterTag.Div); // Control Tag

      //  Table Block
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.GetTableBlock(List.HasMenuBlock));
      if (List.HasMenuBlock && !List.MenuBlockWidth.IsEmpty)
        writer.AddStyleAttribute ("right", List.MenuBlockWidth.ToString ());
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TableBlockRenderer.Render (writer);

      if (List.HasNavigator)
        NavigationBlockRenderer.Render (writer);

      writer.RenderEndTag();

      //  Menu Block
      if (List.HasMenuBlock)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.MenuBlock);
        
        if (!List.MenuBlockWidth.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, List.MenuBlockWidth.ToString ());

        if (!List.MenuBlockOffset.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.PaddingLeft, List.MenuBlockOffset.ToString ());

        writer.RenderBeginTag (HtmlTextWriterTag.Div);
        MenuBlockRenderer.Render (writer);
        writer.RenderEndTag();
      }

      writer.RenderEndTag(); //  Control Tag
    }

    private void RegisterInitializeGlobalsScript ()
    {
      if (!Control.HasClientScript)
        return;

      string startUpScriptKey = typeof (BocListRenderer).FullName + "_Startup";
      if (!Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocListRenderer), startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClasses.DataRow,
            CssClasses.DataRowSelected);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (BocListRenderer), startUpScriptKey, script);
      }
    }
  }
}