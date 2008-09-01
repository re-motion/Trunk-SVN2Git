using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles <see cref="Char"/>s in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// If <see cref="ToTextProviderSettings.UseAutomaticCharEnclosing"/> is true it wraps characters in single quotes.
  /// </summary>
  public class ToTextProviderCharHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (char))
      {
        char c = (char) obj;
        if (settings.UseAutomaticCharEnclosing)
        {
          toTextBuilder.AppendChar ('\'');
          toTextBuilder.AppendChar (c);
          toTextBuilder.AppendChar ('\'');
        }
        else
        {
          toTextBuilder.AppendChar (c);
        }

        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}