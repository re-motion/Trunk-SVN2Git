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
    public string ID
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
    /// Returns the waiting strategy to be used when acting on the control object's scope.
    /// </summary>
    /// <param name="userDefinedWaitingStrategy">User-provided waiting strategy.</param>
    /// <returns>Waiting strategy to be used.</returns>
    protected IWaitingStrategy GetActualWaitingStrategy ([CanBeNull] IWaitingStrategy userDefinedWaitingStrategy)
    {
      return GetActualWaitingStrategy (Scope, userDefinedWaitingStrategy);
    }

    /// <summary>
    /// Returns the waiting strategy to be used.
    /// </summary>
    /// <param name="scope">Scope which is to be acted on.</param>
    /// <param name="userDefinedWaitingStrategy">User-provided waiting strategy.</param>
    /// <returns>Waiting strategy to be used.</returns>
    protected IWaitingStrategy GetActualWaitingStrategy ([NotNull] ElementScope scope, [CanBeNull] IWaitingStrategy userDefinedWaitingStrategy)
    {
      if (userDefinedWaitingStrategy != null)
        return userDefinedWaitingStrategy;

      // Todo RM-6297: Improve exception handling if attributes are not in the correct format.

      if (scope[DiagnosticMetadataAttributes.TriggersPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersPostBack]);
        if (hasAutoPostBack)
          return WaitFor.WxePostBack;
      }

      if (scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return WaitFor.WxeReset;
      }

      return WaitFor.Nothing;
    }
  }
}