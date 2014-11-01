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