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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using System.Web;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base class for all renderers. Contains the essential properties used in rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class RendererBase<TControl>
    where TControl : IStyledControl
  {
    private readonly HttpContextBase _context;
    private readonly TControl _control;

    /// <summary>
    /// Initializes the <see cref="Context"/> and the <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected RendererBase (HttpContextBase context, HtmlTextWriter writer, TControl control)
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

    public abstract void Render (HtmlTextWriter writer);

    /// <summary>Gets the control that will be rendered.</summary>
    public TControl Control
    {
      get { return _control; }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return ServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }

    protected void AddStandardAttributesToRender (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);

      if (!string.IsNullOrEmpty (Control.CssClass))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClass);

      CssStyleCollection styles = Control.ControlStyle.GetStyleAttributes (Control);
      foreach (string style in styles.Keys)
      {
        writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in Control.Attributes.Keys)
      {
        string value = Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          writer.AddAttribute (attribute, value);
      }
    }
  }
}
