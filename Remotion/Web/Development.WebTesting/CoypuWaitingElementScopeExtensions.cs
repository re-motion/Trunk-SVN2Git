using System;
using Coypu;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class, which - after the action has been performed on the DOM element - wait
  /// using a given <see cref="IWaitingStrategy"/>.
  /// </summary>
  public static class CoypuWaitingElementScopeExtensions
  {
    /// <summary>
    /// Performs an <paramref name="action"/> on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by
    /// its <paramref name="context"/>) using the given <paramref name="actionBehavior"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// /// <param name="action">Action to be performed on the DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="actionBehavior"><see cref="IActionBehavior"/> for this action.</param>
    public static void PerformAction (
        this ElementScope scope,
        Action<ElementScope> action,
        TestObjectContext context,
        IActionBehavior actionBehavior)
    {
      var actionBehaviorInternal = actionBehavior as IActionBehaviorInternal;
      Assertion.IsNotNull (actionBehaviorInternal, "IActionBehavior must also - explicitly - implement IActionBehaviorInternal.");

      var state = actionBehaviorInternal.BeforeAction (context);
      action (scope);
      actionBehaviorInternal.AfterAction (context, state);
    }

    /// <summary>
    /// Performs a click on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by its
    /// <paramref name="context"/>) using the given <paramref name="actionBehavior"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="actionBehavior"><see cref="IActionBehavior"/> for this action.</param>
    public static void ClickAndWait (
        this ElementScope scope,
        TestObjectContext context,
        IActionBehavior actionBehavior)
    {
      scope.PerformAction (s => s.FocusClick(), context, actionBehavior);
    }

    /// <summary>
    /// Fills the given DOM input element (given by <paramref name="scope"/>), which is part of a control object (represented by its
    /// <paramref name="context"/>) with the given <paramref name="value"/> using the given <paramref name="thenAction"/> and
    /// <paramref name="actionBehavior"/>.
    /// </summary>
    /// <param name="scope">The DOM input element.</param>
    /// <param name="value">The value to fill in.</param>
    /// <param name="thenAction"><see cref="ThenAction"/> for this action.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="actionBehavior"><see cref="IActionBehavior"/> for this action.</param>
    public static void FillWithAndWait (
        this ElementScope scope,
        string value,
        ThenAction thenAction,
        TestObjectContext context,
        IActionBehavior actionBehavior)
    {
      Action<ElementScope> action = s =>
      {
        // HACK: We cannot use s.FillInWith(value) here, as it internally calls s.Clear() which unfortunately triggers a post back.
        // See https://groups.google.com/forum/#!topic/selenium-users/fBWLmL8iEzA for more information.
        s.SendKeys (Keys.End + Keys.Shift + Keys.Home + Keys.Shift + Keys.Delete + value);
        thenAction (s);
      };

      scope.PerformAction (action, context, actionBehavior);
    }
  }
}