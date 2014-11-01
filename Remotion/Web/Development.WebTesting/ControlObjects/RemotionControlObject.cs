using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based control.
  /// </summary>
  public abstract class RemotionControlObject : WebFormsControlObject
  {
    /// <summary>
    /// Initializes the control object with the given <paramref name="context"/>.
    /// </summary>
    protected RemotionControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns a child element of the control, specified by an <paramref name="idSuffix"/> parameter.
    /// </summary>
    protected ElementScope FindChild ([NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      return FindChild (GetHtmlID(), idSuffix);
    }

    protected ElementScope FindChild ([NotNull] string id, [NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      var fullId = string.Format ("{0}_{1}", id, idSuffix);
      return Scope.FindId (fullId);
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      if (scope[DiagnosticMetadataAttributes.TriggersPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersPostBack]);
        if (hasAutoPostBack)
          return Continue.When (Wxe.PostBackCompleted);
      }

      if (scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return Continue.When (Wxe.Reset);
      }

      return Continue.Immediately();
    }
  }
}