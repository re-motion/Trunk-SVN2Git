using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles <see cref="String"/>s in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// If <see cref="ToTextProviderSettings.UseAutomaticStringEnclosing"/> is true it wraps strings in double quotes.
  /// </summary>
  public class ToTextProviderStringHandler : ToTextProviderHandler
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

      if (type == typeof (string))
      {
        string s= (string) obj;
        if (settings.UseAutomaticStringEnclosing)
        {
          toTextBuilder.AppendChar ('"');
          toTextBuilder.AppendString (s);
          toTextBuilder.AppendChar ('"');
        }
        else
        {
          toTextBuilder.AppendString(s);
        }
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}