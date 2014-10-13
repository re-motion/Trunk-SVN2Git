using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents the ActaNova main menu.
  /// </summary>
  public class ActaNovaMainMenu : ActaNovaMainFrameControlObject
  {
    public ActaNovaMainMenu ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Selects the menu item using a path through the menu given by <paramref name="menuItems"/>, e.g.: <c>.Select("New", "File")</c>.
    /// </summary>
    public UnspecifiedPageObject Select (params string[] menuItems)
    {
      return Select (menuItems.AsEnumerable());
    }

    /// <summary>
    /// See <see cref="Select(string[])"/>, however, a <paramref name="actionBehavior"/> may be given which is used instead of the default one.
    /// </summary>
    public UnspecifiedPageObject Select (IEnumerable<string> menuItems, IActionBehavior actionBehavior = null)
    {
      var actualActionBehavior = GetActualActionBehavior (actionBehavior) as IActionBehaviorInternal;
      if (actualActionBehavior == null)
        Assertion.IsNotNull (actualActionBehavior, "");

      var waitingStrategyState = actualActionBehavior.BeforeAction (Context);

      // HACK: Use Selenium directly, Coypu does not support Actions API yet.
      var webDriver = (IWebDriver) Context.Browser.Native;
      var nativeMainMenuScope = (IWebElement) Scope.Native;

      var actions = new ActionsWithWaitSupport (webDriver);
      var timeout = Context.Configuration.SearchTimeout;

      var nativeLastMenuItemScope = AddMenuItemHoverActions (actions, nativeMainMenuScope, menuItems, timeout);
      AddClickOnLastMenuItemAction (actions, nativeLastMenuItemScope, actualActionBehavior, waitingStrategyState, timeout);

      actions.Perform();

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
        nativeMenuItemScope = nativeMenuItemScope.FindElement (By.XPath (string.Format ("ul/li[a[contains(.,'{0}')]]", menuItem)));
        actions.WaitFor (nativeMenuItemScope, ActionsWithWaitSupport.IsVisible, timeout);
        actions.MoveToElement (nativeMenuItemScope);
      }

      return nativeMenuItemScope;
    }

    private void AddClickOnLastMenuItemAction (
        ActionsWithWaitSupport actions,
        IWebElement nativeLastMenuItemScope,
        IActionBehaviorInternal actionBehavior,
        object waitingStrategyState,
        TimeSpan timeout)
    {
      actions.Click (nativeLastMenuItemScope);
      actions.WaitFor (
          nativeLastMenuItemScope,
          _ =>
          {
            actionBehavior.AfterAction (Context, waitingStrategyState);
            return true;
          },
          timeout);
    }
  }
}