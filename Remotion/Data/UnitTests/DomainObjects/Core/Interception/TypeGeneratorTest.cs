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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [TestFixture]
  public class TypeGeneratorTest : ClientTransactionBaseTest
  {
    private const BindingFlags _declaredPublicInstanceFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
    private const BindingFlags _declaredInstanceFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      SetupAssemblyDirectory ();

      // setup mixin builder to generate files into the same directory
      ConcreteTypeBuilder.SetCurrent (null); // reinitialize ConcreteTypeBuilder
      ConcreteTypeBuilder.Current.Scope.SignedModulePath = Path.Combine (AssemblyDirectory, ConcreteTypeBuilder.Current.Scope.SignedAssemblyName + ".dll");
      ConcreteTypeBuilder.Current.Scope.UnsignedModulePath = Path.Combine (AssemblyDirectory, ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName + ".dll");
    }

    private string AssemblyDirectory
    {
      get { return SetUpFixture.AssemblyDirectory; }
    }

    private InterceptedDomainObjectFactory Factory
    {
      get { return SetUpFixture.Factory; }
    }

    private void SetupAssemblyDirectory ()
    {
      SetUpFixture.SetupAssemblyDirectory ();

      Module unitTestAssemblyModule = Assembly.GetExecutingAssembly ().ManifestModule;
      File.Copy (unitTestAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, unitTestAssemblyModule.Name));

      Module domainObjectAssemblyModule = typeof (DomainObject).Assembly.ManifestModule;
      File.Copy (domainObjectAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, domainObjectAssemblyModule.Name));

      Module mixinAssemblyModule = typeof (Mixin).Assembly.ManifestModule;
      File.Copy (mixinAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, mixinAssemblyModule.Name));

      Module coreAssemblyModule = typeof (ArgumentUtility).Assembly.ManifestModule;
      File.Copy (coreAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, coreAssemblyModule.Name));

      Module objectBindingInterfacesModule = typeof (IBusinessObject).Assembly.ManifestModule;
      File.Copy (objectBindingInterfacesModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, objectBindingInterfacesModule.Name));

      Module objectBindingModule = typeof (BindableObjectBase).Assembly.ManifestModule;
      File.Copy (objectBindingModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, objectBindingModule.Name));
    }

    public override void TestFixtureTearDown ()
    {
      string[] paths = Factory.SaveGeneratedAssemblies ();
      // save mixins as well, we need those files if the intercepted types depend on mixed types
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

#if !NO_PEVERIFY
      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);
#endif

      base.TestFixtureTearDown ();
    }

    private TypeGenerator CreateTypeGenerator (Type baseType)
    {
      return Factory.Scope.CreateTypeGenerator (baseType, baseType);
    }

    private TypeGenerator CreateTypeGenerator (Type publicDomainObjectType, Type typeToDeriveFrom)
    {
      return Factory.Scope.CreateTypeGenerator (publicDomainObjectType, typeToDeriveFrom);
    }

    [Test]
    public void GeneratedTypeHasOtherNameThanBaseType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).Name, type.Name);
      Assert.AreNotEqual (typeof (DOWithVirtualProperties).FullName, type.Name);
    }

    [Test]
    public void EachGeneratedTypeHasDifferentName ()
    {
      Type type1 = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Type type2 = new InterceptedDomainObjectFactory(Environment.CurrentDirectory).GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.AreNotSame (type1, type2);
      Assert.AreNotEqual (type1.Name, type2.Name);
    }

    [Test]
    public void ReplicatesConstructors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      Assert.IsNotNull (type.GetConstructor (new[] { typeof (string), typeof (string) }));
      Assert.IsNotNull (type.GetConstructor (new[] { typeof (int) }));
    }

    [Test]
    public void ReplicatedConstructorsDelegateToBase ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      var instance1 = (DOWithConstructors) Activator.CreateInstance (type, "Foo", "Bar");
      Assert.AreEqual ("Foo", instance1.FirstArg);
      Assert.AreEqual ("Bar", instance1.SecondArg);

      var instance2 = (DOWithConstructors) Activator.CreateInstance (type, 7);
      Assert.AreEqual ("7", instance2.FirstArg);
      Assert.IsNull (instance2.SecondArg);
    }

    [Test]
    public void OverridesGetPublicDomainObjectType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectTypeImplementation", _declaredInstanceFlags));
    }

    [Test]
    public void OverridesGetPublicDomainObjectTypeToReturnBaseType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      Assert.AreEqual (typeof (DOWithVirtualProperties), instance.GetPublicDomainObjectType ());
      Assert.IsNotNull (type.GetMethod ("GetPublicDomainObjectTypeImplementation", _declaredInstanceFlags));
    }

    [Test]
    public void DosNotOverrideVirtualProperties ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.IsNull (type.GetProperty ("PropertyWithGetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("PropertyWithSetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("ProtectedProperty", _declaredInstanceFlags));
    }

    [Test]
    public void OverridesVirtualPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags));

      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags));
    }

    [Test]
    public void OverriddenVirtualPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily);
      Assert.IsTrue (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily);
    }

    [Test]
    public void OverridesVirtualPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.PropertyWithGetterAndSetter = 17;
      Assert.AreEqual (17, instance.PropertyWithGetterAndSetter);

      Assert.IsNull (instance.PropertyWithGetterOnly);
      instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.AreEqual ("hear, hear", instance.PropertyWithGetterOnly);

      Assert.AreEqual (new DateTime(), instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime>());
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.AreEqual (new DateTime (2260, 1, 2), instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());

      typeof (DOWithVirtualProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 67, _declaredInstanceFlags, null, null, null);
      Assert.AreEqual (67, typeof (DOWithVirtualProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .GetValue (instance, _declaredInstanceFlags, null, null, null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInGetter ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      try
      {
        Dev.Null = instance.PropertyThrowing;
        Assert.Fail ("Expected exception");
      }
      catch
      {
        Dev.Null = new object();
      }
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInSetter ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);

      try
      {
        instance.PropertyThrowing = DateTime.Now;
        Assert.Fail ("Expected exception");
      }
      catch
      {
        Dev.Null = new object ();
      }
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void DoesNotOverridePropertiesNotInMapping ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      Dev.Null = instance.PropertyNotInMapping;
    }

    [Test]
    public void IgnoresMixedProperties ()
    {
      Type mixedBaseType = TypeFactory.GetConcreteType (typeof (TargetClassForPersistentMixin));
      // save mixin scope to enable peverifying the intercepted type
      Assert.IsTrue (MixinTypeUtility.HasMixin (mixedBaseType, typeof (MixinAddingPersistentProperties)));
      Type type = CreateTypeGenerator (typeof (TargetClassForPersistentMixin), mixedBaseType).BuildType ();
      Assert.That (type.GetProperties (_declaredInstanceFlags), Is.Empty);
    }

    [Test]
    public void DosNotImplementAbstractProperties ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.IsNull (type.GetProperty ("PropertyWithGetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("PropertyWithSetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetProperty ("ProtectedProperty", _declaredInstanceFlags));
    }

    [Test]
    public void ImplementsAbstractPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags));

      Assert.IsNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags));
    }

    [Test]
    public void ImplementsAbstractPropertyAccessors_CollectionSetters ()
    {
      Type type = CreateTypeGenerator (typeof (ClassWithAbstractRelatedCollectionSetter)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (ClassWithAbstractRelatedCollectionSetter).FullName + ".get_RelatedObjects", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (ClassWithAbstractRelatedCollectionSetter).FullName + ".set_RelatedObjects", _declaredInstanceFlags));
    }

    [Test]
    public void ImplementedAbstractPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily);
    }

    [Test]
    public void ImplementsAbstractPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.PropertyWithGetterAndSetter = 17;
      Assert.AreEqual (17, instance.PropertyWithGetterAndSetter);

      Assert.IsNull (instance.PropertyWithGetterOnly);
      instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.AreEqual ("hear, hear", instance.PropertyWithGetterOnly);

      Assert.AreEqual (new DateTime (), instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.AreEqual (new DateTime (2260, 1, 2), instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> ());

      typeof (DOWithAbstractProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 4711, _declaredInstanceFlags, null, null, null);
      Assert.AreEqual (4711, typeof (DOWithAbstractProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .GetValue (instance, _declaredInstanceFlags, null, null, null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertyGettersCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertySettersCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) Activator.CreateInstance (type);

      instance.PropertyWithGetterAndSetter = 17;
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    public void ImplementsAutomaticPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags));

      Assert.IsNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));

      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags));
      Assert.IsNotNull (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags));
    }

    [Test]
    public void ImplementedAutomaticPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily);
      Assert.IsTrue (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily);
    }

    [Test]
    public void ImplementsAutomaticProperties_ViaPropertyIndexer ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      var instance = (DOWithAutomaticProperties) Activator.CreateInstance (type);

      Assert.AreEqual (0, instance.PropertyWithGetterAndSetter);
      instance.PropertyWithGetterAndSetter = 17;
      Assert.AreEqual (17, instance.PropertyWithGetterAndSetter);
      Assert.AreEqual (17, instance.Properties.Find ("PropertyWithGetterAndSetter").GetValueWithoutTypeCheck ());
      instance.Properties.Find ("PropertyWithGetterAndSetter").SetValueWithoutTypeCheck (8);
      Assert.AreEqual (8, instance.PropertyWithGetterAndSetter);
      
      Assert.IsNull (instance.PropertyWithGetterOnly);
      instance.Properties.Find ("PropertyWithGetterOnly").SetValue ("hear, hear");
      Assert.AreEqual ("hear, hear", instance.PropertyWithGetterOnly);

      Assert.AreEqual (new DateTime (), instance.Properties.Find ("PropertyWithSetterOnly").GetValue<DateTime> ());
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.AreEqual (new DateTime (2260, 1, 2), instance.Properties.Find ("PropertyWithSetterOnly").GetValue<DateTime> ());

      typeof (DOWithAutomaticProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 4711, _declaredInstanceFlags, null, null, null);
      Assert.AreEqual (4711, instance.Properties.Find ("ProtectedProperty").GetValueWithoutTypeCheck ());
      instance.Properties.Find ("ProtectedProperty").SetValueWithoutTypeCheck (9);
      Assert.AreEqual (9, typeof (DOWithAutomaticProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .GetValue (instance, _declaredInstanceFlags, null, null, null));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests."
        + "DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClassWithProps as its member get_Foo (on type NonInstantiableAbstractClassWithProps) is "
        + "abstract (and not an automatic property).")]
    public void ThrowsOnAbstractPropertyNotInMapping ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClassWithProps)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests."
       + "DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClass as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an "
        + "automatic property).")]
    public void ThrowsOnAbstractMethod ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException),
        ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Interception."
        + "TestDomain.NonInstantiableSealedClass as it is sealed.")]
    public void ThrowsOnSealedBaseType ()
    {
      CreateTypeGenerator (typeof (NonInstantiableSealedClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests."
        + "DomainObjects.Core.Interception.TestDomain.NonInstantiableAbstractClassWithoutAttribute as it is abstract and not instantiable.")]
    public void ThrowsOnAbstractClassWithoutInstantiableAttribute ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClassWithoutAttribute)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests."
       + "DomainObjects.Core.Interception.TestDomain.NonInstantiableNonDomainClass as it is not part of the mapping.")]
    public void ThrowsOnClassWithoutClassDefinition ()
    {
      CreateTypeGenerator (typeof (NonInstantiableNonDomainClass)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects." 
        + "Core.Interception.TestDomain.ClassWithNonVirtualAutomaticPropertyGetter' as its member 'RelatedObjects' has a non-virtual get accessor.")]
    public void ThrowsOnNonVirtualAutomaticPropertyGetter ()
    {
      CreateTypeGenerator (typeof (ClassWithNonVirtualAutomaticPropertyGetter)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects."
        + "Core.Interception.TestDomain.ClassWithNonVirtualAutomaticPropertySetter' as its member 'RelatedObjects' has a non-virtual set accessor.")]
    public void ThrowsOnNonVirtualAutomaticPropertySetter ()
    {
      CreateTypeGenerator (typeof (ClassWithNonVirtualAutomaticPropertySetter)).BuildType ();
    }

    [Test]
    public void DoesntThrowOnNonVirtualManualPropertyAccessors ()
    {
      CreateTypeGenerator (typeof (ClassWithNonVirtualManualPropertyAccessors)).BuildType ();
    }

    [Test]
    public void GeneratedTypeImplementsISerializable ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.IsTrue (typeof (ISerializable).IsAssignableFrom (type));
    }

    [Test]
    public void GeneratedTypeCanBeSerialized ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      var instance = (DOWithVirtualProperties) Activator.CreateInstance (type);
      instance.PropertyWithGetterAndSetter = 17;

      Tuple<ClientTransaction, DOWithVirtualProperties> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOWithVirtualProperties> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.A.EnterDiscardingScope ())
      {
        Assert.AreEqual (17, data.B.PropertyWithGetterAndSetter);
      }
    }

    [Test]
    public void GeneratedTypeCanBeSerializedWhenItImplementsISerializable ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingISerializable)).BuildType ();
      var instance = (DOImplementingISerializable) Activator.CreateInstance (type, "Start");
      instance.PropertyWithGetterAndSetter = 23;
      Assert.AreEqual ("Start", instance.MemberHeldAsField);

      Tuple<ClientTransaction, DOImplementingISerializable> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOImplementingISerializable> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.A.EnterDiscardingScope ())
      {
        Assert.AreEqual (23, data.B.PropertyWithGetterAndSetter);
        Assert.AreEqual ("Start-GetObjectData-Ctor", data.B.MemberHeldAsField);
      }
    }

    [Test]
    public void SerializationConstructorPresentEvenIfBaseDoesntImplementISerializable ()
    {
      Type type = CreateTypeGenerator (typeof (NonSerializableDO)).BuildType();
      Assert.IsNotNull (type.GetConstructor (_declaredInstanceFlags, null, new[] {typeof (SerializationInfo), typeof (StreamingContext)}, null));
    }

    [Test]
    public void ShadowedPropertiesAreSeparatelyOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOHidingVirtualProperties)).BuildType ();
      var instance = (DOHidingVirtualProperties) Activator.CreateInstance (type);
      DOWithVirtualProperties instanceAsBase = instance;

      instance.PropertyWithGetterAndSetter = 1;
      instanceAsBase.PropertyWithGetterAndSetter = 2;
      
      Assert.AreEqual (1, instance.PropertyWithGetterAndSetter);
      Assert.AreEqual (2, instanceAsBase.PropertyWithGetterAndSetter);
    }

    [Test]
    public void RealEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithRealRelationEndPoint)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithRealRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags));

      var instance = (DOWithRealRelationEndPoint) Activator.CreateInstance (type);
      var relatedObject = (DOWithVirtualRelationEndPoint) RepositoryAccessor.NewObject (typeof (DOWithVirtualRelationEndPoint), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.AreSame (relatedObject, instance.RelatedObject);
    }

    [Test]
    public void VirtualEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualRelationEndPoint)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithVirtualRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags));

      var instance = (DOWithVirtualRelationEndPoint) Activator.CreateInstance (type);
      var relatedObject = (DOWithRealRelationEndPoint) RepositoryAccessor.NewObject (typeof (DOWithRealRelationEndPoint), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.AreSame (relatedObject, instance.RelatedObject);
    }

    [Test]
    public void UnidirectionalEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithUnidirectionalRelationEndPoint)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOWithUnidirectionalRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags));

      var instance = (DOWithUnidirectionalRelationEndPoint) Activator.CreateInstance (type);
      var relatedObject = (DOWithVirtualProperties) RepositoryAccessor.NewObject (typeof (DOWithVirtualProperties), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.AreSame (relatedObject, instance.RelatedObject);
    }

    [Test]
    public void PropertyAccessorsIndirectlySealedAreNotOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithIndirectlySealedPropertyAccessors)).BuildType ();
      Assert.IsNull (type.GetMethod (typeof (DOWithIndirectlySealedPropertyAccessors).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
    }

    [Test]
    public void SpikeAssertingBaseDefinitionFitsMethodOnBaseType ()
    {
      MethodInfo sealedMethod = typeof (DOWithIndirectlySealedPropertyAccessors).GetMethod ("get_PropertyWithGetterAndSetter", _declaredInstanceFlags);
      MethodInfo unsealedMethod = typeof (DOWithVirtualProperties).GetMethod ("get_PropertyWithGetterAndSetter", _declaredInstanceFlags);

      Assert.IsNotNull (sealedMethod);
      Assert.IsNotNull (unsealedMethod);

      Assert.AreNotEqual (sealedMethod, unsealedMethod);
      Assert.AreEqual (sealedMethod.GetBaseDefinition(), unsealedMethod);
      Assert.AreEqual (sealedMethod.GetBaseDefinition (), unsealedMethod.GetBaseDefinition());
    }

    [Test]
    public void AbstractPropertyAccessorsIndirectlyImplementedAreOverriddenNotImplemented ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingAbstractPropertyAccessors)).BuildType ();
      Assert.IsNotNull (type.GetMethod (typeof (DOImplementingAbstractPropertyAccessors).FullName + ".get_PropertyWithGetterAndSetter",
          _declaredInstanceFlags));
      var instance = (DOImplementingAbstractPropertyAccessors) Activator.CreateInstance (type);
      
      // assert that getter and setter are correctly propagated to base implementation
      instance.PropertyWithGetterAndSetter = 3;
      Assert.AreEqual (10, instance.PropertyWithGetterAndSetter);
    }

    [Test]
    public void DifferentPublicAndBaseType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties), typeof (DerivedDO)).BuildType();
      Assert.IsTrue (typeof (DerivedDO).IsAssignableFrom (type));

      var instance = (DerivedDO) Activator.CreateInstance (type);
      Assert.AreEqual (typeof (DOWithVirtualProperties), instance.GetPublicDomainObjectType ());
    }

    [Test]
    public void PublicTypeIsUsedForOverrideAnalysis ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties), typeof (DerivedDO)).BuildType ();
      Assert.IsNull (type.GetMethod (typeof (DerivedDO).FullName + ".get_VirtualPropertyOnDerivedClass", _declaredInstanceFlags));
    }

    [Test]
    public void DoesNotImplementAbstractStorageClassNonePropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingAbstractStorageClassNoneProperties)).BuildType ();
      Assert.IsNull (type.GetMethod (typeof (DOImplementingAbstractStorageClassNoneProperties).FullName + ".get_PropertyImplementingGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOImplementingAbstractStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithAbstractStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithAbstractStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));
    }

    [Test]
    public void DoesNotOverrideVirtualStorageClassNonePropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualStorageClassNoneProperties)).BuildType ();
      
      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));

      type = CreateTypeGenerator (typeof (DOOverridingVirtualStorageClassNoneProperties)).BuildType ();
      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOOverridingVirtualStorageClassNoneProperties).FullName + ".get_PropertyOverridingGetterAndSetter", _declaredInstanceFlags));
      Assert.IsNull (type.GetMethod (typeof (DOOverridingVirtualStorageClassNoneProperties).FullName + ".set_PropertyOverridingGetterAndSetter", _declaredInstanceFlags));
    }

    [Test]
    public void Overrides_PerformConstructorCheck ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualStorageClassNoneProperties)).BuildType ();
      Assert.That (type.GetMethod ("PerformConstructorCheck", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void Overrides_PerformConstructorCheck_WithNoOp ()
    {
      var instance = 
          (DOWithVirtualStorageClassNoneProperties) RepositoryAccessor.NewObject (typeof (DOWithVirtualStorageClassNoneProperties), ParamList.Empty);
      PrivateInvoke.InvokeNonPublicMethod (instance, "PerformConstructorCheck");
    }
  }
}
