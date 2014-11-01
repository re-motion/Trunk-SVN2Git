using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for a <see cref="ControlObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class ControlObjectContext : WebTestObjectContext
  {
    private readonly PageObjectContext _pageContext;

    /// <summary>
    /// Private constructor, may be obtained only via a <see cref="PageObjectContext"/>.
    /// </summary>
    internal ControlObjectContext ([NotNull] PageObjectContext pageContext, [NotNull] ElementScope scope)
        : base (scope)
    {
      ArgumentUtility.CheckNotNull ("pageContext", pageContext);
      
      _pageContext = pageContext;
    }

    public override BrowserSession Browser
    {
      get { return PageContext.Browser; }
    }

    public override BrowserWindow Window
    {
      get { return PageContext.Window; }
    }

    public override ElementScope RootScope
    {
      get { return PageContext.Scope; }
    }

    /// <summary>
    /// Returns the corresponding <see cref="PageObject"/>'s <see cref="PageObjectContext"/>.
    /// </summary>
    public PageObjectContext PageContext
    {
      get { return _pageContext; }
    }
    
    public override ControlObjectContext CloneForControl (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      return new ControlObjectContext (PageContext, scope);
    }

    public override PageObjectContext CloneForFrame (ElementScope frameScope)
    {
      ArgumentUtility.CheckNotNull ("frameScope", frameScope);

      var frameRootScope = frameScope.FindCss ("html");
      return new PageObjectContext (Browser, Window, frameRootScope, PageContext);
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and replaces the current <see cref="PageObject"/>.
    /// </summary>
    public PageObjectContext CloneForNewPage ()
    {
      var rootScope = Window.GetRootScope();
      return new PageObjectContext (Browser, Window, rootScope, PageContext.ParentContext);
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
      return new PageObjectContext (Browser, window, rootScope, PageContext);
    }
  }
}