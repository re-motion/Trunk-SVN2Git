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
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.Rendering.SingleView.QuirksMode
{
  /// <summary>
  /// Responsible for registering scripts, border spans and the style sheet for <see cref="SingleView"/> controls in quirks mode.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewPreRenderer : PreRendererBase<ISingleView>, ISingleViewPreRenderer
  {
    public SingleViewPreRenderer (IHttpContext context, ISingleView control)
        : base(context, control)
    {
    }

    public override void RegisterHtmlHeadContents ()
    {
      Control control = Control as Control;
      if (control != null)
      {
        ScriptUtility.RegisterElementForBorderSpans (control, Control.ClientID + "_View", false);
        ScriptUtility.RegisterElementForBorderSpans (control, Control.TopControl.ClientID, false);
        ScriptUtility.RegisterElementForBorderSpans (control, Control.BottomControl.ClientID, false);
      }

      string key = typeof (ISingleView).FullName + "_Style";
      if (!HtmlHeadAppender.Current.IsRegistered (key))
      {
        string styleSheetUrl = ResourceUrlResolver.GetResourceUrl (
            Control, Context, typeof (ISingleView), ResourceType.Html, ResourceTheme.Legacy, "SingleView.css");
        HtmlHeadAppender.Current.RegisterStylesheetLink (key, styleSheetUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public override void PreRender ()
    {
      
    }
  }
}