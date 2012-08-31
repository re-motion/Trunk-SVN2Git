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
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using File = System.IO.File;
using Throws = NUnit.Framework.Throws;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectTypeFactoryTest : ClientTransactionBaseTest
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

    public InterceptedDomainObjectTypeFactory Factory
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
      ClassDefinition classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (DerivedDO));
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
      ClassDefinition orderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Order));
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
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.")]
    public void AbstractWithoutInstantiableAttribute_CannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.TestDomain.AbstractClass as it is abstract; "
        + "for classes with automatic properties, InstantiableAttribute must be used.")]
    public void AbstractWithoutInstantiableAttribute_CannotBeInstantiated_WithSpecificType ()
    {
      Factory.GetConcreteDomainObjectType (
          MappingConfiguration.Current.GetTypeDefinition (typeof (AbstractClass)), typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClass "
        + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an automatic property).")]
    public void AbstractWithMethod_CannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClass));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClassWithProps as its "
        + "member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property).")]
    public void AbstractWithNonAutoProperties_CannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClassWithProps));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type " 
        + "'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableClassWithMixinWithPersistentAutoProperties' "
        + "because the mixin member 'MixinWithAutoProperties.PersistentAutoProperty' is an automatic property. Mixins must implement their "
        + "persistent members by using 'Properties' to get and set property values.")]
    public void ClassWithMixinWithAutoProperties_CannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableClassWithMixinWithPersistentAutoProperties));
    }

    [Test]
    public void ClassWithMixinWithStorageClassNoneAutoProperties_CanBeInstantiated ()
    {
      Assert.That (() => Factory.GetConcreteDomainObjectType (typeof (InstantiableClassWithMixinWithStorageClassNoneAutoProperties)), Throws.Nothing);
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableSealedClass as it "
        + "is sealed.")]
    public void Sealed_CannotBeInstantiated ()
    {
      Factory.GetConcreteDomainObjectType (typeof (NonInstantiableSealedClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomain_CannotBeInstantiated ()
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
    public void PrepareUnconstructedInstance ()
    {
      var order = (Order) FormatterServices.GetSafeUninitializedObject (Factory.GetConcreteDomainObjectType (typeof (Order)));
      Factory.PrepareUnconstructedInstance (order);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The domain object's type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.DirectlyInstantiableDO was not "
        + "created by InterceptedDomainObjectTypeFactory.GetConcreteDomainObjectType.\r\nParameter name: instance")]
    public void PrepareUnconstructedInstanceThrowsOnTypeNotCreatedByFactory ()
    {
      var directlyInstantiatedObject = ObjectLifetimeAgentTestHelper.CallWithInitializationContext (
          TestableClientTransaction, DomainObjectIDs.Order1, () => new DirectlyInstantiableDO());

      Factory.PrepareUnconstructedInstance (directlyInstantiatedObject);
    }
  }
}
