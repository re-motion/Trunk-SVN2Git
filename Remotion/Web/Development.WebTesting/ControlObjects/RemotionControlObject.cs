using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based control.
  /// </summary>
  public abstract class RemotionControlObject : ControlObject
  {
    /// <summary>
    /// Initializes the control object with the given <paramref name="context"/> and <paramref name="id"/>.
    /// </summary>
    protected RemotionControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns a child element of the control, specified by an <paramref name="idSuffix"/> parameter.
    /// </summary>
    protected ElementScope FindChild ([NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      var fullId = string.Format ("{0}_{1}", ID, idSuffix);
      return Scope.FindId (fullId);
    }

    /// <summary>
    /// Returns the actual <see cref="IActionBehavior"/> to be used when acting on the control object's scope.
    /// </summary>
    /// <param name="userDefinedActionBehavior">User-provided <see cref="IActionBehavior"/>.</param>
    /// <returns><see cref="IActionBehavior"/> to be used.</returns>
    protected IActionBehavior GetActualActionBehavior ([CanBeNull] IActionBehavior userDefinedActionBehavior)
    {
      return GetActualActionBehavior (Scope, userDefinedActionBehavior);
    }

    /// <summary>
    /// Returns the actual <see cref="IActionBehavior"/> to be used.
    /// </summary>
    /// <param name="scope">Scope which is to be acted on.</param>
    /// <param name="userDefinedActionBehavior">User-provided <see cref="IActionBehavior"/>.</param>
    /// <returns><see cref="IActionBehavior"/> to be used.</returns>
    protected IActionBehavior GetActualActionBehavior ([NotNull] ElementScope scope, [CanBeNull] IActionBehavior userDefinedActionBehavior)
    {
      if (userDefinedActionBehavior != null)
        return userDefinedActionBehavior;

      if (scope[DiagnosticMetadataAttributes.TriggersPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersPostBack]);
        if (hasAutoPostBack)
          return Behavior.WaitFor (WaitFor.WxePostBack);
      }

      if (scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return Behavior.WaitFor (WaitFor.WxeReset);
      }

      return Behavior.WaitFor (WaitFor.Nothing);
    }
  }
}