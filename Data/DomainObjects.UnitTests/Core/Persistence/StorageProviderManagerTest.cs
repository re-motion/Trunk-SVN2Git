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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
{
  [TestFixture]
  public class StorageProviderManagerTest : StandardMappingTest
  {
    private StorageProviderManager _storageProviderManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose ();
    }

    [Test]
    public void LookUp ()
    {
      StorageProvider provider = _storageProviderManager[c_testDomainProviderID];

      Assert.IsNotNull (provider);
      Assert.AreEqual (typeof (SqlProvider), provider.GetType ());
      Assert.AreEqual (c_testDomainProviderID, provider.ID);
    }

    [Test]
    public void Reference ()
    {
      StorageProvider provider1 = _storageProviderManager[c_testDomainProviderID];
      StorageProvider provider2 = _storageProviderManager[c_testDomainProviderID];

      Assert.AreSame (provider1, provider2);
    }

    [Test]
    public void Disposing ()
    {
      RdbmsProvider provider = null;

      using (_storageProviderManager)
      {
        provider = (RdbmsProvider) _storageProviderManager[c_testDomainProviderID];
        provider.LoadDataContainer (DomainObjectIDs.Order1);

        Assert.IsTrue (provider.IsConnected);
      }

      Assert.IsFalse (provider.IsConnected);
    }
  }
}
