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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class SqlProviderBaseTest : TableInheritanceMappingTest
  {
    private StorageProviderManager _storageProviderManager;
    private SqlProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
      _provider = (SqlProvider) _storageProviderManager.GetMandatory (TableInheritanceTestDomainProviderID);
      _provider.Connect ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose ();
    }

    protected StorageProviderManager StorageProviderManager
    {
      get { return _storageProviderManager; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}
