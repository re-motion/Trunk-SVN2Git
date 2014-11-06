// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for an arbitrary <see cref="WebTestObject{TWebTestObjectContext}"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public abstract class WebTestObjectContext
  {
    private readonly ElementScope _scope;

    /// <summary>
    /// Creates a new context for a given DOM element <paramref name="scope"/>.
    /// </summary>s
    protected WebTestObjectContext ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      _scope = scope;
      _scope.EnsureExistence();
    }

    /// <summary>
    /// The browser session in which the <see cref="WebTestObject{TWebTestObjectContext}"/> resides.
    /// </summary>
    public abstract BrowserSession Browser { get; }

    /// <summary>
    /// The browser window on which the <see cref="WebTestObject{TWebTestObjectContext}"/> resides.
    /// </summary>
    public abstract BrowserWindow Window { get; }

    /// <summary>
    /// The scope of the <see cref="WebTestObject{TWebTestObjectContext}"/>.
    /// </summary>
    public ElementScope Scope
    {
      get { return _scope; }
    }

    /// <summary>
    /// Clones the context for another <see cref="ControlObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and on the given <paramref name="pageObject"/>.
    /// </summary>
    /// <param name="pageObject">The <see cref="PageObject"/> on which the <see cref="ControlObject"/> resides.</param>
    /// <param name="scope">The scope of the other <see cref="ControlObject"/>.</param>
    public abstract ControlObjectContext CloneForControl ([NotNull] PageObject pageObject, [NotNull] ElementScope scope);

    /// <summary>
    /// Clones the context for a child <see cref="PageObject"/> which represents an IFRAME on the page.
    /// </summary>
    /// <param name="frameScope">The scope of the <see cref="PageObject"/> representing the IFRAME.</param>
    public abstract PageObjectContext CloneForFrame ([NotNull] ElementScope frameScope);
  }
}