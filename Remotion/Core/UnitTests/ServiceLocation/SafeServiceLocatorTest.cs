// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Configuration;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Configuration.ServiceLocation;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class SafeServiceLocatorTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;
    private ServiceLocationConfiguration _previousConfiguration;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      _serviceLocatorProviderBackup = (ServiceLocatorProvider) PrivateInvoke.GetNonPublicStaticField (typeof (ServiceLocator), "currentProvider");
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (typeof (ServiceLocator), "currentProvider", _serviceLocatorProviderBackup);
    }

    [SetUp]
    public void SetUp ()
    {
      _previousConfiguration = ServiceLocationConfiguration.Current;
    }

    [TearDown]
    public void TearDown ()
    {
      ServiceLocationConfiguration.SetCurrent (_previousConfiguration);
      ResetDefaultServiceLocator ();
    }
 
    [Test]
    public void GetCurrent_WithLocatorProvider()
    {
      var serviceLocatorStub = MockRepository.GenerateStub <IServiceLocator>();
      ServiceLocator.SetLocatorProvider (() => serviceLocatorStub);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (serviceLocatorStub));
    }

    [Test]
    public void GetCurrent_WithLocatorProvider_IgnoresServiceConfiguration ()
    {
      var serviceLocatorStub = MockRepository.GenerateStub<IServiceLocator> ();
      ServiceLocator.SetLocatorProvider (() => serviceLocatorStub);

      ConfigureFakeServiceLocatorProvider ();

      Assert.That (SafeServiceLocator.Current, Is.SameAs (serviceLocatorStub));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsDefaultServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (null);

      Assert.That (SafeServiceLocator.Current, Is.TypeOf (typeof (DefaultServiceLocator)));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_ReturnsConfiguredServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (null);

      ConfigureFakeServiceLocatorProvider ();

      Assert.That (SafeServiceLocator.Current, Is.SameAs (FakeServiceLocatorProvider.Instance));
    }

    [Test]
    public void GetCurrent_WithoutLocatorProvider_SetsServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider (null);

      var safeCurrent = SafeServiceLocator.Current;
      Assert.That (ServiceLocator.Current, Is.Not.Null.And.SameAs (safeCurrent));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_ReturnsDefaultServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      Assert.That (SafeServiceLocator.Current, Is.TypeOf(typeof(DefaultServiceLocator)));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_ReturnsConfiguredServiceLocator ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      ConfigureFakeServiceLocatorProvider();

      Assert.That (SafeServiceLocator.Current, Is.SameAs (FakeServiceLocatorProvider.Instance));
    }

    [Test]
    public void GetCurrent_WithLocatorProviderReturningNull_DoesNotSetServiceLocatorCurrent ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      Dev.Null = SafeServiceLocator.Current;
      Assert.That (ServiceLocator.Current, Is.Null);
    }

    [Test]
    public void GetCurrent_WithInvalidServiceLocationConfiguration ()
    {
      ServiceLocator.SetLocatorProvider (() => null);

      ConfigureServiceLocatorProvider ("Blah");

      Assert.That (
          () => SafeServiceLocator.Current,
          Throws.InstanceOf<ConfigurationException>().With.Message.StartsWith (
              "The value of the property 'type' cannot be parsed. The error is: Could not load type 'Blah'"));
    }

    private class FakeServiceLocatorProvider : IServiceLocatorProvider
    {
      public static readonly IServiceLocator Instance = MockRepository.GenerateStub<IServiceLocator>();

      public IServiceLocator GetServiceLocator ()
      {
        return Instance;
      }
    }

    private void ConfigureFakeServiceLocatorProvider ()
    {
      var serviceLocatorTypeName = typeof (FakeServiceLocatorProvider).AssemblyQualifiedName;
      ConfigureServiceLocatorProvider(serviceLocatorTypeName);
    }

    private void ConfigureServiceLocatorProvider (string serviceLocatorTypeName)
    {
      var serviceLocationConfiguration = new ServiceLocationConfiguration();
      var xmlFragment = string.Format (@"<serviceLocation xmlns=""..."">
        <serviceLocatorProvider type=""{0}"" />
      </serviceLocation>", serviceLocatorTypeName);
      ConfigurationHelper.DeserializeSection (serviceLocationConfiguration, xmlFragment);
      ServiceLocationConfiguration.SetCurrent (serviceLocationConfiguration);
      ResetDefaultServiceLocator();
    }

    private void ResetDefaultServiceLocator ()
    {
      var defaultServiceLocatorContainer = 
          (DoubleCheckedLockingContainer<IServiceLocator>) PrivateInvoke.GetNonPublicStaticField (typeof (SafeServiceLocator), "s_defaultServiceLocator");
      defaultServiceLocatorContainer.Value = null;
    }
  }
}