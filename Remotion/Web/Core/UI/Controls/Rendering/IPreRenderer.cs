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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Base interface for classes that handle markup-related actions, such as registering HTML head contents,
  /// which have to be executed before the rendering stage.
  /// </summary>
  public interface IPreRenderer<TControl>
      where TControl: IControl
  {
    /// <summary>Gets the control to render.</summary>
    TControl Control { get; }

    /// <summary>Gets the context in which rendering occurs.</summary>
    IHttpContext Context { get; }

    /// <summary>Registers script and stylesheet file includes, which has to be done during the initialization stage.</summary>
    void RegisterHtmlHeadContents ();

    /// <summary>Executes rendering-related actions that have to be finished before the control can enter the rendering stage.</summary>
    void PreRender ();
  }
}