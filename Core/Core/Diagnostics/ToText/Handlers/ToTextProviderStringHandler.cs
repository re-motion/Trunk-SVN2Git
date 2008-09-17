/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;

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
        toTextBuilder.WriteRawElementBegin();
        string s= (string) obj;
        if (settings.UseAutomaticStringEnclosing)
        {
          toTextBuilder.WriteRawChar ('"');
          toTextBuilder.WriteRawString (s);
          toTextBuilder.WriteRawChar ('"');
        }
        else
        {
          toTextBuilder.WriteRawString(s);
        }
        //toTextBuilder.HandlerAfterAppendElement ();
        toTextBuilder.WriteRawElementEnd ();
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}