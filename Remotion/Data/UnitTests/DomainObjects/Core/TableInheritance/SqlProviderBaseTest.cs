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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class SqlProviderBaseTest : TableInheritanceMappingTest
  {
    private StorageProviderManager _storageProviderManager;
    private SqlProvider _provider;
    private StorageProviderDefinition _storageProviderDefinition;
    private ReflectionBasedStorageNameProvider _storageNameProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager (NullPersistenceListener.Instance);
      _storageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      _storageNameProvider = new ReflectionBasedStorageNameProvider();
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

    protected StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    protected ReflectionBasedStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}
