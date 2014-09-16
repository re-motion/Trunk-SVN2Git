using System;
using Coypu;
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
  public abstract class PageObject : TestObject
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

    /// <summary>
    /// Tries to find a <see cref="ControlObject"/> on the page using the given <paramref name="selector"/> and the given
    /// <paramref name="selectionParameters"/>.
    /// </summary>
    /// <typeparam name="TControlSelectionParameters">The control selection parameters type required for this control selector.</typeparam>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
    public ControlObject GetControl<TControlSelectionParameters> (
        [NotNull] IControlSelector<TControlSelectionParameters> selector,
        [NotNull] TControlSelectionParameters selectionParameters)
        where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindControl (Context, selectionParameters);
    }

    /// <summary>
    /// Tries to find a <see cref="ControlObject"/> on the page using the given <paramref name="selector"/> and the given
    /// <paramref name="selectionParameters"/>.
    /// </summary>
    /// <typeparam name="TControlObject">The type of the control to be found.</typeparam>
    /// <typeparam name="TControlSelectionParameters">The control selection parameters type required for this control selector.</typeparam>
    /// <returns>The control object.</returns>
    /// <exception cref="MissingHtmlException">If the element cannot be found.</exception>
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