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
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.SampleTypes;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using File=System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectFactoryTest : ClientTransactionBaseTest
  {
    private InterceptedDomainObjectFactory _factory;
    private readonly string _assemblyDirectory = Path.Combine (Environment.CurrentDirectory, "Interception.InterceptedDomainObjectFactoryTest.Dlls");

    public override void SetUp ()
    {
      base.SetUp();
      if (!Directory.Exists (_assemblyDirectory))
        Directory.CreateDirectory (_assemblyDirectory);
      _factory = new InterceptedDomainObjectFactory (_assemblyDirectory);
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsAssignableType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.IsTrue (typeof (Order).IsAssignableFrom (concreteType));
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsDifferentType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreNotEqual (typeof (Order), concreteType);
    }

    public abstract class SpecificDerivedDO : DerivedDO
    {
    }

    [Test]
    public void GetConcreteDomainObjectTypeForSpecificBaseType ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DerivedDO));
      Type concreteType = _factory.GetConcreteDomainObjectType (classDefinition, typeof (SpecificDerivedDO));
      Assert.AreNotEqual (typeof (DerivedDO), concreteType.BaseType);
      Assert.AreEqual (typeof (SpecificDerivedDO), concreteType.BaseType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument concreteBaseType is a Remotion.Data.UnitTests.DomainObjects.TestDomain.Official, which cannot be assigned to type "
        + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.\r\nParameter name: concreteBaseType")]
    public void GetConcreteDomainObjectTypeForSpecificBaseType_ThrowsOnInvalidSpecificType ()
    {
      ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      _factory.GetConcreteDomainObjectType (orderDefinition, typeof (Official));
    }

    [Test]
    public void FactoryCachesGeneratedTypes ()
    {
      Type concreteType1 = _factory.GetConcreteDomainObjectType (typeof (Order));
      Type concreteType2 = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreSame (concreteType1, concreteType2);
    }

    [Test]
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = _factory.SaveGeneratedAssemblies();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (_assemblyDirectory, "Remotion.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
      Assert.IsTrue (File.Exists (paths[0]));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSave ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      _factory.SaveGeneratedAssemblies();
      _factory.GetConcreteDomainObjectType (typeof (OrderItem));
      _factory.SaveGeneratedAssemblies();
      _factory.GetConcreteDomainObjectType (typeof (ClassWithAllDataTypes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseType")]
    public void AbstractWithoutInstantiableAttributeCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseTypeClassDefinition")]
    public void AbstractWithoutInstantiableAttributeCannotBeInstantiated_WithSpecificType ()
    {
      _factory.GetConcreteDomainObjectType (
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (AbstractClass)), typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.SampleTypes.NonInstantiableAbstractClass "
        + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an automatic property).\r\nParameter name: baseType")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.SampleTypes.NonInstantiableAbstractClassWithProps as its "
        + "member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property)."
        + "\r\nParameter name: baseType")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClassWithProps));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "SampleTypes.NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableSealedClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    public void WasCreatedByFactory ()
    {
      Assert.IsTrue (_factory.WasCreatedByFactory (_factory.GetConcreteDomainObjectType (typeof (Order))));
      Assert.IsFalse (_factory.WasCreatedByFactory (typeof (Order)));
    }

    [Test]
    public void GetTypesafeConstructorInvoker ()
    {
      IFuncInvoker<Order> invoker = _factory.GetTypesafeConstructorInvoker<Order> (_factory.GetConcreteDomainObjectType (typeof (Order)));
      Order order = invoker.With();
      Assert.IsNotNull (order);
      Assert.AreSame (_factory.GetConcreteDomainObjectType (typeof (Order)), ((object) order).GetType());
    }

    [Test]
    public void GetTypesafeConstructorInvokerWithConstructors ()
    {
      IFuncInvoker<DOWithConstructors> invoker =
          _factory.GetTypesafeConstructorInvoker<DOWithConstructors> (
              _factory.GetConcreteDomainObjectType (typeof (DOWithConstructors)));
      DOWithConstructors instance = invoker.With ("17", "4");
      Assert.IsNotNull (instance);
      Assert.AreEqual ("17", instance.FirstArg);
      Assert.AreEqual ("4", instance.SecondArg);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The type Remotion.Data.UnitTests.DomainObjects.TestDomain.Order was not "
        + "created by InterceptedDomainObjectFactory.GetConcreteDomainObjectType.\r\nParameter name: dynamicType")]
    public void GetTypesafeConstructorInvokerThrowsOnTypeNotCreatedByFactory ()
    {
      _factory.GetTypesafeConstructorInvoker<Order> (typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage = "Argument dynamicType is a .*Order_WithInterception.* which cannot be assigned to type .*OrderTicket.",
        MatchType = MessageMatch.Regex)]
    public void GetTypesafeConstructorInvokerThrowsOnInvalidTMinimimal ()
    {
      _factory.GetTypesafeConstructorInvoker<OrderTicket> (_factory.GetConcreteDomainObjectType (typeof (Order)));
    }

    [Test]
    public void PrepareUnconstructedInstance ()
    {
      Order order = (Order) FormatterServices.GetSafeUninitializedObject (_factory.GetConcreteDomainObjectType (typeof (Order)));
      _factory.PrepareUnconstructedInstance (order);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object's type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "InterceptedDomainObjectFactoryTest+DirectlyInstantiableDO was not created by "
        + "InterceptedDomainObjectFactory.GetConcreteDomainObjectType.\r\nParameter name: instance")]
    public void PrepareUnconstructedInstanceThrowsOnTypeNotCreatedByFactory ()
    {
      _factory.PrepareUnconstructedInstance (new DirectlyInstantiableDO());
    }

    [DBTable]
    private class DirectlyInstantiableDO : DomainObject
    {
      protected override void PerformConstructorCheck ()
      {
      }
    }
  }
}