using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based control.
  /// </summary>
  public abstract class RemotionControlObject : ControlObject
  {
    private readonly string _id;

    /// <summary>
    /// Initializes the control object with the given <paramref name="context"/> and <paramref name="id"/>.
    /// </summary>
    protected RemotionControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (context)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      _id = id;
    }

    /// <summary>
    /// The control's ID.
    /// </summary>
    protected string ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Returns a child element of the control, specified by an <paramref name="idSuffix"/> parameter.
    /// </summary>
    protected ElementScope FindChild (string idSuffix)
    {
      var fullId = string.Format ("{0}_{1}", ID, idSuffix);
      return Scope.FindId (fullId);
    }

    /// <summary>
    /// Returns the waiting strategy to be used.
    /// </summary>
    /// <param name="waitingStrategy">User-provided waiting strategy.</param>
    /// <returns>Waiting strategy to be used.</returns>
    protected IWaitingStrategy GetActualWaitingStrategy ([CanBeNull] IWaitingStrategy waitingStrategy)
    {
      if (waitingStrategy != null)
        return waitingStrategy;

      // Todo RM-6297: Improve exception handling if attributes are not in the correct format.

      if (Scope[DiagnosticMetadataAttributes.HasAutoPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (Scope[DiagnosticMetadataAttributes.HasAutoPostBack]);
        if (hasAutoPostBack)
          return WaitFor.WxePostBack;
      }

      if (Scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (Scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return WaitFor.WxeReset;
      }

      return WaitFor.Nothing;
    }
  }
}