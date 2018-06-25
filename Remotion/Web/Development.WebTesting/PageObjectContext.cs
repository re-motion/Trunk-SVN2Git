﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for a <see cref="PageObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class PageObjectContext : WebTestObjectContext
  {
    private readonly IBrowserSession _browser;
    private readonly BrowserWindow _window;
    private readonly IRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    private readonly PageObjectContext _parentContext;

    /// <summary>
    /// Private constructor, use <see cref="New"/> to create a new root <see cref="PageObjectContext"/>.
    /// </summary>
    internal PageObjectContext (
        [NotNull] IBrowserSession browser,
        [NotNull] BrowserWindow window,
        [NotNull] IRequestErrorDetectionStrategy requestErrorDetectionStrategy,
        [NotNull] ElementScope scope,
        [CanBeNull] PageObjectContext parentContext)
        : base (scope)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("requestErrorDetectionStrategy", requestErrorDetectionStrategy);

      _browser = browser;
      _window = window;
      _requestErrorDetectionStrategy = requestErrorDetectionStrategy;
      _parentContext = parentContext;
    }

    /// <summary>
    /// Returns a new root context for a <see cref="PageObject"/> without a parent (e.g. a parent frame).
    /// </summary>
    /// <param name="browser">The <see cref="IBrowserSession"/> on which the <see cref="PageObject"/> resides.</param>
    /// <param name="requestErrorDetectionStrategy">The <see cref="IRequestErrorDetectionStrategy"/> which defines the used request error detection.</param>
    /// <returns>A new root context.</returns>
    public static PageObjectContext New ([NotNull] IBrowserSession browser, [NotNull] IRequestErrorDetectionStrategy requestErrorDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("requestErrorDetectionStrategy", requestErrorDetectionStrategy);

      var scope = browser.Window.GetRootScope();

      requestErrorDetectionStrategy.CheckPageForErrors (scope);

      return new PageObjectContext (browser, browser.Window, requestErrorDetectionStrategy, scope, null);
    }

    /// <inheritdoc/>
    public override IBrowserSession Browser
    {
      get { return _browser; }
    }

    /// <inheritdoc/>
    public override BrowserWindow Window
    {
      get { return _window; }
    }

    /// <summary>
    /// Returns the <see cref="PageObject"/>'s parent context, e.g. a <see cref="PageObject"/> representing the parent frame or the parent window.
    /// This property returns <see langword="null"/> for root contexts.
    /// </summary>
    public PageObjectContext ParentContext
    {
      get { return _parentContext; }
    }
    
    /// <summary>
    /// Returns the <see cref="IRequestErrorDetectionStrategy"/> implementation passed in the <see cref="PageObjectContext"/>constructor.
    /// </summary>
    public IRequestErrorDetectionStrategy RequestErrorDetectionStrategy
    {
      get { return _requestErrorDetectionStrategy; }
    }

    /// <summary>
    /// Clones the context for a new <see cref="IBrowserSession"/>.
    /// </summary>
    /// <param name="browserSession">The new <see cref="IBrowserSession"/>.</param>
    public PageObjectContext CloneForSession ([NotNull] IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull ("browserSession", browserSession);

      var rootScope = browserSession.Window.GetRootScope();

      var pageObjectContext = new PageObjectContext (browserSession, browserSession.Window, RequestErrorDetectionStrategy, rootScope, this);

      RequestErrorDetectionStrategy.CheckPageForErrors (rootScope);

      return pageObjectContext;
    }

    /// <summary>
    /// Clones the context for a child <see cref="PageObject"/> which represents an IFRAME on the page.
    /// </summary>
    /// <param name="frameScope">The scope of the <see cref="PageObject"/> representing the IFRAME.</param>
    public PageObjectContext CloneForFrame ([NotNull] ElementScope frameScope)
    {
      ArgumentUtility.CheckNotNull ("frameScope", frameScope);

      var frameRootElement = frameScope.FindCss ("html");

      var pageObjectContext = new PageObjectContext (Browser, Window, RequestErrorDetectionStrategy, frameRootElement, this);

      RequestErrorDetectionStrategy.CheckPageForErrors (frameRootElement);

      return pageObjectContext;
    }

    /// <summary>
    /// Creates a <see cref="ControlObjectContext"/> for a <see cref="ControlObject"/> which resides on the page.
    /// </summary>
    /// <param name="pageObject">
    /// As the <see cref="PageObjectContext"/> does not know about the <see cref="PageObject"/> it belongs to, the <paramref name="pageObject"/> must
    /// be specified.
    /// </param>
    /// <param name="scope">The scope of the <see cref="ControlObject"/>.</param>
    public ControlObjectContext CloneForControl ([NotNull] PageObject pageObject, [NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("pageObject", pageObject);
      ArgumentUtility.CheckNotNull ("scope", scope);

      try
      {
        return new ControlObjectContext (pageObject, scope);
      }
      catch (Exception)
      {
        RequestErrorDetectionStrategy.CheckPageForErrors (pageObject.Context.Scope);

        throw;
      }
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the page object context at hand. As the <see cref="PageObjectContext"/> does not
    /// know about the <see cref="PageObject"/> it belongs to, the <paramref name="pageObject"/> must be specified.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForControlSelection"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the <see cref="PageObject"/> is only intended for use from within implementations of <see cref="IControlSelector"/>, which in turn
    /// perform their own error page detection via <see cref="ControlObjectContext"/>.<see cref="CloneForControl"/>.
    /// </remarks>
    public ControlSelectionContext CloneForControlSelection (PageObject pageObject)
    {
      var cloneForControlSelection = new ControlSelectionContext (pageObject, Scope);

      // No error page detection. See remarks documentation on this method.

      return cloneForControlSelection;
    }
  }
}