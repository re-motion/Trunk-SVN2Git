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
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (char))
      {
        toTextBuilder.AppendRawElementBegin ();
        char c = (char) obj;
        if (settings.UseAutomaticCharEnclosing)
        {
          toTextBuilder.AppendRawChar ('\'');
          toTextBuilder.AppendRawChar (c);
          toTextBuilder.AppendRawChar ('\'');
        }
        else
        {
          toTextBuilder.AppendRawChar (c);
        }
        toTextBuilder.AppendRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}