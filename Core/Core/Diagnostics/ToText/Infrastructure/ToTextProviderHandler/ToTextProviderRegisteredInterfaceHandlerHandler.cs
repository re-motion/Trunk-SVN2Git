// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles instances implementing a registered interface in <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// The interface handler with the highest priority will be used used.
  /// Interface handlers are registered through calling <see cref="ToTextProvider.RegisterSpecificInterfaceHandlerWithHighestPriority{T}"/> and 
  /// <see cref="ToTextProvider.RegisterSpecificInterfaceHandlerWithLowestPriority{T}"/> respectively.
  /// </summary>
  public class ToTextProviderRegisteredInterfaceHandlerHandler : Infrastructure.ToTextProviderHandler.ToTextProviderHandler
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
      Infrastructure.ToTextProviderHandler.ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

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