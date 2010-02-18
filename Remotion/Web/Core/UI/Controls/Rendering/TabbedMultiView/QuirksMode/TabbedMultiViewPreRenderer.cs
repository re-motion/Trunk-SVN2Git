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
using Remotion.Utilities;
using System.Web;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.QuirksMode
{
  /// <summary>
  /// Responsible for registering scripts, border spans and the style sheet for <see cref="TabbedMultiView"/> controls in quirks mode.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewPreRenderer : PreRendererBase<ITabbedMultiView>, ITabbedMultiViewPreRenderer
  {
    public TabbedMultiViewPreRenderer (HttpContextBase context, ITabbedMultiView control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (ITabbedMultiView).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ITabbedMultiView), ResourceType.Html, ResourceTheme.Legacy, "TabbedMultiView.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (Control, htmlHeadAppender);
    }

    public override void PreRender ()
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.ActiveViewClientID + " > *:first");
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.TopControl.ClientID + " > *:first");
      ScriptUtility.Instance.RegisterElementForBorderSpans (Control, "#" + Control.BottomControl.ClientID + " > *:first");
    }
  }
}
