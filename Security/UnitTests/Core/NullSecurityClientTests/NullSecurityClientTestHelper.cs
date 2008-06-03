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
using Rhino.Mocks;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  public class NullSecurityClientTestHelper
  {
    public static NullSecurityClientTestHelper CreateForStatelessSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    public static NullSecurityClientTestHelper CreateForStatefulSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private SecurableObject _securableObject;

    private NullSecurityClientTestHelper()
    {
      _mocks = new MockRepository();
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy>();

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public NullSecurityClient CreateSecurityClient()
    {
      return new NullSecurityClient();
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ReplayAll()
    {
      _mocks.ReplayAll();
    }

    public void VerifyAll()
    {
      _mocks.VerifyAll();
    }
  }
}
