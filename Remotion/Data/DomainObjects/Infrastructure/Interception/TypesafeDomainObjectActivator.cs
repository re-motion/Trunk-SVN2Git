// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  public static class TypesafeDomainObjectActivator
  {
    public static IFuncInvoker<TMinimal> CreateInstance<TMinimal> (Type baseType, Type type, BindingFlags bindingFlags)
        where TMinimal : DomainObject
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (TMinimal));
      FuncInvoker<TMinimal> constructorInvoker = new FuncInvoker<TMinimal> (new DomainObjectConstructorLookupInfo (baseType, type, bindingFlags).GetDelegate);
      return new FuncInvokerWrapper<TMinimal> (constructorInvoker, delegate (TMinimal instance)
      {
        DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);
        return instance;
      }); 
    }
  }
}
