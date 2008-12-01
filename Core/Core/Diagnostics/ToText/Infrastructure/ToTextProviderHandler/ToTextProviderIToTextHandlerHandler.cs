// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// <para>Handles instances which implement the <see cref="IToTextConvertible"/> interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// Types implementing <see cref="IToTextConvertible"/> supply a <see cref="IToTextConvertible.ToText"/> method, which is called to do the transformation into human readable text form.
  /// <see cref="IToTextConvertible.ToText"/> can be seen as the ToText-sibling of <see cref="object.ToString"/> (Note that it works more efficiently since it 
  /// works by appending its result to a <see cref="ToTextBuilder"/> instead of returning a <see cref="String"/>).</para>
  /// </summary>
  public class ToTextProviderIToTextHandlerHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
  {
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if (obj is IToTextConvertible)
      {
        ((IToTextConvertible) obj).ToText (toTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }

    }
  }
}