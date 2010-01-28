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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base class for all renderers. Contains the essential properties used in rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class RendererBase<TControl>
    where TControl : IStyledControl
  {
    private readonly HtmlTextWriter _writer;
    private readonly IHttpContext _context;
    private readonly TControl _control;

    /// <summary>
    /// Initializes the <see cref="Context"/>, <see cref="Writer"/> and <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected RendererBase (IHttpContext context, HtmlTextWriter writer, TControl control)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("control", control);

      _writer = writer;
      _control = control;
      _context = context;
    }

    /// <summary>Gets the <see cref="HtmlTextWriter"/> object used to render the <see cref="IControl"/>.</summary>
    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    /// <summary>Gets the <see cref="IHttpContext"/> that contains the response for which this renderer generates output.</summary>
    public IHttpContext Context
    {
      get { return _context; }
    }

    /// <summary>Gets the control that will be rendered.</summary>
    public TControl Control
    {
      get { return _control; }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return ServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }

    protected void AddStandardAttributesToRender ()
    {
      Writer.AddAttribute (HtmlTextWriterAttribute.Id, Control.ClientID);

      if (!string.IsNullOrEmpty (Control.CssClass))
        Writer.AddAttribute (HtmlTextWriterAttribute.Class, Control.CssClass);

      CssStyleCollection styles = Control.ControlStyle.GetStyleAttributes (Control);
      foreach (string style in styles.Keys)
      {
        Writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in Control.Attributes.Keys)
      {
        string value = Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          Writer.AddAttribute (attribute, value);
      }
    }
  }
}
