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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
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
    [ExpectedException (typeof (ActivationException), ExpectedMessage = "InvalidOperationException: This exception comes from the ctor.")]
    public void GetInstance_ConstructorThrowingException ()
    {
      _serviceLocator.GetInstance (typeof (ITestConcreteImplementationAttributeTypeThrowingExceptionInCtor));
    }

    [Test]
    public void GetInstance_InstanceNotCompatibleWithServiceType ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => new object ());
      Assert.That (
          () => _serviceLocator.GetInstance (typeof (ISomeInterface)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The instance returned by the registered factory does not implement the requested type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'. (Instance type: 'System.Object'.)"));
    }

    [Test]
    public void GetInstance_ServiceTypeWithNullImplementation ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => null);
      Assert.That (
          () => _serviceLocator.GetInstance (typeof (ISomeInterface)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'."));
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
    public void GetInstance_WithMutipleServiceImplementationsRegistered ()
    {
      Assert.That (
          () => _serviceLocator.GetInstance<ITestMultipleConcreteImplementationAttributesType> (),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "Multiple implemetations are configured for service type: 'ITestMultipleConcreteImplementationAttributesType'. " 
              + "Consider using 'GetAllInstances'."));
    }

    [Test]
    public void GetAllInstances ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestInstanceConcreteImplementationAttributeType)).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result.Single(), Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage = "InvalidOperationException: This exception comes from the ctor.")]
    public void GetAllInstances_ConstructorThrowingException ()
    {
      _serviceLocator.GetAllInstances (typeof (ITestConcreteImplementationAttributeTypeThrowingExceptionInCtor)).ToArray();
    }

    [Test]
    public void GetAllInstances_InstanceNotCompatibleWithServiceType ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => new object());
      Assert.That (
          () => _serviceLocator.GetAllInstances (typeof (ISomeInterface)).ToArray (),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The instance returned by the registered factory does not implement the requested type " 
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'. (Instance type: 'System.Object'.)"));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithNullImplementation ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => null);
      Assert.That (
          () => _serviceLocator.GetAllInstances (typeof (ISomeInterface)).ToArray(),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'."));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Invalid ConcreteImplementationAttribute configuration for service type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType'. "
        + "Ambiguous ConcreteImplementationAttribute: Position must be unique.")]
    public void GetAllInstances_ServiceTypeWithAmbiguousPosition ()
    {
      _serviceLocator.GetAllInstances (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicatePositionType)).ToArray ();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Invalid ConcreteImplementationAttribute configuration for service type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType'. "
        + "Ambiguous ConcreteImplementationAttribute: Implementation type must be unique.")]
    public void GetAllInstances_ServiceTypeWithDuplicateImplementation ()
    {
      _serviceLocator.GetAllInstances (typeof (ITestMultipleConcreteImplementationAttributesWithDuplicateImplementationType)).ToArray ();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Invalid ConcreteImplementationAttribute configuration for service type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestConcreteImplementationAttributeTypeWithInvalidImplementation'. "
        + "The implementation type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestConcreteImplementationAttributeType' does not implement "
        + "the service type.")]
    public void GetAllInstances_ServiceTypeNotImplementedByImplementationType()
    {
      _serviceLocator.GetAllInstances (typeof (ITestConcreteImplementationAttributeTypeWithInvalidImplementation)).ToArray ();
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithMultipleConcreteImplementationAttributes ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestMultipleConcreteImplementationAttributesType)).ToArray ();

      Assert.That (result, Has.Length.EqualTo (3));
      Assert.That (result[0], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType2)));
      Assert.That (result[1], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType3)));
      Assert.That (result[2], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType1)));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (IServiceLocator));

      Assert.That (result, Is.Empty);
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
    public void GetInstance_Generic_ServiceTypeWithUnresolvableAndResolveImplementationTypes ()
    {
      var result = _serviceLocator.GetInstance<ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes> ();

      Assert.That (result, Is.TypeOf<TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting> ());
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances<IServiceLocator>();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances<ITestInstanceConcreteImplementationAttributeType>().ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result.Single(), Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithMultipleConcreteImplementationAttributes ()
    {
      var result = _serviceLocator.GetAllInstances<ITestMultipleConcreteImplementationAttributesType> ().ToArray();

      Assert.That (result, Has.Length.EqualTo(3));
      Assert.That (result[0], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType2)));
      Assert.That (result[1], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType3)));
      Assert.That (result[2], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType1)));
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithUnresolvableAndResolveImplementationTypes ()
    {
      var results = _serviceLocator.GetAllInstances<ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes>().ToArray();

      Assert.That (results, Has.Length.EqualTo (1));
      Assert.That (results[0], Is.TypeOf<TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting> ());
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
        ExpectedMessage = "Type 'TestTypeWithTooManyPublicConstructors' has not exactly one public constructor and cannot be instantiated.")]
    public void GetInstance_TypeWithTooManyPublicCtors ()
    {
      _serviceLocator.GetInstance<ITestTypeWithTooManyPublicConstructors>();
    }

    [Test]
    [ExpectedException (typeof (ActivationException),
        ExpectedMessage = "Type 'TestTypeWithOnlyProtectedConstructor' has not exactly one public constructor and cannot be instantiated.")]
    public void GetInstance_TypeWithOnlyProtectedCtor ()
    {
      _serviceLocator.GetInstance<ITestTypeWithOnlyProtectedConstructor> ();
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
    public void GetService_TypeWithMutipleServiceImplementationsRegistered ()
    {
      Assert.That (
          () => ((IServiceLocator) _serviceLocator).GetService (typeof (ITestMultipleConcreteImplementationAttributesType)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "Multiple implemetations are configured for service type: 'ITestMultipleConcreteImplementationAttributesType'. "
              + "Consider using 'GetAllInstances'."));
    }

    [Test]
    public void GetService_ServiceTypeWithUnresolvableAndResolveImplementationTypes ()
    {
      var result =
        ((IServiceLocator) _serviceLocator).GetService (typeof (ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes));

      Assert.That (result, Is.TypeOf<TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called twice or after GetInstance for service type: 'ITestInstanceConcreteImplementationAttributeType'.")
    ]
    public void Register_Factory_ServiceAlreadyExists ()
    {
      _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), () => new object());
    }

    [Test]
    public void Register_Factory_ServiceIsAdded ()
    {
      var instance = new TestConcreteImplementationAttributeType ();
      Func<object> instanceFactory = () => instance;

      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);

      Assert.That (_serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType)), Is.SameAs (instance));
    }

    [Test]
    public void Register_MultipleFactories_ServiceIsAdded ()
    {
      var instance1 = new object ();
      var instance2 = new object ();
      Func<object> instanceFactory1 = () => instance1;
      Func<object> instanceFactory2 = () => instance2;

      _serviceLocator.Register (typeof (object), instanceFactory1, instanceFactory2);

      Assert.That (_serviceLocator.GetAllInstances (typeof (object)), Is.EqualTo (new[] { instance1, instance2 }));
    }

    [Test]
    public void Register_NoFactories_OverridesAttributes ()
    {
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (_serviceLocator.GetAllInstances (typeof (object)), Is.Empty);
    }

    [Test]
    public void Register_Twice_ExceptionIsThrown ()
    {
      Func<object> instanceFactory = () => new object();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);

      Assert.That (
          () => _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'ITestInstanceConcreteImplementationAttributeType'."));
    }

    [Test]
    public void Register_ConcreteImplementation_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      Assert.That (
          () => _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'ITestSingletonConcreteImplementationAttributeType'."));
    }

    [Test]
    public void Register_ConcreteImplementation_ImplementationTypeDoesNotImplementServiceType_ThrowsException ()
    {
      Assert.That (
          () => _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (object), LifetimeKind.Singleton),
          Throws.ArgumentException.With.Message.EqualTo (
              "Implementation type must implement service type.\r\nParameter name: concreteImplementationType"));
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
        ExpectedMessage = "Register cannot be called twice or after GetInstance for service type: 'ITestSingletonConcreteImplementationAttributeType'.")
    ]
    public void Register_ServiceConfigurationEntry_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      var serviceImplementation = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), serviceImplementation);
      _serviceLocator.Register (serviceConfigurationEntry);
    }

    [Test]
    public void Register_ServiceConfigurationEntry_ServiceAdded ()
    {
      var serviceImplementation = new ServiceImplementationInfo (
          typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), serviceImplementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void Register_ServiceConfigurationEntry_MultipleServices ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType1), LifetimeKind.Singleton);
      var implementation2 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType2), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestMultipleRegistrationsType), implementation1, implementation2);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instances = _serviceLocator.GetAllInstances<ITestMultipleRegistrationsType> ().ToArray ();
      Assert.That (instances, Has.Length.EqualTo (2));
      Assert.That (instances[0], Is.TypeOf<TestMultipleRegistrationType1> ());
      Assert.That (instances[1], Is.TypeOf<TestMultipleRegistrationType2> ());
    }

    [Test]
    public void GetInstance_IEnumerable ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType1), LifetimeKind.Singleton);
      var implementation2 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType2), LifetimeKind.Singleton);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestMultipleRegistrationsType), implementation1, implementation2));
      _serviceLocator.Register (typeof (DomainType), typeof (DomainType), LifetimeKind.Singleton);

      var instances = ((DomainType) _serviceLocator.GetInstance (typeof (DomainType))).AllInstances.ToArray();

      Assert.That (instances, Has.Length.EqualTo (2));
      Assert.That (instances[0], Is.TypeOf<TestMultipleRegistrationType1> ());
      Assert.That (instances[1], Is.TypeOf<TestMultipleRegistrationType2> ());
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithIndirectActivationException': "
        + "Error resolving indirect dependendency of constructor parameter 'innerDependency' of type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ClassWithIndirectActivationException': Cannot get a concrete implementation of type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithoutImplementation': "
        + "Expected 'ConcreteImplementationAttribute' could not be found.")]
    public void GetInstance_IndirectActivationException_CausesFullMessageToBeBuilt ()
    {
      _serviceLocator.GetInstance<IInterfaceWithIndirectActivationException>();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithIndirectActivationExceptionForCollectionParameter': "
        + "Error resolving indirect collection dependendency of constructor parameter 'innerDependency' of type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ClassWithIndirectActivationExceptionForCollectionParameter': "
        + "InvalidOperationException: This exception comes from the ctor.")]
    public void GetInstance_IndirectActivationException_ForCollectionParameter_CausesFullMessageToBeBuilt ()
    {
      _serviceLocator.GetInstance<IInterfaceWithIndirectActivationExceptionForCollectionParameter> ();
    }

    class DomainType
    {
      public readonly IEnumerable<ITestMultipleRegistrationsType> AllInstances;

      public DomainType (IEnumerable<ITestMultipleRegistrationsType> allInstances)
      {
        AllInstances = allInstances;
      }
    }

    public interface ISomeInterface { }
  }
}