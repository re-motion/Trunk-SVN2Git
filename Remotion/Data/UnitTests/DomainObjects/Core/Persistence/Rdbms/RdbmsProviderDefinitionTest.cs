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
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest : StandardMappingTest
  {
    private StorageProviderDefinition _definition;

    public override void SetUp()
    {
      base.SetUp();

      _definition = new RdbmsProviderDefinition ("StorageProviderID", typeof (SqlProvider), "ConnectionString");

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper();
      configurationWrapper.SetUpConnectionString ("SqlProvider", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent (configurationWrapper);
    }

    [Test]
    public void Initialize_FromArguments()
    {
      RdbmsProviderDefinition provider = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreSame (typeof (SqlProvider), provider.StorageProviderType);
      Assert.AreEqual ("ConnectionString", provider.ConnectionString);
      Assert.IsNotNull (provider.TypeConversionProvider);
    }

    [Test]
    public void Initialize_FromConfig()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("providerType", "Remotion.Data.DomainObjects::Persistence.Rdbms.SqlProvider");
      config.Add ("connectionString", "SqlProvider");

      RdbmsProviderDefinition provider = new RdbmsProviderDefinition ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
      Assert.AreSame (typeof (SqlProvider), provider.StorageProviderType);
      Assert.AreEqual ("ConnectionString", provider.ConnectionString);
      Assert.IsEmpty (config);
      Assert.IsNotNull (provider.TypeConversionProvider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The attribute 'providerType' is missing in the configuration of the 'Provider' provider.")]
    public void Initialize_FromConfig_WithMissingProviderType()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("connectionString", "SqlProvider");

      Dev.Null = new RdbmsProviderDefinition ("Provider", config);
    }

    [Test]
    public void IsIdentityTypeSupportedFalse()
    {
      Assert.IsFalse (_definition.IsIdentityTypeSupported (typeof (int)));
    }

    [Test]
    public void IsIdentityTypeSupportedTrue()
    {
      Assert.IsTrue (_definition.IsIdentityTypeSupported (typeof (Guid)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsIdentityTypeSupportedNull()
    {
      _definition.IsIdentityTypeSupported (null);
    }

    [Test]
    public void CheckValidIdentityType()
    {
      _definition.CheckIdentityType (typeof (Guid));
    }

    [Test]
    [ExpectedException (typeof (IdentityTypeNotSupportedException),
        ExpectedMessage = "The StorageProvider 'Remotion.Data.DomainObjects.Persistence.Rdbms.SqlProvider' does not support identity values of type 'System.String'.")]
    public void CheckInvalidIdentityType()
    {
      _definition.CheckIdentityType (typeof (string));
    }

    [Test]
    public void CheckDetailsOfInvalidIdentityType()
    {
      try
      {
        _definition.CheckIdentityType (typeof (string));
        Assert.Fail ("Test expects an IdentityTypeNotSupportedException.");
      }
      catch (IdentityTypeNotSupportedException ex)
      {
        Assert.AreEqual (typeof (SqlProvider), ex.StorageProviderType);
        Assert.AreEqual (typeof (string), ex.InvalidIdentityType);
      }
    }
  }
}
