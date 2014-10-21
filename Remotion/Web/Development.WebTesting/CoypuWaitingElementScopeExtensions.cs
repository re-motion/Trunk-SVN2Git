using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various extension methods for Coypu's <see cref="ElementScope"/> class, which - after the action has been performed on the DOM element - wait
  /// using a given <see cref="IActionBehavior"/>.
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
        [NotNull] this ElementScope scope,
        [NotNull] Action<ElementScope> action,
        [NotNull] TestObjectContext context,
        [NotNull] IActionBehavior actionBehavior)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("action", action);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("actionBehavior", actionBehavior);

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
        [NotNull] this ElementScope scope,
        [NotNull] TestObjectContext context,
        [NotNull] IActionBehavior actionBehavior)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("actionBehavior", actionBehavior);

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
        [NotNull] this ElementScope scope,
        [NotNull] string value,
        [NotNull] ThenAction thenAction,
        [NotNull] TestObjectContext context,
        [NotNull] IActionBehavior actionBehavior)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("thenAction", thenAction);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("actionBehavior", actionBehavior);

      scope.PerformAction (s => s.FillInWithFixed (value, thenAction), context, actionBehavior);
    }
  }
}