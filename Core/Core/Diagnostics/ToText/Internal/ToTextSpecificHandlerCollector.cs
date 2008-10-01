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
using System.Collections;
using System.ComponentModel.Design;
using Remotion.Diagnostics.ToText.Internal;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText.Internal
{
  /// <summary>
  /// Finds <see cref="ToTextProvider"/> type handlers that supply <see cref="ToTextProvider.ToText"/> functionality for
  /// a specific type through reflection and registers them with a <see cref="ToTextProvider"/>.
  /// Type handlers are classes which derive directly from <see cref="ToTextSpecificTypeHandler{T}"/> and carry the 
  /// <see cref="ToTextSpecificHandlerAttribute"/> attribute.
  /// </summary>
  public class ToTextSpecificHandlerCollector
  {
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
      ITypeDiscoveryService _typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
          new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, excludeGlobalTypes));

      ICollection types = _typeDiscoveryService.GetTypes (typeof (T), excludeGlobalTypes);

      foreach (Type type in types)
      {
        if (RetrieveTextHandlerAttribute (type) != null)
        {
          Type baseType = type.BaseType;
          //Assertion.IsTrue (baseType.Name == "ToTextSpecificTypeHandler`1");
          Assertion.IsTrue (baseType.Name == baseTypeName);
          Type[] genericArguments = baseType.GetGenericArguments ();
          Type handledType = genericArguments[0];

          //toTextProvider.RegisterSpecificTypeHandler (handledType, (IToTextSpecificTypeHandler) Activator.CreateInstance (type));
          handlerMap[handledType] = (T) Activator.CreateInstance (type);
        }
      }
      return handlerMap;
    }
  }
}