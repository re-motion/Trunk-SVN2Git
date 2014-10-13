using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting
{
  // Todo RM-6297: Improve class design.

  /// <summary>
  /// Todo RM-6297
  /// </summary>
  public interface IActionBehavior
  {
    /// <summary>
    /// Todo RM-6297
    /// </summary>
    IActionBehavior AcceptModalDialog ();

    /// <summary>
    /// Todo RM-6297
    /// </summary>
    IActionBehavior CancelModalDialog ();

    /// <summary>
    /// Todo RM-6297
    /// </summary>
    IActionBehavior ClosesWindow ();

    /// <summary>
    /// Todo RM-6297
    /// </summary>
    IActionBehavior WaitFor ([NotNull] IWaitingStrategy waitingStrategy);
  }

  /// <summary>
  /// Todo RM-6297
  /// </summary>
  public interface IActionBehaviorInternal
  {
    /// <summary>
    /// Todo RM-6297
    /// </summary>
    object BeforeAction ([NotNull] TestObjectContext context);

    /// <summary>
    /// Todo RM-6297
    /// </summary>
    void AfterAction ([NotNull] TestObjectContext context, [NotNull] object behaviorState);
  }

  /// <summary>
  /// Todo RM-6297
  /// </summary>
  public class ActionBehavior : IActionBehavior, IActionBehaviorInternal
  {
    private static class ActionBehaviorCounter
    {
      public static int Counter = 1;
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (ActionBehavior));

    private readonly List<Action<TestObjectContext>> _afterClickActions = new List<Action<TestObjectContext>>();
    private readonly List<IWaitingStrategy> _waitingStrategies = new List<IWaitingStrategy>();
    private bool _closesWindow = false;
    private int _id = ActionBehaviorCounter.Counter++;

    public IActionBehavior AcceptModalDialog ()
    {
      _afterClickActions.Add (
          ctx =>
          {
            s_log.DebugFormat ("Action {0}: Accepting modal dialog.", _id);
            ctx.Window.AcceptModalDialog();
          });
      return this;
    }

    public IActionBehavior CancelModalDialog ()
    {
      _afterClickActions.Add (
          ctx =>
          {
            s_log.DebugFormat ("Action {0}: Canceling modal dialog.", _id);
            ctx.Window.CancelModalDialog();
          });
      return this;
    }

    public IActionBehavior ClosesWindow ()
    {
      _closesWindow = true;
      return this;
    }

    public IActionBehavior WaitFor (IWaitingStrategy waitingStrategy)
    {
      ArgumentUtility.CheckNotNull ("waitingStrategy", waitingStrategy);

      _waitingStrategies.Add (waitingStrategy);

      return this;
    }

    object IActionBehaviorInternal.BeforeAction (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      s_log.DebugFormat ("Action {0} started.", _id);

      return _waitingStrategies
          .Select (waitingStrategy => waitingStrategy.OnBeforeActionPerformed (context))
          .ToList();
    }

    void IActionBehaviorInternal.AfterAction (TestObjectContext context, object behaviorState)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("behaviorState", behaviorState);

      ExecuteAfterClickActions(context);
      
      var newContext = GetNewTestObjectContext(context);
      newContext.EnsureWindowIsActive ();

      var states = (List<object>) behaviorState;
      for (var i = 0; i < _waitingStrategies.Count; ++i)
      {
        s_log.DebugFormat (
            "Action {0}: Performing wait '{1}' on context '{2}'.",
            _id,
            _waitingStrategies[i].GetType().Name,
            newContext.FrameRootElement.FindCss ("title").InnerHTML);

        var stateForWaitingStrategy = states[i];
        _waitingStrategies[i].PerformWaitAfterActionPerformed (newContext, stateForWaitingStrategy);
      }

      s_log.DebugFormat ("Action {0} finished.", _id);
    }

    private TestObjectContext GetNewTestObjectContext (TestObjectContext context)
    {
      if (_closesWindow)
        return context.CloneForParentWindow();

      return context;
    }

    private void ExecuteAfterClickActions (TestObjectContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      foreach (var afterClickAction in _afterClickActions)
        afterClickAction (context);
    }
  }

  //Click(); // SimplePostBack
  //Click(WaitFor.WxeResetIn(home.Frame)); // LoadFrameFunctionInFrame, LoadWindowFunctionInFrame, LoadMainAutoRefreshingFrameFunctionInFrame
  //// WXE RESET must check for new function token!!

  //Click(WaitFor.WxePostBackIn(home.Frame)); // Load FrameFunction as sub function in Frame
  //Click().ExpectNewWindow("MultiWindowTest"); // Load WindowFunction in new Window
  //Click(WaitFor.WxePostBackIn(home)); // RefreshMainUpdatePanel

  //Click(WaitFor.WxePostBackInParent()); // wenn opened from main, or when opened from frame (see following line)
  //Click(WaitFor.WxePostBackIn(home.Frame)); // Window: Close & refresh only frame

  //Click(WaitFor.WxePostBackIn(home)); // Window: Close & refresh main
}