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
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class ObjectFactoryImplementation : IObjectFactoryImplementation
  {
    public Type ResolveType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      Type targetType;
      if (baseType.IsInterface)
      {
        ClassContext registeredContext = MixinConfiguration.ActiveConfiguration.ResolveCompleteInterface (baseType);
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

    public object CreateInstance (bool allowNonPublicConstructors, Type baseTypeOrInterfaceOrConcreteType, ParamList constructorParameters, GenerationPolicy generationPolicy, params object[] preparedMixins)
    {
      ArgumentUtility.CheckNotNull ("baseTypeOrInterfaceOrConcreteType", baseTypeOrInterfaceOrConcreteType);
      ArgumentUtility.CheckNotNull ("preparedMixins", preparedMixins);

      Type resolvedTargetType = ResolveType (baseTypeOrInterfaceOrConcreteType);
      Type concreteType = TypeFactory.GetConcreteType (resolvedTargetType, generationPolicy);

      if (!typeof (IMixinTarget).IsAssignableFrom (concreteType) && preparedMixins.Length > 0)
        throw new ArgumentException (string.Format ("There is no mixin configuration for type {0}, so no mixin instances must be specified.",
            baseTypeOrInterfaceOrConcreteType.FullName), "preparedMixins");

      var constructorLookupInfo = new MixedTypeConstructorLookupInfo (concreteType, resolvedTargetType, allowNonPublicConstructors);

      using (new MixedObjectInstantiationScope (preparedMixins))
      {
        return constructorParameters.InvokeConstructor (constructorLookupInfo);
      }
    }
  }
}
