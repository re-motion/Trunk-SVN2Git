using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and instead provide a semantic interface. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public abstract class ControlObject : WebTestObject<ControlObjectContext>
  {
    protected ControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Provides access to child controls of the control.
    /// </summary>
    public IControlHost Children
    {
      get { return new ControlHost (Context); }
    }

    /// <summary>
    /// Return's the control's HTML ID.
    /// </summary>
    public string GetHtmlID ()
    {
      return Scope.Id;
    }

    /// <summary>
    /// Convinience method which returns a new <see cref="UnspecifiedPageObject"/>.
    /// </summary>
    /// <returns>A new unspecified page object.</returns>
    protected UnspecifiedPageObject UnspecifiedPage ()
    {
      return new UnspecifiedPageObject (Context);
    }
  }
}