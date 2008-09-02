using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// <para>Handles instances which implement the <see cref="IToText"/> interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// Types implementing <see cref="IToText"/> supply a <see cref="IToText.ToText"/> method, which is called to do the transformation into human readable text form.
  /// <see cref="IToText.ToText"/> can be seen as the ToText-sibling of <see cref="object.ToString"/> (Note that it works more efficiently since it 
  /// works by appending its result to a <see cref="ToTextBuilder"/> instead of returning a <see cref="String"/>).</para>
  /// </summary>
  public class ToTextProviderIToTextHandlerHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      ToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is IToText)
      {
        ((IToText) obj).ToText (toTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}