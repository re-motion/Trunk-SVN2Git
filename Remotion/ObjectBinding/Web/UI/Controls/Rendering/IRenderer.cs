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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base interface for all renderers able to render <see cref="IBocRenderableControl"/> objects.
  /// </summary>
  public interface IRenderer<TControl>
    where TControl : IBocRenderableControl
  {
    /// <summary>Gets the control to render.</summary>
    TControl Control { get; }

    /// <summary>Gets the writer to use for rendering.</summary>
    HtmlTextWriter Writer { get; }

    /// <summary>Gets the context in which rendering occurs.</summary>
    IHttpContext Context { get; }

    /// <summary>Gets the default CSS class, which is applied if no CSS class is defined on the control.</summary>
    string CssClassBase { get; }

    string CssClassDisabled { get; }

    string CssClassReadOnly { get; }

    /// <summary>Renders the <see cref="Control"/> using the <see cref="Writer"/> in the given <see cref="Context"/>.</summary>
    void Render ();
  }
}