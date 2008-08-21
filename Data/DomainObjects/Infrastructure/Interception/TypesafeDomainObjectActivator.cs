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
