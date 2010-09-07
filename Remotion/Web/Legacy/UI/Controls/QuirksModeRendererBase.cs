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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Base class for all renderers. Contains the essential properties used in rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class QuirksModeRendererBase<TControl> : IRenderer
      where TControl : IStyledControl
  {
    private readonly HttpContextBase _context;
    private readonly TControl _control;

    /// <summary>
    /// Initializes the <see cref="Context"/> and the <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected QuirksModeRendererBase (HttpContextBase context, TControl control)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);

      _control = control;
      _context = context;
    }

    /// <summary>Gets the <see cref="HttpContextBase"/> that contains the response for which this renderer generates output.</summary>
    public HttpContextBase Context
    {
      get { return _context; }
    }

    public abstract void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender);

    public abstract void Render (HtmlTextWriter writer);

    /// <summary>Gets the control that will be rendered.</summary>
    public TControl Control
    {
      get { return _control; }
    }

    protected void AddStandardAttributesToRender (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);

      if (!string.IsNullOrEmpty (renderingContext.Control.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, renderingContext.Control.CssClass);

      CssStyleCollection styles = renderingContext.Control.ControlStyle.GetStyleAttributes (renderingContext.Control);
      foreach (string style in styles.Keys)
      {
        renderingContext.Writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in renderingContext.Control.Attributes.Keys)
      {
        string value = renderingContext.Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          renderingContext.Writer.AddAttribute (attribute, value);
      }
    }
  }
}