using System;
using Remotion.Diagnostics.ToText.Handlers;

namespace Remotion.Diagnostics.ToText
{
  public interface IToTextProviderHandler
  {
    void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);
    bool Disabled { get; set; }
  }
}