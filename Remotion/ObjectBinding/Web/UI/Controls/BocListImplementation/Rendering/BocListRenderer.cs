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
using Remotion.Web.UI.Controls;

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
    private readonly BocListCssClassDefinition _cssClasses;

    public BocListRenderer (
        HttpContextBase context,
        IBocList list,
        IResourceUrlFactory resourceUrlFactory,
        BocListCssClassDefinition cssClasses,
        IBocListTableBlockRenderer tableBlockRenderer,
        IBocListNavigationBlockRenderer navigationBlockRenderer,
        IBocListMenuBlockRenderer menuBlockRenderer)
        : base (context, list, resourceUrlFactory)
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

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary>Gets the <see cref="BocList"/> object that will be rendered.</summary>
    public IBocList List
    {
      get { return Control; }
    }

    public override sealed string GetCssClassBase(IBocList control)
    {
      return CssClasses.Base;
    }

    public override sealed string CssClassDisabled
    {
      get { return CssClasses.Disabled; }
    }

    public override sealed string CssClassReadOnly
    {
      get { return CssClasses.ReadOnly; }
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string styleFileKey = typeof (BocListRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocListRenderer), ResourceType.Html, "BocList.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);

      string scriptFileKey = typeof (BocListRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocListRenderer), ResourceType.Html, "BocList.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);

      ((IBocList) control).EditModeControlFactory.RegisterHtmlHeadContents (Context, htmlHeadAppender);
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

      Render (new BocListRenderingContext (Context, writer, Control));
    }

    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterInitializeGlobalsScript (renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag ();
    }

    protected virtual void RenderContents (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      //  Menu Block
      if (renderingContext.Control.HasMenuBlock)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.MenuBlock);

        if (!renderingContext.Control.MenuBlockWidth.IsEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.MenuBlockWidth.ToString ());

        if (!renderingContext.Control.MenuBlockOffset.IsEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.MarginLeft, renderingContext.Control.MenuBlockOffset.ToString ());

        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);
        MenuBlockRenderer.Render (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag ();
      }

      //  Table Block
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.GetTableBlock (renderingContext.Control.HasMenuBlock, renderingContext.Control.HasNavigator));
      if (renderingContext.Control.HasMenuBlock && !renderingContext.Control.MenuBlockWidth.IsEmpty)
        renderingContext.Writer.AddStyleAttribute ("right", renderingContext.Control.MenuBlockWidth.ToString ());
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      TableBlockRenderer.Render (renderingContext.Writer);

      if (renderingContext.Control.HasNavigator)
        NavigationBlockRenderer.Render (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag ();
    }

    private void RegisterInitializeGlobalsScript (BocListRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasClientScript)
        return;

      string startUpScriptKey = typeof (BocListRenderer).FullName + "_Startup";
      if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocListRenderer), startUpScriptKey))
      {
        string script = string.Format (
            "BocList_InitializeGlobals ('{0}', '{1}');",
            CssClasses.DataRow,
            CssClasses.DataRowSelected);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (BocListRenderer), startUpScriptKey, script);
      }
    }
  }
}