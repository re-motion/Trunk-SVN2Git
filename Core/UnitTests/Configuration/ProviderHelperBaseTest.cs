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
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class ProviderHelperBaseTest
  {
    private StubProviderHelper _providerHelper;
    private ConfigurationPropertyCollection _propertyCollection;
    private StubExtendedConfigurationSection _stubConfigurationSection;

    [SetUp]
    public void SetUp ()
    {
      _stubConfigurationSection = new StubExtendedConfigurationSection ("WellKnown", "defaultProvider", "Default Value", "providers");
      _providerHelper = _stubConfigurationSection.GetStubProviderHelper();
      _propertyCollection = _stubConfigurationSection.GetProperties();
      _providerHelper.InitializeProperties (_propertyCollection);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (2, _propertyCollection.Count);

      ConfigurationProperty defaultProviderProperty = _propertyCollection["defaultProvider"];
      Assert.IsNotNull (defaultProviderProperty);
      Assert.AreEqual (typeof (string), defaultProviderProperty.Type);
      Assert.AreEqual ("Default Value", defaultProviderProperty.DefaultValue);
      Assert.IsFalse (defaultProviderProperty.IsRequired);
      Assert.IsInstanceOfType (typeof (StringValidator), defaultProviderProperty.Validator);

      ConfigurationProperty providersProperty = _propertyCollection["providers"];
      Assert.IsNotNull (providersProperty);
      Assert.AreEqual (typeof (ProviderSettingsCollection), providersProperty.Type);
      Assert.IsNull (providersProperty.DefaultValue);
      Assert.IsFalse (providersProperty.IsRequired);
      Assert.IsInstanceOfType (typeof (DefaultValidator), providersProperty.Validator);
    }

    [Test]
    public void GetProviders ()
    {
      string xmlFragment =
          @"
          <stubConfigSection>
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.AreEqual (2, _providerHelper.Providers.Count);
      Assert.IsInstanceOfType (typeof (FakeProvider), _providerHelper.Providers["Fake"]);
    }

    [Test]
    public void GetProvider ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""Fake"">
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeProvider), _providerHelper.Provider);
      Assert.AreSame (_providerHelper.Providers["Fake"], _providerHelper.Provider);
    }

    [Test]
    public void GetProvider_WithWellKnownProvider ()
    {
      string xmlFragment = @"<stubConfigSection defaultProvider=""WellKnown"" />";
      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
      Assert.IsInstanceOfType (typeof (FakeWellKnownProvider), _providerHelper.Provider);
    }

    [Test]
    public void GetProvider_WithoutDefaultProvider ()
    {
      StubExtendedConfigurationSection stubConfigurationSection =
          new StubExtendedConfigurationSection ("WellKnown", "defaultProvider", null, "providers");
      StubProviderHelper providerHelper = stubConfigurationSection.GetStubProviderHelper ();
      providerHelper.InitializeProperties (_stubConfigurationSection.GetProperties ());

      Assert.IsNull (providerHelper.Provider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultProvider does not exist in the providers collection.")]
    public void GetProvider_WithInvalidProviderName ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""Invalid"">
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Dev.Null = _providerHelper.Provider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'WellKnown' identifies a well known provider and cannot be reused for custom providers.")]
    public void PostDeserialize_DuplicateWellKnownProvider ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""WellKnown"">
            <providers>
              <add name=""WellKnown"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
    }

    [Test]
    public void GetType_Test ()
    {
      Type type = _providerHelper.GetType (
          _propertyCollection["defaultProvider"],
          typeof (FakeProvider).Assembly.GetName(),
          "Remotion.UnitTests.Configuration.FakeProvider");

      Assert.AreSame (typeof (FakeProvider), type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage =
            "The current value of property 'defaultProvider' requires that the assembly 'Invalid' is placed within the CLR's probing path for this application."
        )]
    public void GetType_WithInvalidAssemblyName ()
    {
      _providerHelper.GetType (
          _propertyCollection["defaultProvider"],
          new AssemblyName ("Invalid"),
          "Remotion.UnitTests.Configuration.FakeProvider");
    }

    [Test]
    public void GetTypeWithMatchingVersionNumber ()
    {
      Type type = _providerHelper.GetTypeWithMatchingVersionNumber (
          _propertyCollection["defaultProvider"],
          "Remotion.UnitTests",
          "Remotion.UnitTests.Configuration.FakeProvider");

      Assert.AreSame (typeof (FakeProvider), type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void GetTypeWithMatchingVersionNumber_WithInvalidAssemblyName ()
    {
      _providerHelper.GetTypeWithMatchingVersionNumber (
          _propertyCollection["defaultProvider"],
          "Invalid",
          "Remotion.UnitTests.Configuration.FakeProvider");
    }

    [Test]
    public void InstantiateProvider ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProvider");
      providerSettings.Parameters.Add ("description", "The Description");

      ProviderBase providerBase = _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.IsNotNull (providerBase);
      Assert.IsInstanceOfType (typeof (FakeProvider), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
      Assert.AreEqual ("The Description", providerBase.Description);
    }


    [Test]
    public void InstantiateProvider_WithConstructorException ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.ThrowingFakeProvider");
      providerSettings.Parameters.Add ("description", "The Description");

      try
      {
        _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));
        Assert.Fail ("Expected ConfigurationErrorsException.");
      }
      catch (ConfigurationErrorsException ex)
      {
        Assert.IsInstanceOfType (typeof (TargetInvocationException), ex.InnerException);
        Assert.IsInstanceOfType (typeof (ConstructorException), ex.InnerException.InnerException);
        Assert.AreEqual ("A message from the constructor.", ex.Message);
      }
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), ExpectedMessage = "Type name must be specified for this provider.")]
    public void InstantiateProvider_WithMissingTypeName ()
    {
      _providerHelper.InstantiateProvider (new ProviderSettings(), typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the class 'Remotion.UnitTests.Configuration.FakeProviderBase'.")]
    public void InstantiateProvider_WithTypeNotDerivedFromRequiredBaseType ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeOtherProvider");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the interface 'Remotion.UnitTests.Configuration.IFakeProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProviderBase");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));
    }

    [Test]
    public void InstantiateProviders ()
    {
      ProviderSettingsCollection providerSettingsCollection = new ProviderSettingsCollection();
      providerSettingsCollection.Add (new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProvider"));
      ProviderCollection providerCollection = new ProviderCollection();

      _providerHelper.InstantiateProviders (providerSettingsCollection, providerCollection, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.AreEqual (1, providerCollection.Count);
      ProviderBase providerBase = providerCollection["Custom"];
      Assert.IsInstanceOfType (typeof (FakeProvider), providerBase);
      Assert.AreEqual ("Custom", providerBase.Name);
    }
  }
}
