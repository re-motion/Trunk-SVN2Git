using System;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;
using Keys = OpenQA.Selenium.Keys;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Extension methods for Coypu's <see cref="ElementScope"/> class fixing <see cref="ElementScope.FillInWith"/> and
  /// <see cref="ElementScope.SendKeys"/>.
  /// </summary>
  public static class CoypuElementScopeFillInWithAndSendKeysExtensions
  {
    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatiable version for Coypu's <see cref="ElementScope.SendKeys"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    public static void SendKeysFixed ([NotNull] this ElementScope scope, [NotNull] string value)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);

      const bool clearValue = false;
      scope.FillInWithFixed (value, Then.DoNothing, clearValue);
    }

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="thenAction"><see cref="ThenAction"/> for this action.</param>
    public static void FillInWithFixed ([NotNull] this ElementScope scope, [NotNull] string value, [NotNull] ThenAction thenAction)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("thenAction", thenAction);

      const bool clearValue = true;
      scope.FillInWithFixed (value, thenAction, clearValue);
    }

    /// <summary>
    /// ASP.NET WebForms-ready &amp; IE-compatible version for Coypu's <see cref="ElementScope.FillInWith"/> method.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> on which the action is performed.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="thenAction"><see cref="ThenAction"/> for this action.</param>
    /// <param name="clearValue">Determines whether the old content should be cleared before filling in the new value.</param>
    private static void FillInWithFixed ([NotNull] this ElementScope scope, [NotNull] string value, [NotNull] ThenAction thenAction, bool clearValue)
    {
      // Todo RM-6297: ugly boolean flag in method parameters.

      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("thenAction", thenAction);

      if (!WebTestConfiguration.Current.BrowserIsInternetExplorer())
        scope.FillInWithFixedForNormalBrowsers (value, clearValue);
      else
        scope.FillInWithFixedForInternetExplorer (value, clearValue);

      thenAction (scope);
    }

    /// <summary>
    /// We cannot use Coypu's <see cref="ElementScope.FillInWith"/> here, as it internally calls Selenium's <see cref="IWebElement.Clear"/> which
    /// unfortunately triggers a post back. See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
    /// </summary>
    private static void FillInWithFixedForNormalBrowsers ([NotNull] this ElementScope scope, [NotNull] string value, bool clearValue)
    {
      if (clearValue)
      {
        var clearTextBox = Keys.End + Keys.Shift + Keys.Home + Keys.Shift + Keys.Delete;
        value = clearTextBox + value;
      }

      scope.SendKeys (value);
    }

    /// <summary>
    /// Unfortunately, Selenium's Internet Explorer driver (with native events enabled) does not send required modifier keys when sending keyboard
    /// input (e.g. "@!" would result in "q1"). Therefore we must use <see cref="System.Windows.Forms.SendKeys.SendWait"/> instead.
    /// </summary>
    private static void FillInWithFixedForInternetExplorer ([NotNull] this ElementScope scope, [NotNull] string value, bool clearValue)
    {
      value = SeleniumSendKeysToWindowsFormsSendKeysTransformer.Convert (value);

      if (clearValue)
      {
        const string clearTextBox = "{END}+{HOME}{DEL}";
        value = clearTextBox + value;
      }

      scope.Focus();
      SendKeys.SendWait (value);
    }
  }
}