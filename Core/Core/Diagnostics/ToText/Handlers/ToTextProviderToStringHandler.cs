using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Special type of handler which handles all instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade 
  /// by calling their <see cref="Object.ToString"/> method.
  /// Since it handles all incoming types, if it is used this should always the last handler in the fallback cascade.
  /// 
  /// </summary>
  public class ToTextProviderToStringHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;

      toTextBuilder.AppendString (obj.ToString ());

      toTextProviderHandlerFeedback.Handled = true;
    }
  }
}