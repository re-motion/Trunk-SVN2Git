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
using Remotion.Collections;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class MixedObjectInstantiator : IMixedObjectInstantiator
  {
    public Type ResolveType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      Type targetType;
      if (baseType.IsInterface)
      {
        ClassContext registeredContext = MixinConfiguration.ActiveConfiguration.ResolveInterface (baseType);
        if (registeredContext == null)
        {
          string message = string.Format ("The interface '{0}' has not been registered in the current configuration, no instances of the "
              + "type can be created.", baseType.FullName);
          throw new InvalidOperationException (message);
        }
        targetType = registeredContext.Type;
      }
      else
        targetType = baseType;
      return targetType;
    }

    public FuncInvokerWrapper<T> CreateConstructorInvoker<T> (Type baseTypeOrInterface, GenerationPolicy generationPolicy, bool allowNonPublic,
        params object[] preparedMixins)
    {
      ArgumentUtility.CheckNotNull ("baseTypeOrInterface", baseTypeOrInterface);
      ArgumentUtility.CheckNotNull ("preparedMixins", preparedMixins);

      Type resolvedTargetType = ResolveType (baseTypeOrInterface);
      Type concreteType = TypeFactory.GetConcreteType (resolvedTargetType, generationPolicy);

      if (!typeof (IMixinTarget).IsAssignableFrom (concreteType) && preparedMixins.Length > 0)
        throw new ArgumentException (string.Format ("There is no mixin configuration for type {0}, so no mixin instances must be specified.",
            baseTypeOrInterface.FullName), "preparedMixins");

      MixedTypeConstructorLookupInfo constructorLookupInfo = new MixedTypeConstructorLookupInfo (concreteType, resolvedTargetType, allowNonPublic);
      FuncInvoker<object[], T> constructorInvoker = new FuncInvoker<object[], T> (constructorLookupInfo.GetDelegate, preparedMixins);
      return new FuncInvokerWrapper<T> (constructorInvoker);
    }
  }
}
