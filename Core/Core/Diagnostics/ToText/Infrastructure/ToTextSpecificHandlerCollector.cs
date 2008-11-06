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
using System.Collections;
using System.ComponentModel.Design;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection;
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
      // TODO: Use ContextAwareTypeDiscoveryUtility.GetInstance() instead to make use of assembly caching
      ITypeDiscoveryService _typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
          new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, excludeGlobalTypes));

      ICollection types = _typeDiscoveryService.GetTypes (typeof (T), excludeGlobalTypes);

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