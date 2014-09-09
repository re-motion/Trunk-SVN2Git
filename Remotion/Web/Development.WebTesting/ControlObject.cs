using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and instead provide a semantic interface. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public class ControlObject : WebTestingObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="context">The control's context.</param>
    public ControlObject (WebTestingObjectContext context)
        : base (context)
    {
    }
  }
}