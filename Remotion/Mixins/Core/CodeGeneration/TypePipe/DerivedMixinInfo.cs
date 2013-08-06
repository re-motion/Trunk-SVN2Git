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
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class DerivedMixinInfo : IMixinInfo
  {
    private readonly ConcreteMixinType _concreteMixinType;

    public DerivedMixinInfo (ConcreteMixinType concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      _concreteMixinType = concreteMixinType;
    }

    public Type MixinType
    {
      get { return _concreteMixinType.GeneratedType; }
    }

    public IEnumerable<Type> GetInterfacesToImplement ()
    {
      yield return _concreteMixinType.GeneratedOverrideInterface;
    }

    public MethodInfo GetPubliclyCallableMixinMethod (MethodInfo methodToBeCalled)
    {
      ArgumentUtility.CheckNotNull ("methodToBeCalled", methodToBeCalled);

      if (methodToBeCalled.IsPublic)
        return methodToBeCalled;

      return _concreteMixinType.GetMethodWrapper (methodToBeCalled);
    }

    public MethodInfo GetOverrideInterfaceMethod (MethodInfo mixinMethod)
    {
      ArgumentUtility.CheckNotNull ("mixinMethod", mixinMethod);

      return _concreteMixinType.GetOverrideInterfaceMethod (mixinMethod);
    }
  }
}