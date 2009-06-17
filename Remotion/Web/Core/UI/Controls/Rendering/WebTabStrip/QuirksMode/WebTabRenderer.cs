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
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="WebTab"/> controls in quirks mode.
  /// </summary>
  public class WebTabRenderer : RendererBase<IWebTabStrip>, IWebTabRenderer
  {
    public WebTabRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control)
        : base(context, writer, control)
    {
    }

    public virtual void RenderBeginTagForCommand (IWebTab tab, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("style", style);
      if (isEnabled && ! tab.IsDisabled)
      {
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, tab.GetPostBackClientEvent ());
      }
      style.AddAttributesToRender (Writer);
      Writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor
    }

    public virtual void RenderEndTagForCommand (IWebTab tab, bool isEnabled)
    {
      Writer.RenderEndTag ();
    }

    public virtual void RenderContents (IWebTab tab)
    {
      bool hasIcon = tab.Icon != null && !string.IsNullOrEmpty (tab.Icon.Url);
      bool hasText = !string.IsNullOrEmpty (tab.Text);
      if (hasIcon)
        tab.Icon.Render (Writer);
      else
        IconInfo.RenderInvisibleSpacer (Writer);
      if (hasIcon && hasText)
        Writer.Write ("&nbsp;");
      if (hasText)
        Writer.Write (tab.Text); // Do not HTML encode
      if (!hasIcon && !hasText)
        Writer.Write ("&nbsp;");
    }
  }
}