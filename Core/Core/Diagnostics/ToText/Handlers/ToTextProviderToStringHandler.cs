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
  /// Special type of handler which handles all instances in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade 
  /// by calling their <see cref="Object.ToString"/> method.
  /// Since it handles all incoming types, if it is used this should always the last handler in the fallback cascade.
  /// 
  /// </summary>
  public class ToTextProviderToStringHandler : ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      toTextBuilder.WriteRawElementBegin();
      toTextBuilder.WriteRawString (obj.ToString ());
      toTextBuilder.WriteRawElementEnd ();

      toTextProviderHandlerFeedback.Handled = true;
    }
  }
}