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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  public abstract class RendererBase<TControl>
  {
    private readonly HtmlTextWriter _writer;
    private readonly IHttpContext _context;
    private readonly TControl _control;

    protected RendererBase (IHttpContext context, HtmlTextWriter writer, TControl control)
    {
      _writer = writer;
      _control = control;
      _context = context;
    }

    /// <summary>Gets the <see cref="HtmlTextWriter"/> object used to render the <see cref="BocList"/>.</summary>
    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    /// <summary>Gets the <see cref="IHttpContext"/> that contains the response for which this renderer generates output.</summary>
    public IHttpContext Context
    {
      get { return _context; }
    }

    protected TControl Control
    {
      get { return _control; }
    }
  }
}