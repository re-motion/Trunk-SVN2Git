using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class of <see cref="PageObject"/> and <see cref="ControlObject"/>, holding common 
  /// </summary>
  public abstract class WebTestingObject // TODO: Naming?
  {
    /// <summary>
    /// The web testing object's context.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global : May be used by web tests to directly interact with the underlying web testing framework.
    public WebTestingObjectContext Context { get; private set; }

    /// <summary>
    /// Shortcut for <c>Context.Scope</c>.
    /// </summary>
    public ElementScope Scope
    {
      get { return Context.Scope; }
    }

    /// <summary>
    /// Initializes the web testing object.
    /// </summary>
    /// <remarks>
    /// The web testing object must be provided with a valid <see cref="WebTestingObjectContext"/> which already references its HTML root element in
    /// the <see cref="WebTestingObjectContext.Scope"/> property. This implies that finding the web testing object within the parent is the
    /// responsibility of the parent!
    /// </remarks>
    /// <param name="context">The web testing object's context.</param>
    protected WebTestingObject (WebTestingObjectContext context)
    {
      Context = context;
    }
  }
}