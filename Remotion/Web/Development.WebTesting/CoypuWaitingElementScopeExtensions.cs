using System;
using Coypu;
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
    /// its <paramref name="context"/>) using the given <paramref name="waitStrategy"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// /// <param name="action">Action to be performed on the DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="waitStrategy">Wait strategy for proper waiting.</param>
    public static void PerformActionUsingWaitStrategy (
        this ElementScope scope,
        Action<ElementScope> action,
        TestObjectContext context,
        IWaitingStrategy waitStrategy)
    {
      var state = waitStrategy.OnBeforeActionPerformed (context);
      action (scope);
      waitStrategy.PerformWaitAfterActionPerformed (context, state);
    }

    /// <summary>
    /// Performs a click on a DOM element (given by <paramref name="scope"/>), which is part of a control object (represented by its
    /// <paramref name="context"/>) using the given <paramref name="waitStrategy"/>.
    /// </summary>
    /// <param name="scope">The DOM element.</param>
    /// <param name="context">The corresponding control object's context.</param>
    /// <param name="waitStrategy">Wait strategy for proper waiting.</param>
    public static void ClickAndWait (
        this ElementScope scope,
        TestObjectContext context,
        IWaitingStrategy waitStrategy)
    {
      PerformActionUsingWaitStrategy (scope, s => s.FocusClick(), context, waitStrategy);
    }

    public static void FillWithAndWait (
        this ElementScope scope,
        string value,
        ThenAction thenAction,
        TestObjectContext context,
        IWaitingStrategy waitStrategy)
    {
      Action<ElementScope> action = s =>
      {
        s.FillInWith (value);
        thenAction (s);
      };

      PerformActionUsingWaitStrategy (scope, action, context, waitStrategy);
    }
  }
}