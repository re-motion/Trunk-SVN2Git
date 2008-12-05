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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using File=System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectFactoryTest : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetUpFixture.SetupAssemblyDirectory();
    }

    public override void TestFixtureTearDown ()
    {
      SetUpFixture.CleanupAssemblyDirectory();
      base.TestFixtureTearDown ();
    }

    public static string AssemblyDirectory
    {
      get { return SetUpFixture.AssemblyDirectory; }
    }

    public InterceptedDomainObjectFactory Factory
    {
      get { return SetUpFixture.Factory; }
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsAssignableType ()
    {
      Type concreteType = Factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.IsTrue (typeof (Order).IsAssignableFrom (concreteType));
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsDifferentType ()
    {
      Type concreteType = Factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreNotEqual (typeof (Order), concreteType);
    }

    [Test]
    public void GetConcreteDomainObjectTypeForSpecificBaseType ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DerivedDO));
      Type concreteType = Factory.GetConcreteDomainObjectType (classDefinition, typeof (SpecificDerivedDO));
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
      Factory.GetConcreteDomainObjectType (orderDefinition, typeof (Official));
    }

    [Test]
    public void FactoryCachesGeneratedTypes ()
    {
      Type concreteType1 = Factory.GetConcreteDomainObjectType (typeof (Order));
      Type concreteType2 = Factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreSame (concreteType1, concreteType2);
    }

    [Test]
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      Factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = Factory.SaveGeneratedAssemblies();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (AssemblyDirectory, "Remotion.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
      Assert.IsTrue (File.Exists (paths[0]));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSave ()
    {
      Factory.GetConcreteDomainObjectType (typeof (Order));
      Factory.SaveGeneratedAssemblies();
      Factory.GetConcreteDomainObjectType (typeof (ClassWithAllDataTypes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseType")]
    public void AbstractWithoutInstantiableAttributeCannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseTypeClassDefinition")]
    public void AbstractWithoutInstantiableAttributeCannotBeInstantiated_WithSpecificType ()
    {
      Factory.GetConcreteDomainObjectType (
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (AbstractClass)), typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClass "
        + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an automatic property).\r\nParameter name: baseType")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClassWithProps as its "
        + "member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property)."
        + "\r\nParameter name: baseType")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClassWithProps));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "TestDomain.NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableSealedClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    public void WasCreatedByFactory ()
    {
      Assert.IsTrue (Factory.WasCreatedByFactory (Factory.GetConcreteDomainObjectType (typeof (Order))));
      Assert.IsFalse (Factory.WasCreatedByFactory (typeof (Order)));
    }

    [Test]
    public void GetTypesafeConstructorInvoker ()
    {
      IFuncInvoker<Order> invoker = Factory.GetTypesafeConstructorInvoker<Order> (Factory.GetConcreteDomainObjectType (typeof (Order)));
      Order order = invoker.With();
      Assert.IsNotNull (order);
      Assert.AreSame (Factory.GetConcreteDomainObjectType (typeof (Order)), ((object) order).GetType());
    }

    [Test]
    public void GetTypesafeConstructorInvokerWithConstructors ()
    {
      IFuncInvoker<DOWithConstructors> invoker =
          Factory.GetTypesafeConstructorInvoker<DOWithConstructors> (
              Factory.GetConcreteDomainObjectType (typeof (DOWithConstructors)));
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
      Factory.GetTypesafeConstructorInvoker<Order> (typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage = "Argument dynamicType is a .*Order_WithInterception.* which cannot be assigned to type .*OrderTicket.",
        MatchType = MessageMatch.Regex)]
    public void GetTypesafeConstructorInvokerThrowsOnInvalidTMinimimal ()
    {
      Factory.GetTypesafeConstructorInvoker<OrderTicket> (Factory.GetConcreteDomainObjectType (typeof (Order)));
    }

    [Test]
    public void PrepareUnconstructedInstance ()
    {
      Order order = (Order) FormatterServices.GetSafeUninitializedObject (Factory.GetConcreteDomainObjectType (typeof (Order)));
      Factory.PrepareUnconstructedInstance (order);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object's type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "TestDomain.DirectlyInstantiableDO was not created by "
        + "InterceptedDomainObjectFactory.GetConcreteDomainObjectType.\r\nParameter name: instance")]
    public void PrepareUnconstructedInstanceThrowsOnTypeNotCreatedByFactory ()
    {
      Factory.PrepareUnconstructedInstance (new DirectlyInstantiableDO());
    }
  }
}
