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
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;

      // Catch Type|s to avoid endless recursion. 
      if (obj is Type) 
      {
        toTextBuilder.AppendRawElementBegin ();
        toTextBuilder.AppendRawString (obj.ToString ());
        toTextBuilder.AppendRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}