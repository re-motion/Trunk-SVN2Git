// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections;
using System.ComponentModel.Design;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  /// <summary>
  /// Finds <see cref="ToTextProvider"/> type handlers that supply <see cref="ToTextProvider.ToText"/> functionality for
  /// a specific type through reflection and registers them with a <see cref="ToTextProvider"/>.
  /// Type handlers are classes which derive directly from <see cref="ToTextSpecificTypeHandler{T}"/> and carry the 
  /// <see cref="ToTextSpecificHandlerAttribute"/> attribute.
  /// </summary>
  public class ToTextSpecificHandlerCollector
  {
    // TODO: Move down (public methods are more important)
    private ToTextSpecificHandlerAttribute RetrieveTextHandlerAttribute (Type type)
    {
      return AttributeUtility.GetCustomAttribute<ToTextSpecificHandlerAttribute> (type, false);
    }

    public ToTextSpecificHandlerMap<IToTextSpecificTypeHandler> CollectTypeHandlers()
    {
      return CollectHandlers <IToTextSpecificTypeHandler>("ToTextSpecificTypeHandler`1");
    }

    public ToTextSpecificHandlerMap<IToTextSpecificInterfaceHandler> CollectInterfaceHandlers ()
    {
      return CollectHandlers<IToTextSpecificInterfaceHandler> ("ToTextSpecificInterfaceHandler`1");
    }

    public ToTextSpecificHandlerMap<T> CollectHandlers<T> (string baseTypeName) where T : IToTextSpecificHandler
    {
      var handlerMap = new ToTextSpecificHandlerMap<T> ();
      const bool excludeGlobalTypes = true;

      ICollection types = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService().GetTypes (typeof (T), excludeGlobalTypes);

      foreach (Type type in types)
      {
        if (RetrieveTextHandlerAttribute (type) != null)
        {
          Type baseType = type.BaseType;
          //Assertion.IsTrue (baseType.Name == "ToTextSpecificTypeHandler`1");
          Assertion.IsTrue (baseType.Name == baseTypeName); // Note: This check disallows use of derived type handlers
          // TODO: Refactor check to use Type objects and IsAssignableFrom instead.
          // TODO: Throw exception instead if attribute is attached to wrong type
          // Idea: If IToTextSpecificHandler had a membler "HandledType", the check (and the following two lines) could be removed.
          Type[] genericArguments = baseType.GetGenericArguments ();
          Type handledType = genericArguments[0];

          handlerMap[handledType] = (T) Activator.CreateInstance (type);
        }
      }
      return handlerMap;
    }
  }
}
