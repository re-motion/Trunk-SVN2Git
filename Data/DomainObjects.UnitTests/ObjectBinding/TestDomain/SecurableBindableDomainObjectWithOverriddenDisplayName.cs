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
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [Instantiable]
  [Serializable]
  public abstract class SecurableBindableDomainObjectWithOverriddenDisplayName : BindableDomainObjectWithOverriddenDisplayName, ISecurableObject
  {
    public static SecurableBindableDomainObjectWithOverriddenDisplayName NewObject (IObjectSecurityStrategy objectSecurityStrategy)
    {
      return NewObject<SecurableBindableDomainObjectWithOverriddenDisplayName> ().With (objectSecurityStrategy);
    }

    public static new SecurableBindableDomainObjectWithOverriddenDisplayName GetObject (ObjectID id)
    {
      return GetObject<SecurableBindableDomainObjectWithOverriddenDisplayName> (id);
    }

    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    protected SecurableBindableDomainObjectWithOverriddenDisplayName (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return typeof (SecurableBindableDomainObjectWithOverriddenDisplayName);
    }
  }
}
