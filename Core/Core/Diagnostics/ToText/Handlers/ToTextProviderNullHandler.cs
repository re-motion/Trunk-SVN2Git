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