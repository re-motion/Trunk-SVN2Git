using System;

namespace Remotion.Diagnostics.ToText.Internal.ToTextProviderHandler
{
  public interface IToTextProviderHandler
  {
    void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);
    bool Disabled { get; set; }
  }
}