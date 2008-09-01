using System;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Parameter class used to pass the instance to convert, type of the instance and <see cref="ToTextBuilder"/> to <see cref="ToTextProviderHandler.ToTextIfTypeMatches"/>.
  /// </summary>
  public class ToTextParameters
  {
    public object Object { get; set; }
    public Type Type { get; set; }
    public ToTextBuilder ToTextBuilder { get; set; }
    public ToTextProvider ToTextProvider
    {
      get { return ToTextBuilder.ToTextProvider; }
    }
  }
}