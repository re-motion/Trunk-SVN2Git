using System;
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration
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