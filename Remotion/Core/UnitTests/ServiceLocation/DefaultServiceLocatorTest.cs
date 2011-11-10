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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Implementation;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Cannot get a concrete implementation of type 'Microsoft.Practices.ServiceLocation.IServiceLocator': " +
        "Expected 'ConcreteImplementationAttribute' could not be found.")]
    public void GetInstance_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      _serviceLocator.GetInstance (typeof (IServiceLocator));
    }

    [Test]
    public void GetInstance_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (
          SafeServiceLocator.Current.GetInstance<ITestInstanceConcreteImplementationAttributeType>(),
          Is.InstanceOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage = "System.InvalidOperationException : This exception comes from the ctor.")]
    public void GetInstance_ConstructorThrowingException ()
    {
      _serviceLocator.GetInstance (typeof (ITestConcreteImplementationAttributeTypeThrowingExceptionInCtor));
    }

    [Test]
    public void GetInstance_InstanceLifeTime_ReturnsNotSameInstancesForAServiceType ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (instance1, Is.Not.SameAs (instance2));
    }

    [Test]
    public void GetInstance_SingletonLifeTime_ReturnsSameInstancesForAServiceType ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));

      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void GetInstance_WithKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType), "Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result.ToArray().Length, Is.EqualTo (1));
      Assert.That (result.ToArray()[0], Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (IServiceLocator));

      Assert.That (result.ToArray().Length, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "The implementation type does not implement the service interface. Unable to cast object of type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.TestConcreteImplementationAttributeType' to type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestConcreteImplementationAttributeTypeWithInvalidImplementation'.")]
    public void GetInstance_Generic_ServiceTypeWithWithIncompatibleImplementationType ()
    {
      _serviceLocator.GetInstance<ITestConcreteImplementationAttributeTypeWithInvalidImplementation>();
    }

    [Test]
    public void GetInstance_Generic ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_Generic_WithKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType> ("Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances<IServiceLocator>();

      Assert.That (result.ToArray().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances<ITestInstanceConcreteImplementationAttributeType>();

      Assert.That (result.ToArray().Length, Is.EqualTo (1));
      Assert.That (result.ToArray()[0], Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_OneParameter ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithOneParameter>();

      Assert.That (result, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (((TestConstructorInjectionWithOneParameter) result).Param, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_ThreeParametersRecursive ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithThreeParameters>();

      Assert.That (result, Is.TypeOf (typeof (TestConstructorInjectionWithThreeParameters)));

      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param1, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (
          ((TestConstructorInjectionWithOneParameter) ((TestConstructorInjectionWithThreeParameters) result).Param1).Param,
          Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param2, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (
          ((TestConstructorInjectionWithOneParameter) ((TestConstructorInjectionWithThreeParameters) result).Param2).Param,
          Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param3, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_InstanceLifeTime_ReturnsNotSameInstances_ForServiceParameter_WithInstanceLifetime ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithInstanceLifetime));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithInstanceLifetime));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (
          ((TestConstructorInjectionWithOneParameterWithInstanceLifetime) instance1).Param,
          Is.Not.SameAs (((TestConstructorInjectionWithOneParameterWithInstanceLifetime) instance2).Param));
    }

    [Test]
    public void GetInstance_ConstructorInjection_InstanceLifeTime_ReturnsNotSameInstances_ForServiceParameter_WithSingletonLifetime ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithSingletonLifetime));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithSingletonLifetime));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (
          ((TestConstructorInjectionWithOneParameterWithSingletonLifetime) instance1).Param,
          Is.SameAs (((TestConstructorInjectionWithOneParameterWithSingletonLifetime) instance2).Param));
    }

    [Test]
    [ExpectedException (typeof (ActivationException),
        ExpectedMessage = "Type 'TestTypeWithNotExactOnePublicConstructor' has not exactly one public constructor and cannot be instantiated.")]
    public void GetInstance_TypeHasNotExaclytOnePublicConstructor ()
    {
      _serviceLocator.GetInstance<ITestTypeWithNotExactOnePublicConstructor>();
    }

    [Test]
    public void GetService_TypeWithConcreteImplementationAttribute ()
    {
      var result = ((IServiceLocator) _serviceLocator).GetService (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetService_TypeWithoutConcreteImplementatioAttribute ()
    {
      var result = ((IServiceLocator) _serviceLocator).GetService (typeof (string));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called after GetInstance for service type: ITestInstanceConcreteImplementationAttributeType")
    ]
    public void Register_Factory_ServiceAlreadyExists ()
    {
      _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), () => new object());
    }

    [Test]
    public void Register_Factory_ServiceIsAdded ()
    {
      var instance = new object();
      Func<object> instanceFactory = () => instance;

      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);

      Assert.That (_serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType)), Is.SameAs (instance));
    }

    [Test]
    public void Register_Factory_SameServiceFactoryIsAddedTwice_NoExceptionIsThrown ()
    {
      Func<object> instanceFactory = () => new object();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called after GetInstance for service type: ITestSingletonConcreteImplementationAttributeType")]
    public void Register_ConcreteImplementation_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
    }

    [Test]
    public void Register_ConcreteImplementation_ServiceIsAdded ()
    {
      _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void Register_SingletonServiceIsLazyInitialized ()
    {
      _serviceLocator.Register (
          typeof (TestConstructorInjectionForServiceWithoutConcreteImplementationAttribute),
          typeof (TestConstructorInjectionForServiceWithoutConcreteImplementationAttribute),
          LifetimeKind.Singleton);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called after GetInstance for service type: ITestSingletonConcreteImplementationAttributeType")
    ]
    public void Register_ServiceConfigurationEntry_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      _serviceLocator.Register (serviceConfigurationEntry);
    }

    [Test]
    public void Register_ServiceConfigurationEntry_ServiceAdded ()
    {
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (instance2));
    }
  }
}