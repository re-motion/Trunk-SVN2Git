using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Implementation class for <see cref="IActionBehavior"/> and <see cref="IActionBehaviorInternal"/>.
  /// </summary>
  public class ActionBehavior : IActionBehavior, IActionBehaviorInternal
  {
    /// <summary>
    /// Returns a unique ID for an <see cref="ActionBehavior"/> (used for debug output purposes).
    /// </summary>
    private static class ActionBehaviorCounter
    {
      private static int s_counter = 0;

      public static int GetNextID ()
      {
        return Interlocked.Increment (ref s_counter);
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (ActionBehavior));

    private readonly int _debugOutputID;
    private readonly List<IWaitingStrategy> _waitingStrategies;
    private readonly List<Action<TestObjectContext>> _afterClickActions;
    private bool _closesWindow;

    public ActionBehavior ()
    {
      _debugOutputID = ActionBehaviorCounter.GetNextID();
      _waitingStrategies = new List<IWaitingStrategy>();
      _afterClickActions = new List<Action<TestObjectContext>>();
      _closesWindow = false;
    }

    public IActionBehavior WaitFor (IWaitingStrategy waitingStrategy)
    {
      ArgumentUtility.CheckNotNull ("waitingStrategy", waitingStrategy);

      _waitingStrategies.Add (waitingStrategy);

      return this;
    }

    public IActionBehavior AcceptModalDialog ()
    {
      _afterClickActions.Add (
          context =>
          {
            s_log.DebugFormat ("Action {0}: Accepting modal browser dialog.", _debugOutputID);
            context.Browser.AcceptModalDialogFixed (context);
          });

      return this;
    }

    public IActionBehavior CancelModalDialog ()
    {
      _afterClickActions.Add (
          context =>
          {
            s_log.DebugFormat ("Action {0}: Canceling modal browser dialog.", _debugOutputID);
            context.Browser.CancelModalDialogFixed (context);
          });

      return this;
    }

    public IActionBehavior ClosesWindow ()
    {
      _closesWindow = true;
      return this;
    }

    object IActionBehaviorInternal.BeforeAction (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      s_log.DebugFormat ("Action {0} started.", _debugOutputID);

      return _waitingStrategies
          .Select (waitingStrategy => waitingStrategy.OnBeforeActionPerformed (context))
          .ToList();
    }

    void IActionBehaviorInternal.AfterAction (TestObjectContext context, object state)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("state", state);

      ExecuteAfterClickActions (context);

      var newContext = GetNewTestObjectContext (context);
      newContext.EnsureWindowIsActive();

      var states = (List<object>) state;
      for (var i = 0; i < _waitingStrategies.Count; ++i)
      {
        s_log.DebugFormat (
            "Action {0}: Wait using '{1}' on context '{2}'.",
            _debugOutputID,
            _waitingStrategies[i].GetType().Name,
            newContext.ToDebugString().Trim());

        var stateForWaitingStrategy = states[i];
        _waitingStrategies[i].PerformWaitAfterActionPerformed (newContext, stateForWaitingStrategy);
      }

      s_log.DebugFormat ("Action {0} finished.", _debugOutputID);
    }

    private TestObjectContext GetNewTestObjectContext (TestObjectContext context)
    {
      if (_closesWindow)
      {
        s_log.DebugFormat ("Action {0}: Cloning for parent window.", _debugOutputID);
        return context.CloneForParentWindow();
      }

      return context;
    }

    private void ExecuteAfterClickActions (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      foreach (var afterClickAction in _afterClickActions)
        afterClickAction (context);
    }
  }
}