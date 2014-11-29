using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents the ActaNova main menu.
  /// </summary>
  public class ActaNovaMainMenuControlObject : ActaNovaMainFrameControlObject
  {
    public ActaNovaMainMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Selects the menu item using a path through the menu given by <paramref name="menuItems"/>, e.g.: <c>.Select("New", "File")</c>.
    /// </summary>
    public UnspecifiedPageObject Select ([NotNull] params string[] menuItems)
    {
      ArgumentUtility.CheckNotNull ("menuItems", menuItems);

      return Select (menuItems.AsEnumerable());
    }

    /// <summary>
    /// See <see cref="Select(string[])"/>, however, <paramref name="actionOptions"/> may be given which are used instead of the default one.
    /// </summary>
    public UnspecifiedPageObject Select ([NotNull] IEnumerable<string> menuItems, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("menuItems", menuItems);

      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);

      var customAction = new CustomAction (
          this,
          Scope,
          "SelectMainMenu",
          s => RetryUntilTimeout.Run (
              () =>
              {
                // HACK: Use Selenium directly, Coypu does not support Actions API yet.
                var webDriver = (IWebDriver) Context.Browser.Native;
                var nativeMainMenuScope = (IWebElement) s.Native;

                var actions = new ActionsWithWaitSupport (webDriver);
                var timeout = WebTestingConfiguration.Current.SearchTimeout;

                var nativeLastMenuItemScope = AddMenuItemHoverActions (actions, nativeMainMenuScope, menuItems, timeout);
                actions.Click (nativeLastMenuItemScope);

                actions.Perform();
              }));
      customAction.Execute (actualActionOptions);

      return UnspecifiedPage();
    }

    private static IWebElement AddMenuItemHoverActions (
        ActionsWithWaitSupport actions,
        IWebElement nativeMainMenuScope,
        IEnumerable<string> menuItems,
        TimeSpan timeout)
    {
      var nativeMenuItemScope = nativeMainMenuScope;

      foreach (var menuItem in menuItems)
      {
        nativeMenuItemScope = nativeMenuItemScope.FindElement (By.XPath (string.Format ("ul/li[normalize-space(a)='{0}']", menuItem)));
        actions.WaitFor (nativeMenuItemScope, ActionsWithWaitSupport.IsVisible, timeout);
        actions.MoveToElement (nativeMenuItemScope);
      }

      return nativeMenuItemScope;
    }
  }
}