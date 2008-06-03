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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class SecurableClassWithReferenceType<T> : ClassWithReferenceType<T>, ISecurableObject
      where T: class
  {
    private readonly IObjectSecurityStrategy _objectSecurityStrategy;

    public SecurableClassWithReferenceType (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _objectSecurityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return typeof (SecurableClassWithReferenceType<T>);
    }
  }
}
