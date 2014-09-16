using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// <para>
  /// We must ensure that once an element has been found by the Coypu engine, it is not invalidated anymore (e.g. due to another (partial) page
  /// reload). This may occur due to single or multiple asynchronous post backs in ASP.NET Web Forms applications. A race condition occurs, if the
  /// element on the following page is already present on the current page.
  /// </para>
  /// <para>
  /// To overcome the race condition we need to use a <see cref="IWaitingStrategy"/> after interacting with an element in order to
  /// wait for the next page to be present. Strategies may range form doing nothing (e.g. if the interaction doesn't trigger any DOM-changing
  /// behavior) to waiting for specific changes in the DOM tree which signal us that the following page is present.
  /// </para>
  /// <para>
  /// Due to asynchronous JavaScript scripts we can theoretically still run into race conditions, i.e. if parts of the DOM are replaced by completely
  /// different elements, however, possessing the same IDs / labels / etc. This is a not-supported scenario.
  /// </para>
  /// </summary>
  public interface IWaitingStrategy
  {
    /// <summary>
    /// This method is called immediately before the actual interaction with the DOM element (e.g. a click) is performed. Therefore we are sure to
    /// reference the current (old) page and DOM tree. This method allows us to retrieve a sequence counter or other information from the DOM which
    /// we need to compare with the elements on the following (new) page, in order to determine whether the new page is already present.
    /// </summary>
    /// <param name="context">The context of the web testing object on which the interaction is performed afterwards.</param>
    /// <returns>A state object which must be passed to <see cref="PerformWaitAfterActionPerformed"/>.</returns>
    object OnBeforeActionPerformed (TestObjectContext context);

    /// <summary>
    /// This method is called after the actual interaction with the DOM element (e.g. a click) has been performed. We are not yet sure if we still
    /// reference the old or the new page. The task of this method is to block until the strategy is able to guarantee that elements found in the DOM
    /// are not going to be invalidated anymore (e.g. due to page reloads, AJAX calls which replace/remove DOM elements, etc.) before the next user
    /// interaction takes place.
    /// </summary>
    /// <param name="context">The context of the web testing object on which the interaction has been performed.</param>
    /// <param name="state">The state object returned by <see cref="OnBeforeActionPerformed"/>.</param>
    void PerformWaitAfterActionPerformed (TestObjectContext context, object state);
  }
}