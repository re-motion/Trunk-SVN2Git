using System;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles instances implementing the <see cref="IFormattable"/> interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// This handler takes care of all primitive data types and prevents them from being treated by the <see cref="ToTextProviderAutomaticObjectToTextHandler"/> handler,
  /// which should always come after it.
  /// </summary>
  public class ToTextProviderFormattableHandler : ToTextProviderHandler
  {
    private static readonly CultureInfo s_cultureInfoInvariant = CultureInfo.InvariantCulture;

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);


      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilderBase toTextBuilder = toTextParameters.ToTextBuilder;

      if(obj is IFormattable)
      {
        IFormattable formattable = (IFormattable) obj;
        //toTextBuilder.AppendString (StringUtility.Format (obj, s_cultureInfoInvariant));
        toTextBuilder.AppendString (formattable.ToString (null, s_cultureInfoInvariant));
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}