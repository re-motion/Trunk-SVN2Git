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

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Declares the interface needed to provide a spcific rendering implementation for <see cref="IControl"/>.
  /// </summary>
  public interface IRenderer
  {
    /// <summary>Registers script and stylesheet file includes, which has to be done during the initialization stage.</summary>
    void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender);

    /// <summary>Renders the markup for the control into the <paramref name="writer"/> provided during the invocation.</summary>
    void Render (HtmlTextWriter writer);
  }
}