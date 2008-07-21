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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration
{
  [TestFixture]
  public class FakeDomainObjectsConfigurationTest
  {
    [Test]
    public void Initialize()
    {
      PersistenceConfiguration storage = new PersistenceConfiguration ();
      MappingLoaderConfiguration mappingLoader = new MappingLoaderConfiguration ();
      QueryConfiguration query = new QueryConfiguration ();
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (mappingLoader, storage, query);
    
      Assert.AreSame (mappingLoader, configuration.MappingLoader);
      Assert.AreSame (storage, configuration.Storage);
      Assert.AreSame (query, configuration.Query);
    }
  }
}
