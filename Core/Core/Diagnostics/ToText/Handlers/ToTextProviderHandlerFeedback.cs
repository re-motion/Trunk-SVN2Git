using System;

namespace Remotion.Diagnostics.ToText.Handlers
{
  public class ToTextProviderHandlerFeedback
  {
    public ToTextProviderHandlerFeedback ()  
    {
      Handled = false;
    }
    public bool Handled { get; set; }
  }
}