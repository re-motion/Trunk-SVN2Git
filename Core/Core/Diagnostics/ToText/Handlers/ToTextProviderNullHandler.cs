using System;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Handles null-references in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderNullHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters", toTextParameters);
      ArgumentUtility.CheckNotNull ("toTextProviderHandlerFeedback", toTextProviderHandlerFeedback);

      if (toTextParameters.Object == null)
      {
        Log ("null");

        var toTextBuilder = toTextParameters.ToTextBuilder;
        toTextBuilder.WriteRawElementBegin ();
        toTextBuilder.WriteRawString ("null");
        toTextBuilder.WriteRawElementEnd ();

        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}