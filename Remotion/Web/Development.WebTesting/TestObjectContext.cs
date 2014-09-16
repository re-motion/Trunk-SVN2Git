using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context of an arbitrary <see cref="TestObject"/>. Holds various Coypu-based references into the DOM.
  /// </summary>
  public class TestObjectContext
  {
    // Todo RM-6297: Replace auto-properties with readonly backing fields (expresses immutability in a clearer way).

    /// <summary>
    /// Private constructor, use <see cref="New"/> to create a new root <see cref="TestObjectContext"/>.
    /// </summary>
    private TestObjectContext (
        [NotNull] BrowserSession browser,
        [NotNull] BrowserWindow window,
        [NotNull] ElementScope rootElement,
        [NotNull] ElementScope frameRootElement,
        [NotNull] ElementScope scope,
        [CanBeNull] TestObjectContext parentContext)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);
      ArgumentUtility.CheckNotNull ("window", window);
      ArgumentUtility.CheckNotNull ("rootElement", rootElement);
      ArgumentUtility.CheckNotNull ("frameRootElement", frameRootElement);
      ArgumentUtility.CheckNotNull ("scope", scope);

      Browser = browser;
      Window = window;
      RootElement = rootElement;
      FrameRootElement = frameRootElement;
      Scope = scope;
      ParentContext = parentContext;
    }

    /// <summary>
    /// The test object's corresponding browser session.
    /// </summary>
    public BrowserSession Browser { get; private set; }

    /// <summary>
    /// The test object's corresponding browser window.
    /// </summary>
    public BrowserWindow Window { get; private set; }

    /// <summary>
    /// The root element of the window where the test object resides.
    /// </summary>
    public ElementScope RootElement { get; private set; }

    /// <summary>
    /// The root element of the frame where the test object resides.
    /// </summary>
    public ElementScope FrameRootElement { get; private set; }

    /// <summary>
    /// The root element of the test object itself.
    /// </summary>
    public ElementScope Scope { get; private set; }

    /// <summary>
    /// The test object's parent test object's context.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> in case the test object is a root-level component (e.g. a <see cref="PageObject"/>).
    /// </remarks>
    public TestObjectContext ParentContext { get; private set; }

    /// <summary>
    /// Returns a new root <see cref="TestObjectContext"/> for a <see cref="TestObject"/> without a parent.
    /// </summary>
    /// <param name="browser">The browser session (and at the same time the browser window) on which the test object resides.</param>
    /// <returns>A new root test object context.</returns>
    public static TestObjectContext New (BrowserSession browser)
    {
      ArgumentUtility.CheckNotNull ("browser", browser);

      var rootElement = browser.FindCss ("html");
      return new TestObjectContext (browser, browser, rootElement, rootElement, rootElement, null);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForScope (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return new TestObjectContext (Browser, Window, RootElement, FrameRootElement, scope, ParentContext);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForFrame (ElementScope frameScope)
    {
      ArgumentUtility.CheckNotNull ("frameScope", frameScope);

      var frameRootElement = frameScope.FindCss ("html");
      return new TestObjectContext (Browser, Window, RootElement, frameRootElement, frameRootElement, ParentContext);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForNewPage ()
    {
      var rootScope = Window.FindCss ("html");
      return new TestObjectContext (Browser, Window, rootScope, rootScope, rootScope, ParentContext);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForNewWindow (string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return CloneForNewWindow (windowLocator, maximizeWindow: true);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForNewPopupWindow (string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return CloneForNewWindow (windowLocator, maximizeWindow: false);
    }

    private TestObjectContext CloneForNewWindow (string windowLocator, bool maximizeWindow)
    {
      var window = Browser.FindWindow (windowLocator);
      if (maximizeWindow)
        window.MaximiseWindow();

      var rootScope = window.FindCss ("html");
      return new TestObjectContext (Browser, window, rootScope, rootScope, rootScope, this);
    }

    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public TestObjectContext CloneForParentWindow ()
    {
      Assertion.IsNotNull (ParentContext, "No parent context available.");

      return new TestObjectContext (
          Browser,
          ParentContext.Window,
          ParentContext.RootElement,
          ParentContext.RootElement,
          ParentContext.RootElement,
          ParentContext.ParentContext);
    }
  }
}