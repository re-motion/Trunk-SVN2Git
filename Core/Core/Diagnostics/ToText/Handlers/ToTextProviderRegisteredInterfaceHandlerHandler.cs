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
  /// Handles instances implementing a registered interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// The interface handler with the highest priority will be used used.
  /// Interface handlers are registered through calling <see cref="ToTextProvider.RegisterSpecificInterfaceHandlerWithHighestPriority{T}"/> and 
  /// <see cref="ToTextProvider.RegisterSpecificInterfaceHandlerWithLowestPriority{T}"/> respectively.
  /// </summary>
  public class ToTextProviderRegisteredInterfaceHandlerHandler : ToTextProviderHandler
  {
    private readonly ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> _interfaceTypeHandlerMap;
    private int _interfaceHandlerPriorityMin = 0;
    private int _interfaceHandlerPriorityMax = 0;

    public ToTextProviderRegisteredInterfaceHandlerHandler (ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> interfaceTypeHandlerMap)
    {
      _interfaceTypeHandlerMap = interfaceTypeHandlerMap;
    }



    public void RegisterInterfaceHandlerAppendLast<T> (Action<T, IToTextBuilder> handler)
    {
      --_interfaceHandlerPriorityMin;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandlerWrapper<T> (handler, _interfaceHandlerPriorityMin));
    }

    public void RegisterInterfaceHandlerAppendFirst<T> (Action<T, IToTextBuilder> handler)
    {
      ++_interfaceHandlerPriorityMax;
      _interfaceTypeHandlerMap.Add (typeof (T), new ToTextSpecificInterfaceHandlerWrapper<T> (handler, _interfaceHandlerPriorityMax));
    }
    
    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      toTextProviderHandlerFeedback.Handled = false;

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      if (!settings.UseInterfaceHandlers)
      {
        return;
      }

      IToTextSpecificInterfaceHandler interfaceHandlerWithMaximumPriority = null;
      foreach (var interfaceType in type.GetInterfaces ())
      {
        IToTextSpecificInterfaceHandler interfaceHandler;
        _interfaceTypeHandlerMap.TryGetValue (interfaceType, out interfaceHandler);
        if (interfaceHandler != null &&
            (interfaceHandlerWithMaximumPriority == null || (interfaceHandler.Priority > interfaceHandlerWithMaximumPriority.Priority)))
        {
          interfaceHandlerWithMaximumPriority = interfaceHandler;
        }
      }

      if (interfaceHandlerWithMaximumPriority != null)
      {
        interfaceHandlerWithMaximumPriority.ToText (obj, toTextBuilder);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }
  }
}