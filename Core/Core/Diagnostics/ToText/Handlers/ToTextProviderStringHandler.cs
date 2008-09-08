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
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (type == typeof (string))
      {
        //toTextBuilder.HandlerBeforeAppendElement ();
        toTextBuilder.AppendRawElementBegin();
        string s= (string) obj;
        if (settings.UseAutomaticStringEnclosing)
        {
          toTextBuilder.AppendRawChar ('"');
          toTextBuilder.AppendRawString (s);
          toTextBuilder.AppendRawChar ('"');
        }
        else
        {
          toTextBuilder.AppendRawString(s);
        }
        //toTextBuilder.HandlerAfterAppendElement ();
        toTextBuilder.AppendRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}