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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  public class SchemaGenerationTestBase
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      
    }

    [SetUp]
    public virtual void SetUp ()
    {
      var setUpFixture = new SetUpFixture ();
      setUpFixture.SetUp ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetMappingConfiguration (); }
    }

    protected StorageConfiguration StorageConfiguration
    {
      get { return SchemaGenerationConfiguration.Instance.GetPersistenceConfiguration (); }
    }

    protected RdbmsProviderDefinition SchemaGenerationFirstStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.SchemaGenerationFirstStorageProviderID]; }
    }

    protected RdbmsProviderDefinition SchemaGenerationSecondStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.SchemaGenerationSecondStorageProviderID]; }
    }

    protected RdbmsProviderDefinition SchemaGenerationInternalStorageProviderDefinition
    {
      get { return (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.SchemaGenerationInternalStorageProviderID]; }
    }
  }
}