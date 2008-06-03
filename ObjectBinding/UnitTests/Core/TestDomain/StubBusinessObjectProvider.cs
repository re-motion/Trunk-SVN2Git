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
using Remotion.Collections;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    private readonly InterlockedDataStore<Type, IBusinessObjectService> _serviceStore = new InterlockedDataStore<Type, IBusinessObjectService>();


    public StubBusinessObjectProvider ()
        : this (MockRepository.GenerateStub<IBusinessObjectServiceFactory>())
    {
    }

    public StubBusinessObjectProvider (IBusinessObjectServiceFactory serviceFactory)
        : base (serviceFactory)
    {
    }

    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }
  }
}
