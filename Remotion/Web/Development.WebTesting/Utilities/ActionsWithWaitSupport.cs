using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Enhanced version of Selenium's <see cref="Actions"/> class. It enables us to add WaitFor predicates after actions.
  /// </summary>
  public class ActionsWithWaitSupport : Actions
  {
    private class ActionAdapter : IAction
    {
      private readonly Action _action;

      public ActionAdapter (Action action)
      {
        _action = action;
      }

      public void Perform ()
      {
        _action();
      }
    }

    /// <summary>
    /// Pre-defined predicate for "is element visible" condition.
    /// </summary>
    public static readonly Func<IWebElement, bool> IsVisible = we => we.Displayed;

    private readonly IWebDriver _driver;

    public ActionsWithWaitSupport (IWebDriver driver)
        : base (driver)
    {
      _driver = driver;
    }

    /// <summary>
    /// Adds a WaitFor predicate after the last added action.
    /// // TODO RM-6297: Refactor predicate to DSL in order to be more along the style of Coypu?
    /// </summary>
    /// <param name="webElement">The web element we want to pass to the predicate.</param>
    /// <param name="predicate">The wait condition.</param>
    /// <param name="timeout">A maximum timeout until the condition must be fulfilled.</param>
    /// <returns>Itself, allows chaining of calls.</returns>
    /// <exception cref="WebDriverException">Depending on the predicate, a variety of <see cref="WebDriverException"/>s may be thrown.
    /// <see cref="StaleElementReferenceException"/> may only appear after the timeout has been reached, before the timeout, it is ignored and the
    /// check of the condition is retried.</exception>
    public ActionsWithWaitSupport WaitFor (IWebElement webElement, Func<IWebElement, bool> predicate, TimeSpan timeout)
    {
      AddAction (
          new ActionAdapter (
              () =>
              {
                var webDriverWait = new WebDriverWait (_driver, timeout);
                webDriverWait.IgnoreExceptionTypes (typeof (StaleElementReferenceException));
                webDriverWait.Until (_ => predicate (webElement));
              }));

      return this;
    }
  }
}