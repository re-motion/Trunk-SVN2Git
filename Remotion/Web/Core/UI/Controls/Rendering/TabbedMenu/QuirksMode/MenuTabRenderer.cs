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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip.QuirksMode;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMenu.QuirksMode
{
  public class MenuTabRenderer : WebTabRenderer, IMenuTabRenderer
  {
    public MenuTabRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control)
        : base(context, writer, control)
    {
    }

    public override void RenderBeginTagForCommand (IWebTab tab, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("style", style);

      var menuTab = ((IMenuTab) tab).GetActiveTab();
      var renderingCommand = GetRenderingCommand(menuTab, isEnabled);

      if (renderingCommand != null)
      {
        NameValueCollection additionalUrlParameters = menuTab.GetUrlParameters();
        renderingCommand.RenderBegin (
            Writer, menuTab.GetPostBackClientEvent(), new string[0], string.Empty, null, additionalUrlParameters, false, style);
      }
      else
      {
        style.AddAttributesToRender (Writer);
        Writer.RenderBeginTag (HtmlTextWriterTag.A);
      }
    }

    private Command GetRenderingCommand (IWebTab tab, bool isEnabled)
    {
      if (! (tab is IMenuTab))
        return null;

      if (isEnabled && tab.EvaluateEnabled())
        return ((IMenuTab)tab).Command;

      return null;
    }

    public override void RenderEndTagForCommand (IWebTab tab, bool isEnabled)
    {
      var renderingCommand = GetRenderingCommand (tab, isEnabled);
      if (renderingCommand != null)
        renderingCommand.RenderEnd (Writer);
      else
        Writer.RenderEndTag ();
    }
  }
}