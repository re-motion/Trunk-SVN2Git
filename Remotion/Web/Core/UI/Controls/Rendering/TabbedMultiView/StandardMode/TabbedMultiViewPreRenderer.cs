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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  /// <summary>
  /// Responsible for registering scripts, border spans and the style sheet for <see cref="TabbedMultiView"/> controls in standard mode.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewPreRenderer : PreRendererBase<ITabbedMultiView>, ITabbedMultiViewPreRenderer
  {
    public TabbedMultiViewPreRenderer (HttpContextBase context, ITabbedMultiView control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude (Control.Page);

      string keyStyle = typeof (ITabbedMultiView).FullName + "_Style";
      string keyScript = typeof (ITabbedMultiView).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (keyStyle))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme, "TabbedMultiView.css");
        htmlHeadAppender.RegisterStylesheetLink (keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

        string scriptFileUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, "ViewLayout.js");
        htmlHeadAppender.RegisterJavaScriptInclude (keyScript, scriptFileUrl);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (Control, htmlHeadAppender);
    }

    public override void PreRender ()
    {
      RegisterAdjustViewScript ();

      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.WrapperClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.ActiveViewClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.TopControl.ClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.BottomControl.ClientID);
    }

    private void RegisterAdjustViewScript ()
    {
      ScriptUtility.Instance.RegisterResizeOnElement (Control, string.Format ("'#{0}'", Control.ClientID), "ViewLayout.AdjustTabbedMultiView");

      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (TabbedMultiViewPreRenderer),
          Guid.NewGuid().ToString(),
          string.Format ("ViewLayout.AdjustTabbedMultiView ($('#{0}'));", Control.ClientID));
    }
  }
}
