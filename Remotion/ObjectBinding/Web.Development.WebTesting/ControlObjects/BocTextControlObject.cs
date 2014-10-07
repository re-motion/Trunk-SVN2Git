using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="BocTextValue"/>.
  /// </summary>
  public class BocTextControlObject : BocControlObject
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

    public UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] IWaitingStrategy waitingStrategy = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return FillWith (text, Then.TabAway, waitingStrategy);
    }

    public UnspecifiedPageObject FillWith ([NotNull] string text, [NotNull] ThenAction then, [CanBeNull] IWaitingStrategy waitingStrategy = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("then", then);

      var actualWaitStrategy = GetActualWaitingStrategy (waitingStrategy);
      FindChild ("Value").FillWithAndWait (text, then, Context, actualWaitStrategy);
      return UnspecifiedPage();
    }
  }
}