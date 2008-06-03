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

namespace Remotion.Security.UnitTests.Core.SampleDomain
{
  public class SecurableObjectWithSecuredInstanceMethods : ISecurableObject
  {
    public SecurableObjectWithSecuredInstanceMethods ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.First)]
    public void InstanceMethod ()
    {
    }

    [DemandMethodPermission (TestAccessTypes.Second)]
    public void InstanceMethod (string value)
    {
    }

    [DemandMethodPermission (TestAccessTypes.Third)]
    public void OtherInstanceMethod (string value)
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Type GetSecurableType ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
