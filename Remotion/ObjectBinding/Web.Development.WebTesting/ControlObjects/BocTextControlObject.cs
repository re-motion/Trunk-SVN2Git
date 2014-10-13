using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="BocTextValue"/>.
  /// </summary>
  public class BocTextControlObject : BocControlObject, IFillableControlObject
  {
    /// <summary>
    /// Initializes the control object.
    /// </summary>
    /// <param name="id">The control object's ID.</param>
    /// <param name="context">The control object's context.</param>
    public BocTextControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
    }

    public string GetText ()
    {
      var valueScope = FindChild ("Value");

      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return valueScope.Text;

      return valueScope.Value;
    }

    public UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, actionBehavior);
    }

    public UnspecifiedPageObject FillWith ([NotNull] string text, [NotNull] ThenAction then, [CanBeNull] IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualActionBehavior = GetActualActionBehavior (actionBehavior);
      FindChild ("Value").FillWithAndWait (text, then, Context, actualActionBehavior);
      return UnspecifiedPage();
    }
  }
}