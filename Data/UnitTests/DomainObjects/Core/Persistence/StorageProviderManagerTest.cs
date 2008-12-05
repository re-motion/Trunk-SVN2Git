// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
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
