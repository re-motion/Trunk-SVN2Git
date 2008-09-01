using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles <see cref="String"/>s in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderToStringHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      toTextBuilder.AppendString (obj.ToString ());

      toTextProviderHandlerFeedback.Handled = true;
    }
  }
}