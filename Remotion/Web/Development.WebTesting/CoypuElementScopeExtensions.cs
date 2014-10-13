using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeExtensions
  {
    /// <summary>
    /// Find an element with the given <paramref name="tagSelector"/> bearing a given diagnostic metadata attribute name/value combination.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the diagnostic metadata attributes.</param>
    /// <param name="diagnosticMetadataAttributeName">The diagnostic metadata attribute name.</param>
    /// <param name="diagnosticMetadataAttributeValue">The diagnostic metadata attribute value.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindDMA (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        [NotNull] string diagnosticMetadataAttributeName,
        [NotNull] string diagnosticMetadataAttributeValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeName", diagnosticMetadataAttributeName);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeValue", diagnosticMetadataAttributeValue);

      var cssSelector = string.Format ("{0}[{1}='{2}']", tagSelector, diagnosticMetadataAttributeName, diagnosticMetadataAttributeValue);
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Finds the first &lt;a&gt; element within the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindLink ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return scope.FindCss ("a");
    }

    /// <summary>
    /// Focuses a link before actually clicking it.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void FocusClick ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.FocusLink();
      scope.Click();
    }

    /// <summary>
    /// Focuses a link.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    private static void FocusLink ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.SendKeys ("");
    }

    /// <summary>
    /// Performs a context click (right click).
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    public static void ContextClick ([NotNull] this ElementScope scope, [NotNull] TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);

      // Hack: Coypu does not directly support the Actions interface, therefore we need to fall back to using Selenium.
      var webDriver = (IWebDriver) context.Browser.Native;
      var nativeScope = (IWebElement) scope.Native;

      var actions = new Actions (webDriver);
      actions.ContextClick (nativeScope);
      actions.Perform();
    }
  }
}