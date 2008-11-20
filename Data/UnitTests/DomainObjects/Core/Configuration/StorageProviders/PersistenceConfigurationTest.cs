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
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.StorageProviders
{
  [TestFixture]
  public class PersistenceConfigurationTest
  {
    private PersistenceConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
      _configuration = new PersistenceConfiguration();

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper ();
      configurationWrapper.SetUpConnectionString ("Rdbms", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent (configurationWrapper);
    }

    [TearDown]
    public void TearDown()
    {
      ConfigurationWrapper.SetCurrent (null);      
    }

    [Test]
    public void Initialize_WithProviderCollectionAndProvider()
    {
      StorageProviderDefinition providerDefinition1 = new RdbmsProviderDefinition ("ProviderDefinition1", typeof (SqlProvider), "ConnectionString");
      StorageProviderDefinition providerDefinition2 = new RdbmsProviderDefinition ("ProviderDefinition2", typeof (SqlProvider), "ConnectionString");
      StorageProviderDefinition providerDefinition3 = new RdbmsProviderDefinition ("ProviderDefinition3", typeof (SqlProvider), "ConnectionString");
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (providerDefinition1);
      providers.Add (providerDefinition2);

      PersistenceConfiguration configuration = new PersistenceConfiguration (providers, providerDefinition3);
      Assert.That (configuration.DefaultStorageProviderDefinition, Is.SameAs (providerDefinition3));
      Assert.That (configuration.StorageProviderDefinitions, Is.Not.SameAs (providers));
      Assert.That (configuration.StorageProviderDefinitions.Count, Is.EqualTo (2));
      Assert.That (providers["ProviderDefinition1"], Is.SameAs (providerDefinition1));
      Assert.That (providers["ProviderDefinition2"], Is.SameAs (providerDefinition2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Collection is read-only.")]
    public void Initialize_WithProviderCollectionAndProvider_Expect()
    {
      StorageProviderDefinition providerDefinition = new RdbmsProviderDefinition ("ProviderDefinition", typeof (SqlProvider), "ConnectionString");
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();

      PersistenceConfiguration configuration = new PersistenceConfiguration (providers, providerDefinition);
      configuration.StorageProviderDefinitions.Add (providerDefinition);
    }

    [Test]
    public void Deserialize_WithRdbmsProviderDefinition()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <providerDefinitions>
              <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.That (_configuration.DefaultStorageProviderDefinition, Is.InstanceOfType (typeof (RdbmsProviderDefinition)));
      Assert.That (_configuration.StorageProviderDefinitions.Count, Is.EqualTo (1));
      Assert.That (_configuration.StorageProviderDefinitions["Rdbms"], Is.SameAs (_configuration.DefaultStorageProviderDefinition));
      Assert.That (_configuration.DefaultStorageProviderDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (((RdbmsProviderDefinition) _configuration.DefaultStorageProviderDefinition).ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultProviderDefinition does not exist in the providers collection.")]
    public void Test_WithRdbmsProviderDefinitionAndInvalidName()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Invalid"">
            <providerDefinitions>
              <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Dev.Null = _configuration.DefaultStorageProviderDefinition;
    }

    [Test]
    public void Deserialize_WithStorageGroups()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <groups>
              <add type=""Remotion.Data.UnitTests::DomainObjects.Core.Configuration.StorageProviders.StubStorageGroup1Attribute"" 
                  provider=""Rdbms""/>
              <add type=""Remotion.Data.UnitTests::DomainObjects.Core.Configuration.StorageProviders.StubStorageGroup2Attribute"" 
                  provider=""Rdbms""/>
            </groups>
            <providerDefinitions>
              <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.That (_configuration.StorageGroups.Count, Is.EqualTo (2));
      Assert.That (_configuration.StorageGroups[0].StorageGroup, Is.InstanceOfType (typeof (StubStorageGroup1Attribute)));
      Assert.That (_configuration.StorageGroups[0].StorageProviderName, Is.EqualTo ("Rdbms"));
      Assert.That (_configuration.StorageGroups[1].StorageGroup, Is.InstanceOfType (typeof (StubStorageGroup2Attribute)));
      Assert.That (_configuration.StorageGroups[1].StorageProviderName, Is.EqualTo ("Rdbms"));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value of the property 'type' cannot be parsed.",
        MatchType = MessageMatch.Contains)]
    public void Deserialize_WithStorageGroupHavingInvalidTypeName()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <groups>
              <add type=""Invalid, Assembly"" provider=""Rdbms""/>
            </groups>
            <providerDefinitions>
              <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Dev.Null = _configuration.StorageGroups;
    }
  }
}
