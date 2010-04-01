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

namespace Remotion.Web.Legacy.Factories
{
  /// <summary>
  /// Responsible for creating objects that implement <see cref="IResourceUrl"/> in quirks mode rendering.
  /// Uses a <see cref="ResourceTheme"/> with the theme-name <c>Legacy</c> for <see cref="ThemedResourceUrl"/>.
  /// <seealso cref="ResourceUrl"/>
  /// <seealso cref="ThemedResourceUrl"/>
  /// </summary>
  public class QuirksModeResourceUrlFactory : IResourceUrlFactory
  {
    private readonly ResourceTheme _resourceTheme = new ResourceTheme ("Legacy");

    public QuirksModeResourceUrlFactory ()
    {
    }

    public IResourceUrl CreateResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      return new ResourceUrl (definingType, resourceType, relativeUrl);
    }

    public IResourceUrl CreateThemedResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      return new ThemedResourceUrl (definingType, resourceType, _resourceTheme, relativeUrl);
    }
  }
}