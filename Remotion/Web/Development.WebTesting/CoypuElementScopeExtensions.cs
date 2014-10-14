using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Coypu;
using Coypu.Drivers;
using JetBrains.Annotations;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Remotion.Utilities;
using Keys = OpenQA.Selenium.Keys;

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
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Selenium's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="thenAction"><see cref="ThenAction"/> for this action.</param>
    public static void FillInWithFixed (
        [NotNull] this ElementScope scope,
        [NotNull] TestObjectContext context,
        [NotNull] string value,
        [NotNull] ThenAction thenAction)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("thenAction", thenAction);

      if (context.Configuration.Browser != Browser.InternetExplorer)
        scope.FillInWithFixedNormalBrowser (value);
      else
        scope.FillInWithFixedInternetExplorer (value);

      thenAction (scope);
    }

    /// <summary>
    /// We cannot use Coypu's <see cref="ElementScope.FillInWith"/> here, as it internally calls Selenium's <see cref="IWebElement.Clear"/> which
    /// unfortunately triggers a post back. See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
    /// </summary>
    private static void FillInWithFixedNormalBrowser ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      var clearTextBox = Keys.End + Keys.Shift + Keys.Home + Keys.Shift + Keys.Delete;
      scope.SendKeys (clearTextBox + value);
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) does not send required modifier keys when sending keyboard
    /// input (e.g. "@!" would result in "q1"). Therefore we must use <see cref="SendKeys.SendWait"/> instead.
    /// </summary>
    private static void FillInWithFixedInternetExplorer (
        [NotNull] this ElementScope scope,
        [NotNull] string value)
    {
      const string clearTextBox = "{END}+{HOME}{DEL}";

      scope.Focus();
      Console.WriteLine (PrepareValueForSendKeysAPI (value));
      SendKeys.SendWait (clearTextBox + PrepareValueForSendKeysAPI (value));
    }

    private static string PrepareValueForSendKeysAPI (string value)
    {
      var charactersToEncloseForSendKeys = new[] { "+", "^", "%", "~", "(", ")", "'", "[", "]", "{", "}" };
      var charactersToEncloseForRegex = new[] { '.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '\\' };

      for (var i = 0; i < charactersToEncloseForSendKeys.Length; ++i)
      {
        var c = charactersToEncloseForSendKeys[i][0];
        if (charactersToEncloseForRegex.Contains (c))
          charactersToEncloseForSendKeys[i] = "\\" + charactersToEncloseForSendKeys[i];
      }

      var charactersToEncloseForSendKeysPattern = string.Join ("|", charactersToEncloseForSendKeys);
      return Regex.Replace (value, charactersToEncloseForSendKeysPattern, match => "{" + match.Value + "}");
    }

    /// <summary>
    /// Focuses an element.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void Focus ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      var webElement = (IWebElement) scope.Native;

      if (webElement.TagName == "input" || webElement.TagName == "textarea")
        webElement.Click();
      else
        webElement.SendKeys ("");
    }

    /// <summary>
    /// Focuses a link before actually clicking it.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    public static void FocusClick ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.Focus();
      scope.Click();
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