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
using System.Globalization;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles instances implementing the <see cref="IFormattable"/> interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// This handler takes care of all primitive data types and prevents them from being treated by the <see cref="ToTextProviderAutomaticObjectToTextHandler"/> handler,
  /// which should always come after it.
  /// </summary>
  public class ToTextProviderFormattableHandler : ToTextProviderHandler
  {
    private static readonly CultureInfo s_cultureInfoInvariant = CultureInfo.InvariantCulture;

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);


      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;

      if(obj is IFormattable)
      {
        toTextBuilder.WriteRawElementBegin ();
        IFormattable formattable = (IFormattable) obj;
        //toTextBuilder.AppendString (StringUtility.Format (obj, s_cultureInfoInvariant));
        toTextBuilder.WriteRawString (formattable.ToString (null, s_cultureInfoInvariant));
        toTextProviderHandlerFeedback.Handled = true;
        toTextBuilder.WriteRawElementEnd ();
      }
    }
  }
}