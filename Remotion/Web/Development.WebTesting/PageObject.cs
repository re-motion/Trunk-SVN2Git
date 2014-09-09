using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all page objects.
  /// </summary>
  /// <remarks>
  /// See http://martinfowler.com/bliki/PageObject.html or https://code.google.com/p/selenium/wiki/PageObjects for more information on the page object
  /// pattern.
  /// </remarks>
  public abstract class PageObject : WebTestingObject
  {
    /// <summary>
    /// Initializes the page object.
    /// </summary>
    /// <param name="context">The page's context.</param>
    protected PageObject (WebTestingObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the page's title.<br/>
    /// The default implementation returns the corresponding window's title, defined by the HTML &lt;title&gt; tag.
    /// </summary>
    public virtual string Title
    {
      get { return Context.Window.Title; }
    }
  }
}