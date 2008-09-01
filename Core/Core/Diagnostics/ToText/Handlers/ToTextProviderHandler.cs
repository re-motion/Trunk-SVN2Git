using System;


namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Base class for all handlers which can be used by <see cref="ToTextProvider"/> in its ToText-fallback-cascade.
  /// </summary>
  public abstract class ToTextProviderHandler : IToTextProviderHandler
  {
    protected ToTextProviderHandler ()
    {
      Disabled = false;
    }

    protected void Log (string s)
    {
      Console.WriteLine ("[ToTextProviderHandler]: " + s);
    }

    /// <summary>
    /// Abstract method whose concrete implementations supply conversion into human readable text form (<see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> method) 
    /// of the passed instance for specific classes of types (e.g. types implementing IFormattable).
    /// </summary>
    /// <param name="toTextParameters">The instance to convert, type of the instance and <see cref="ToTextBuilder"/> to add the human readable text to.</param>
    /// <param name="toTextProviderHandlerFeedback">If the type was handled by the method, it must set 
    /// the <see cref="ToTextProviderHandlerFeedback.Handled"/> property of the passed argument to <c>true</c>.</param>
    public abstract void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback);

    /// <summary>
    /// Enables/disables the handler. Disabled handlers are skipped by the <see cref="ToTextProvider"/> fallback cascade).
    /// </summary>
    public bool Disabled { get; set; }
  }
}