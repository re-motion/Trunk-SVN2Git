using System;
using Coypu;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeExtensions
  {
    /// <summary>
    /// Focuses a link before actually clicking it.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void FocusClick (this ElementScope scope)
    {
      scope.FocusLink();
      scope.Click();
    }

    /// <summary>
    /// Focuses a link.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    private static void FocusLink (this ElementScope scope)
    {
      scope.SendKeys ("");
    }

    /// <summary>
    /// Performs a context click (right click).
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    public static void ContextClick (this ElementScope scope, TestObjectContext context)
    {
      // Hack: Coypu does not directly support the Actions interface, therefore we need to fall back to using Selenium.
      var webDriver = (IWebDriver) context.Browser.Native;
      var nativeScope = (IWebElement) scope.Native;

      var actions = new Actions (webDriver);
      actions.ContextClick (nativeScope);
      actions.Perform();
    }
  }
}