// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  public class ObjectFactoryImplementation : IObjectFactoryImplementation
  {
    public object CreateInstance (
        bool allowNonPublicConstructors, 
        Type targetOrConcreteType, 
        ParamList constructorParameters, 
        params object[] preparedMixins)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("preparedMixins", preparedMixins);

      if (targetOrConcreteType.IsInterface)
      {
        var message = string.Format ("Cannot instantiate type '{0}', it's an interface.", targetOrConcreteType);
        throw new ArgumentException (message, "targetOrConcreteType");
      }

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);

      IConstructorLookupInfo constructorLookupInfo;
      if (classContext == null)
      {
        if (preparedMixins.Length > 0)
          throw new ArgumentException (string.Format ("There is no mixin configuration for type {0}, so no mixin instances must be specified.",
              targetOrConcreteType.FullName), "preparedMixins");

        constructorLookupInfo = new MixedTypeConstructorLookupInfo (targetOrConcreteType, targetOrConcreteType, allowNonPublicConstructors);
      }
      else
        constructorLookupInfo = ConcreteTypeBuilder.Current.GetConstructorLookupInfo (classContext, allowNonPublicConstructors);

      using (new MixedObjectInstantiationScope (preparedMixins))
      {
        return constructorParameters.InvokeConstructor (constructorLookupInfo);
      }
    }
  }
}
