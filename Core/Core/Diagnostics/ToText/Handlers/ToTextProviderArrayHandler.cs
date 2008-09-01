using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles <see cref="Array"/> instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderArrayHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (type.IsArray)
      {
        toTextBuilder.AppendArray ((Array) obj);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}