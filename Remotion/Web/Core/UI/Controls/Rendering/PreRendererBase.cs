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

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base class for all prerenderers. Contains the essential properties used for preparing a control's rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class PreRendererBase<TControl> : IPreRenderer<TControl>
      where TControl: IControl
  {
    private readonly IHttpContext _context;
    private readonly TControl _control;

    /// <summary>
    /// Initializes the <see cref="Context"/> and <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected PreRendererBase (IHttpContext context, TControl control)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("control", control);

      _control = control;
      _context = context;
    }

    public abstract void RegisterHtmlHeadContents ();
    public abstract void PreRender ();

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
  }
}