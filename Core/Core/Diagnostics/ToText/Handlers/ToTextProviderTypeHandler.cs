using System;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Handlers;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers {
  /// <summary>
  /// Handles <see cref="Type"/> instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderTypeHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is Type) 
      {
        // Catch Type|s to avoid endless recursion. 
        toTextBuilder.AppendString (obj.ToString ());
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}