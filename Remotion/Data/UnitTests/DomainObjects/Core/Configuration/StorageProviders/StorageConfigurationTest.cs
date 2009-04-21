// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class StorageConfigurationTest
  {
    private StorageConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
      _configuration = new StorageConfiguration();

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

      StorageConfiguration configuration = new StorageConfiguration (providers, providerDefinition3);
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

      StorageConfiguration configuration = new StorageConfiguration (providers, providerDefinition);
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
    public void Deserialize_WithoutDefaultProviderDefinition ()
    {
      string xmlFragment =
          @"<storage>
            <providerDefinitions>
              <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.That (_configuration.DefaultStorageProviderDefinition, Is.Null);
      Assert.That (_configuration.StorageProviderDefinitions.Count, Is.EqualTo (1));
      Assert.That (_configuration.StorageProviderDefinitions["Rdbms"].StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (((RdbmsProviderDefinition) _configuration.StorageProviderDefinitions["Rdbms"]).ConnectionString, Is.EqualTo ("ConnectionString"));
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
