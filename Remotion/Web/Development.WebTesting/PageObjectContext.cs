using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for a <see cref="PageObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class PageObjectContext : WebTestObjectContext
  {
    private readonly BrowserSession _browser;
    private readonly BrowserWindow _window;
    private readonly PageObjectContext _parentContext;

    /// <summary>
    /// Private constructor, use <see cref="New"/> to create a new root <see cref="PageObjectContext"/>.
    /// </summary>
    internal PageObjectContext (
        [NotNull] BrowserSession browser,
        [NotNull] BrowserWindow window,
        [NotNull] ElementScope scope,
        [CanBeNull] PageObjectContext parentContext)
        : base (scope)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("window", window);

      _browser = browser;
      _window = window;
      _parentContext = parentContext;
    }

    /// <summary>
    /// Returns a new root context for a <see cref="PageObject"/> without a parent (e.g. a parent frame).
    /// </summary>
    /// <param name="browser">The <see cref="BrowserSession"/> on which the <see cref="PageObject"/> resides.</param>
    /// <returns>A new root context.</returns>
    public static PageObjectContext New ([NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      var scope = browser.GetRootScope();
      return new PageObjectContext (browser, browser, scope, null);
    }

    public override BrowserSession Browser
    {
      get { return _browser; }
    }

    public override BrowserWindow Window
    {
      get { return _window; }
    }

    /// <summary>
    /// Returns the <see cref="PageObject"/>'s parent context, e.g. a <see cref="PageObject"/> respresenting the parent frame or the parent window. This
    /// property returns <see langword="null"/> for root contexts.
    /// </summary>
    public PageObjectContext ParentContext
    {
      get { return _parentContext; }
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the page object context at hand. As the <see cref="PageObjectContext"/> does not
    /// know about the <see cref="PageObject"/> it belongs to, the <paramref name="pageObject"/> must be specified.
    /// </summary>
    public ControlSelectionContext CloneForControlSelection (PageObject pageObject)
    {
      return new ControlSelectionContext (pageObject, Scope);
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

      var frameRootElement = frameScope.FindCss ("html");
      return new PageObjectContext (Browser, Window, frameRootElement, this);
    }
  }
}