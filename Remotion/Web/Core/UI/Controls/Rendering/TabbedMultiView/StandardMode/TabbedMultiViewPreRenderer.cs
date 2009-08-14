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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  /// <summary>
  /// Responsible for registering scripts, border spans and the style sheet for <see cref="TabbedMultiView"/> controls in standard mode.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewPreRenderer : PreRendererBase<ITabbedMultiView>, ITabbedMultiViewPreRenderer
  {
    public TabbedMultiViewPreRenderer (IHttpContext context, ITabbedMultiView control)
        : base (context, control)
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterJQueryJavaScriptInclude (Control.Page);

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

      RegisterAdjustViewScript ();

      ScriptUtility.Instance.RegisterElementForBorderSpans (htmlHeadAppender, Control, "#" + Control.WrapperClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (htmlHeadAppender, Control, "#" + Control.ActiveViewClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (htmlHeadAppender, Control, "#" + Control.TopControl.ClientID);
      ScriptUtility.Instance.RegisterElementForBorderSpans (htmlHeadAppender, Control, "#" + Control.BottomControl.ClientID);
    }

    public override void PreRender ()
    {
      
    }

    private void RegisterAdjustViewScript ()
    {
      string keyAdjust = Control.ClientID + "_AdjustView";
      string scriptAdjust = "function adjustView_{0}(){{ ViewLayout.AdjustTabbedMultiView($('#{0}')); }}";
      scriptAdjust = string.Format (scriptAdjust, Control.ClientID, Control.TabStripContainerClientID, Control.ActiveViewClientID);
      Control.Page.ClientScript.RegisterClientScriptBlock (Control, typeof (TabbedMultiViewPreRenderer), keyAdjust, scriptAdjust);

      ScriptUtility.Instance.RegisterResizeOnElement (
          Control,
          string.Format ("$('#{0}')", Control.ClientID),
          string.Format ("adjustView_{0}", Control.ClientID));
      ScriptUtility.Instance.TriggerEventAfterPageLoad (Control, "$(window)", ScriptUtility.Event.Resize);
    }
  }
}