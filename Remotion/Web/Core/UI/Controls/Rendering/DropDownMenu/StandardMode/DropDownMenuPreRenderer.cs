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

namespace Remotion.Web.UI.Controls.Rendering.DropDownMenu.StandardMode
{
  /// <summary>
  /// Overrides <see cref="GetBrowserCapableOfScripting"/> to determine if the <see cref="DropDownMenu"/> can be rendered in standard mode.
  /// <seealso cref="DropDownMenuPreRendererBase"/>
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuPreRenderer : DropDownMenuPreRendererBase
  {
    public DropDownMenuPreRenderer (IHttpContext context, IDropDownMenu control)
        : base (context, control)
    {
    }

    public override void PreRender ()
    {
      base.PreRender();

      if (!Control.Enabled )
        return;

      string key = Control.ClientID + "_KeyDownEventHandlerBindScript";
      string getSelectionCount = (string.IsNullOrEmpty (Control.GetSelectionCount) ? "null" : Control.GetSelectionCount);
      string script = string.Format (
          "$('#{0}').keydown( function(event){{ DropDownMenu_OnKeyDown(event, document.getElementById('{0}'), {1}); }} );",
          Control.ClientID,
          getSelectionCount);
      
      Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuPreRenderer), key, script);

      if (Control.Enabled && Control.Visible && Control.Mode==MenuMode.DropDownMenu)
      {
        key = Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format ("$('#{0}')", Control.ClientID);
        string menuIDReference = string.Format ("'{0}'", Control.ClientID);
        script = Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
        Control.Page.ClientScript.RegisterStartupScriptBlock (Control, typeof (DropDownMenuPreRenderer), key, script);
      }
    }

    public override bool GetBrowserCapableOfScripting ()
    {
      return true;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterJQueryJavaScriptInclude (Control.Page);

      string key = typeof (IDropDownMenu).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string url = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDropDownMenu), ResourceType.Html, "DropDownMenu.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, url);
      }


      key = typeof (IDropDownMenu).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (IDropDownMenu), ResourceType.Html, ResourceTheme, "DropDownMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }
  }
}