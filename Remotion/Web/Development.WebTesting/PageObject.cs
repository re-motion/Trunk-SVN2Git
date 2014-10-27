using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all page objects.<br/>
  /// All derived page object types must provide a constructor with a single <see cref="TestObjectContext"/> argument!
  /// </summary>
  /// <remarks>
  /// See http://martinfowler.com/bliki/PageObject.html or https://code.google.com/p/selenium/wiki/PageObjects for more information on the page object
  /// pattern.
  /// </remarks>
  public abstract class PageObject : TestObject, IControlHost
  {
    protected PageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Gets the page's title.<br/>
    /// The default implementation returns the corresponding window's title, defined by the HTML &lt;title&gt; tag.
    /// </summary>
    public virtual string GetTitle ()
    {
      return Context.Window.Title;
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return controlSelectionCommand.Select (Context);
    }

    /// <summary>
    /// Returns a new <see cref="IActionBehavior"/> object.
    /// </summary>
    protected IActionBehavior Behavior
    {
      // Note: property exists for "syntactical sugar" only, therefore returning a new object in the get accessor is okay.
      get { return new ActionBehavior(); }
    }
  }
}