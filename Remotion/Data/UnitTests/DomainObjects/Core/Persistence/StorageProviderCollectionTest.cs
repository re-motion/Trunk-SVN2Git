// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class StorageProviderCollectionTest : SqlProviderBaseTest
  {
    private StorageProviderCollection _collection;
    private StorageProvider _provider;
    private ReflectionBasedStorageNameProvider _storageNameProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _storageNameProvider = new ReflectionBasedStorageNameProvider();
      _provider = new SqlProvider (
          new RdbmsProviderDefinition ("TestDomain", new SqlStorageObjectFactory(), "ConnectionString"),
          _storageNameProvider,
          NullPersistenceListener.Instance,
          CommandFactory);
      _collection = new StorageProviderCollection();
    }

    [Test]
    public void ContainsProviderTrue ()
    {
      _collection.Add (_provider);
      Assert.IsTrue (_collection.Contains (_provider));
    }

    [Test]
    public void ContainsProviderFalse ()
    {
      _collection.Add (_provider);

      StorageProvider copy = new SqlProvider (
          (RdbmsProviderDefinition) _provider.StorageProviderDefinition, _storageNameProvider, NullPersistenceListener.Instance, CommandFactory);
      Assert.IsFalse (_collection.Contains (copy));
    }
  }
}