﻿using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for a <see cref="ControlObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class ControlObjectContext : WebTestObjectContext
  {
    private readonly PageObject _pageObject;

    /// <summary>
    /// Private constructor, may be obtained only via a <see cref="PageObjectContext"/>.
    /// </summary>
    internal ControlObjectContext ([NotNull] PageObject pageObject, [NotNull] ElementScope scope)
        : base (scope)
    {
      ArgumentUtility.CheckNotNull ("pageObject", pageObject);
      
      _pageObject = pageObject;
    }

    public override BrowserSession Browser
    {
      get { return PageObject.Context.Browser; }
    }

    public override BrowserWindow Window
    {
      get { return PageObject.Context.Window; }
    }

    public ElementScope RootScope
    {
      get { return PageObject.Scope; }
    }

    /// <summary>
    /// Returns the <see cref="PageObject"/> on which the <see cref="ControlObject"/> resides.
    /// </summary>
    public PageObject PageObject
    {
      get { return _pageObject; }
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the control object context at hand.
    /// </summary>
    public ControlSelectionContext CloneForControlSelection ()
    {
      return new ControlSelectionContext (PageObject, Scope);
    }

    /// <summary>
    /// Clones the context for another <see cref="ControlObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and on the same page.
    /// </summary>
    /// <param name="scope">The scope of the other <see cref="ControlObject"/>.</param>
    public ControlObjectContext CloneForControl (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return CloneForControl (PageObject, scope);
    }

    public override ControlObjectContext CloneForControl (PageObject pageObject, ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("pageObject", pageObject);
      ArgumentUtility.CheckNotNull ("scope", scope);
      
      return new ControlObjectContext (pageObject, scope);
    }

    public override PageObjectContext CloneForFrame (ElementScope frameScope)
    {
      ArgumentUtility.CheckNotNull ("frameScope", frameScope);

      var frameRootScope = frameScope.FindCss ("html");
      return new PageObjectContext (Browser, Window, frameRootScope, PageObject.Context);
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and replaces the current <see cref="PageObject"/>.
    /// </summary>
    public PageObjectContext CloneForNewPage ()
    {
      var rootScope = Window.GetRootScope();
      return new PageObjectContext (Browser, Window, rootScope, PageObject.Context.ParentContext);
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new window (specified by <paramref name="windowLocator"/>) within the same
    /// <see cref="BrowserSession"/>.
    /// </summary>
    public PageObjectContext CloneForNewWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      var context = CloneForNewWindowInternal (windowLocator);
      context.Window.MaximiseWindow();
      return context;
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new popup window (specified by <paramref name="windowLocator"/>) within the
    /// same <see cref="BrowserSession"/>. In contrast to <see cref="CloneForNewWindow"/>, the window is not maximized.
    /// </summary>
    public PageObjectContext CloneForNewPopupWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return CloneForNewWindowInternal (windowLocator);
    }

    private PageObjectContext CloneForNewWindowInternal (string windowLocator)
    {
      var window = Browser.FindWindow (windowLocator);
      var rootScope = window.GetRootScope();
      return new PageObjectContext (Browser, window, rootScope, PageObject.Context);
    }
  }
}