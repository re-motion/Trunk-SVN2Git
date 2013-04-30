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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Configuration.ServiceLocation;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class SafeServiceLocatorTest
  {
    private ServiceLocatorProvider _serviceLocatorProviderBackup;
    private IServiceLocationConfiguration _previousConfiguration;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      _serviceLocatorProviderBackup = (ServiceLocatorProvider) PrivateInvoke.GetNonPublicStaticField (typeof (ServiceLocator), "currentProvider");
      PrivateInvoke.SetNonPublicStaticField (typeof (ServiceLocator), "currentProvider", null);
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

      ConfigureServiceLocatorProvider (MockRepository.GenerateStrictMock<IServiceLocatorProvider>());

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

      var serviceLocatorProvider = MockRepository.GenerateStub<IServiceLocatorProvider> ();
      var fakeServiceLocator = MockRepository.GenerateStub<IServiceLocator> ();
      serviceLocatorProvider.Stub (stub => stub.GetServiceLocator()).Return (fakeServiceLocator);
      
      ConfigureServiceLocatorProvider (serviceLocatorProvider);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (fakeServiceLocator));
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

      var serviceLocatorProvider = MockRepository.GenerateStub<IServiceLocatorProvider> ();
      var fakeServiceLocator = MockRepository.GenerateStub<IServiceLocator> ();
      serviceLocatorProvider.Stub (stub => stub.GetServiceLocator ()).Return (fakeServiceLocator);

      ConfigureServiceLocatorProvider (serviceLocatorProvider);

      Assert.That (SafeServiceLocator.Current, Is.SameAs (fakeServiceLocator));
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

      var exception = new Exception ();
      var serviceLocatorProvider = MockRepository.GenerateMock<IServiceLocatorProvider> ();
      serviceLocatorProvider.Expect (mock => mock.GetServiceLocator()).Throw (exception);

      ConfigureServiceLocatorProvider (serviceLocatorProvider);

      Assert.That (() => SafeServiceLocator.Current, Throws.Exception.SameAs (exception));
    }

    [Test]
    public void GetCurrent_ProvidesAccessToBootstrapLocator_WhileConfiguredLocatorIsConstructed ()
    {
      var serviceLocatorProvider = MockRepository.GenerateStub<IServiceLocatorProvider> ();
      var fakeServiceLocator = MockRepository.GenerateStub<IServiceLocator> ();
      serviceLocatorProvider
          .Stub (stub => stub.GetServiceLocator())
          .Return (null)
          .WhenCalled (
              mi =>
              {
                Assert.That (
                    SafeServiceLocator.Current,
                    Is.Not.Null.And.SameAs (((BootstrapServiceConfiguration) SafeServiceLocator.BootstrapConfiguration).BootstrapServiceLocator));
                mi.ReturnValue = fakeServiceLocator;
              });
      
      ConfigureServiceLocatorProvider (serviceLocatorProvider);

      var result = SafeServiceLocator.Current;

      Assert.That (result, Is.SameAs (fakeServiceLocator));
    }

    private void ConfigureServiceLocatorProvider (IServiceLocatorProvider serviceLocatorProvider)
    {
      var serviceLocationConfiguration = MockRepository.GenerateStub<IServiceLocationConfiguration>();
      serviceLocationConfiguration.Stub (stub => stub.CreateServiceLocatorProvider()).Return (serviceLocatorProvider);
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