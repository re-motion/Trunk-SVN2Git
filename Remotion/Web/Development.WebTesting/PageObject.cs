using System;
using JetBrains.Annotations;
using Remotion.Utilities;

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

    public ControlObject GetControl<TControlSelectionParameters> (
        [NotNull] IControlSelector<TControlSelectionParameters> selector,
        [NotNull] TControlSelectionParameters selectionParameters)
        where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindControl (Context, selectionParameters);
    }

    public TControlObject GetControl<TControlObject, TControlSelectionParameters> (
        [NotNull] IControlSelector<TControlObject, TControlSelectionParameters> selector,
        [NotNull] TControlSelectionParameters selectionParameters)
        where TControlObject : ControlObject
        where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindTypedControl (Context, selectionParameters);
    }
  }
}