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
    /// See <see cref="Expect{TPageObject}(System.Func{Remotion.Web.Development.WebTesting.TestObjectContext,bool})"/>. It is implicitly assumed that
    /// the actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> () where TPageObject : PageObject
    {
      return Expect<TPageObject> (ctx => true);
    }

    /// <summary>
    /// Expects a page of type <typeparamref name="TPageObject"/>. A conditon given by <paramref name="pageIsShownCondition"/> may check whether the
    /// actual page matches the expected page.
    /// </summary>
    /// <typeparam name="TPageObject">Expected page type.</typeparam>
    /// <param name="pageIsShownCondition">Condition which determines whether the correct page is shown.</param>
    /// <returns>A page object of the expected type.</returns>
    public TPageObject Expect<TPageObject> ([NotNull] Func<TestObjectContext, bool> pageIsShownCondition) where TPageObject : PageObject
    {
      ArgumentUtility.CheckNotNull ("pageIsShownCondition", pageIsShownCondition);

      var newContext = Context.CloneForNewPage();
      pageIsShownCondition (newContext);
      return (TPageObject) Activator.CreateInstance (typeof (TPageObject), new object[] { newContext });
    }
  }
}