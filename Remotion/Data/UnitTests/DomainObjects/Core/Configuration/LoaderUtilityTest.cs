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
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration
{
  [TestFixture]
  public class LoaderUtilityTest
  {
    private FakeConfigurationWrapper _configurationWrapper;

    [SetUp]
    public void SetUp()
    {
      _configurationWrapper = new FakeConfigurationWrapper();
      _configurationWrapper.SetUpConnectionString ("Rdbms", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent (_configurationWrapper);
    }

    [TearDown]
    public void TearDown()
    {
      ConfigurationWrapper.SetCurrent (null);
    }

    [Test]
    public void GetConfigurationFileName()
    {
      _configurationWrapper.SetUpAppSetting ("ConfigurationFileThatDoesNotExist", @"C:\NonExistingConfigurationFile.xml");

      Assert.AreEqual (
          @"C:\NonExistingConfigurationFile.xml",
          LoaderUtility.GetConfigurationFileName ("ConfigurationFileThatDoesNotExist", "Mapping.xml"));
    }

    [Test]
    public void GetEmptyConfigurationFileName()
    {
      _configurationWrapper.SetUpAppSetting ("EmptyConfigurationFileName", string.Empty);

      Assert.AreEqual (string.Empty, LoaderUtility.GetConfigurationFileName ("EmptyConfigurationFileName", "Mapping.xml"));
    }

    [Test]
    public void GetConfigurationFileNameForNonExistingAppSettingsKey()
    {
      Assert.AreEqual (
          Path.Combine (ReflectionUtility.GetConfigFileDirectory(), "Mapping.xml"),
          LoaderUtility.GetConfigurationFileName ("AppSettingKeyDoesNotExist", "Mapping.xml"));
    }

    [Test]
    public void GetTypeWithTypeUtilityNotation ()
    {
      Assert.AreEqual (typeof (LoaderUtility), LoaderUtility.GetType ("Remotion.Data.DomainObjects::ConfigurationLoader.XmlBasedConfigurationLoader.LoaderUtility"));
    }
  }
}
