using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Handlers
{
  /// <summary>
  /// Parameter class used to pass the instance to convert, type of the instance and <see cref="ToTextBuilder"/> to <see cref="ToTextProviderHandler.ToTextIfTypeMatches"/>.
  /// </summary>
  public class ToTextParameters
  {
    public object Object { get; set; }
    public Type Type { get; set; }
    public IToTextBuilderBase ToTextBuilder { get; set; }

    public ToTextProvider ToTextProvider
    {
      get { return ToTextBuilder.ToTextProvider; }
    }

    public ToTextProviderSettings Settings
    {
      get { return ToTextProvider.Settings; }
    }


    public static void CheckNotNull (ToTextParameters toTextParameters)
    {
      ArgumentUtility.CheckNotNull ("toTextParameters", toTextParameters);
      ArgumentUtility.CheckNotNull ("toTextParameters.Object", toTextParameters.Object);
      ArgumentUtility.CheckNotNull ("toTextParameters.Type", toTextParameters.Type);
      ArgumentUtility.CheckNotNull ("toTextParameters.ToTextBuilder", toTextParameters.ToTextBuilder);
    }
  }

}