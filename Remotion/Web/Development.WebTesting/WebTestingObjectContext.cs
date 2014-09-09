using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context of an arbitrary web testing object. Holds various Coypu-based references into the DOM.
  /// </summary>
  public class WebTestingObjectContext
  {
    /// <summary>
    /// The web testing object's corresponding browser session.
    /// </summary>
    public BrowserSession Browser { get; private set; }

    /// <summary>
    /// The web testing object's corresponding browser window.
    /// </summary>
    public BrowserWindow Window { get; private set; }

    /// <summary>
    /// The root element of the window where the web testing object resides.
    /// </summary>
    public ElementScope RootElement { get; private set; }

    /// <summary>
    /// The root element of the frame where the web testing object resides.
    /// </summary>
    public ElementScope FrameRootElement { get; private set; }

    /// <summary>
    /// The root element of the web testing object itself.
    /// </summary>
    public ElementScope Scope { get; private set; }

    /// <summary>
    /// The web testing object's parent web testing object.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> in case the web testing object is a root-level component (e.g. a <see cref="PageObject"/>).
    /// </remarks>
    public WebTestingObject Parent { get; private set; }

    /// <summary>
    /// Returns a new root <see cref="WebTestingObjectContext"/> for a <see cref="WebTestingObject"/> without a parent.
    /// </summary>
    /// <param name="browser">The browser session and at the same time the browser window on which the web testing object resides.</param>
    /// <returns>A new root web testing object context.</returns>
    public static WebTestingObjectContext New (BrowserSession browser)
    {
      var rootElement = browser.FindCss ("html");
      return new WebTestingObjectContext (browser, browser, rootElement, rootElement, rootElement, null);
    }

    private WebTestingObjectContext (
        BrowserSession browser,
        BrowserWindow window,
        ElementScope rootElement,
        ElementScope frameRootElement,
        ElementScope scope,
        WebTestingObject parentComponent)
    {
      Browser = browser;
      Window = window;
      RootElement = rootElement;
      FrameRootElement = frameRootElement;
      Scope = scope;
      Parent = parentComponent;
    }
  }
}