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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class FakeDomainObjectFactory : IDomainObjectFactory
  {
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      throw new NotImplementedException ();
    }

    public Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType)
    {
      throw new NotImplementedException();
    }

    public bool WasCreatedByFactory (Type t)
    {
      throw new NotImplementedException ();
    }

    public IFuncInvoker<TMinimal> GetTypesafeConstructorInvoker<TMinimal> (Type dynamicType) where TMinimal : DomainObject
    {
      throw new NotImplementedException ();
    }

    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      throw new NotImplementedException ();
    }
  }
}
