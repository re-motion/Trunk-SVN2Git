using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Represents a generic page object which is returned by control object interactions (e.g. click on an HtmlAnchorControlObject). The user of
  /// the framework must specify which actual page object he is expecting by calling one of the various Expect* methods or extension methods.
  /// </summary>
  public class UnspecifiedPageObject : TestObject
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">Previous context of the control object triggering the interaction.</param>
    public UnspecifiedPageObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// See <see cref="Expect{TPageObject}(System.Func{TPageObject,bool})"/>. It is implicitly assumed that the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> () where TPageObject : PageObject
    {
      return Expect<TPageObject> (ctx => true);
    }

    /// <summary>
    /// Expects a page of type <typeparamref name="TPageObject"/>. A conditon given by <paramref name="pageIsShownAssertion"/> may check whether the
    /// actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> ([NotNull] Func<TPageObject, bool> pageIsShownAssertion) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewPage();
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    /// <summary>
    /// Expects a page of type <typeparamref name="TPageObject"/> - on a new window with title <see cref="windowLocator"/>. A conditon given by
    /// <paramref name="pageIsShownAssertion"/> may check whether the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new window.</param>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectOnNewWindow<TPageObject> (string windowLocator, [NotNull] Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewWindow (windowLocator);
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    /// <summary>
    /// Expects a page of type <typeparamref name="TPageObject"/> - on a new popup window with title <see cref="windowLocator"/>. A conditon given by
    /// <paramref name="pageIsShownAssertion"/> may check whether the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="windowLocator">Title of the new popup window.</param>
    /// <param name="pageIsShownAssertion">Condition which asserts whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject ExpectOnNewPopupWindow<TPageObject> (string windowLocator, [NotNull] Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("pageIsShownAssertion", pageIsShownAssertion);

      var newContext = Context.CloneForNewPopupWindow (windowLocator);
      return AssertPageIsShownAndReturnNewPageObject (newContext, pageIsShownAssertion);
    }

    private static TPageObject AssertPageIsShownAndReturnNewPageObject<TPageObject> (
        TestObjectContext newContext,
        Func<TPageObject, bool> pageIsShownAssertion)
        where TPageObject : PageObject
    {
      var pageObject = (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { newContext });
      pageIsShownAssertion (pageObject);
      return pageObject;
    }
  }
}