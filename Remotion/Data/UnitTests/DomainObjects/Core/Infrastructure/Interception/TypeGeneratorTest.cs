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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.ObjectBinding;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception
{
  [TestFixture]
  public class TypeGeneratorTest : ClientTransactionBaseTest
  {
    private const BindingFlags _declaredInstanceFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      SetupAssemblyDirectory ();

      // setup mixin builder to generate files into the same directory
      ConcreteTypeBuilder.SetCurrent (null); // reinitialize ConcreteTypeBuilder
      
      var scope = ((ConcreteTypeBuilder) ConcreteTypeBuilder.Current).ModuleInfo;
      scope.SignedModulePath = Path.Combine (AssemblyDirectory, scope.SignedAssemblyName + ".dll");
      scope.UnsignedModulePath = Path.Combine (AssemblyDirectory, scope.UnsignedAssemblyName + ".dll");
    }

    private string AssemblyDirectory
    {
      get { return SetUpFixture.AssemblyDirectory; }
    }

    private InterceptedDomainObjectTypeFactory Factory
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

      Module coreAssemblyModule = typeof (ArgumentUtility).Assembly.ManifestModule;
      File.Copy (coreAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, coreAssemblyModule.Name));

      Module mixinAssemblyModule = typeof (Mixin<>).Assembly.ManifestModule;
      File.Copy (mixinAssemblyModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, mixinAssemblyModule.Name));

      Module objectBindingModule = typeof (BindableObjectBase).Assembly.ManifestModule;
      File.Copy (objectBindingModule.FullyQualifiedName, Path.Combine (AssemblyDirectory, objectBindingModule.Name));
    }

    public override void TestFixtureTearDown ()
    {
      string[] paths = Factory.SaveGeneratedAssemblies ();
      // save mixins as well, we need those files if the intercepted types depend on mixed types
      ConcreteTypeBuilder.Current.SaveGeneratedConcreteTypes ();

#if !NO_PEVERIFY
      foreach (string path in paths)
        PEVerifier.CreateDefault ().VerifyPEFile (path);
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
      Assert.That (type.Name, Is.Not.EqualTo (typeof (DOWithVirtualProperties).Name));
      Assert.That (type.Name, Is.Not.EqualTo (typeof (DOWithVirtualProperties).FullName));
    }

    [Test]
    public void EachGeneratedTypeHasDifferentName ()
    {
      Type type1 = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Type type2 = new InterceptedDomainObjectTypeFactory(Environment.CurrentDirectory, TypeConversionProvider.Create ()).GetConcreteDomainObjectType (typeof (DOWithVirtualProperties));
      Assert.That (type2, Is.Not.SameAs (type1));
      Assert.That (type2.Name, Is.Not.EqualTo (type1.Name));
    }

    [Test]
    public void ReplicatesConstructors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      Assert.That (type.GetConstructor (new[] { typeof (string), typeof (string) }), Is.Not.Null);
      Assert.That (type.GetConstructor (new[] { typeof (int) }), Is.Not.Null);
    }

    [Test]
    public void ReplicatedConstructorsDelegateToBase ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithConstructors)).BuildType ();
      var instance1 = (DOWithConstructors) CreateInstanceOfGeneratedType (type, "Foo", "Bar");
      Assert.That (instance1.FirstArg, Is.EqualTo ("Foo"));
      Assert.That (instance1.SecondArg, Is.EqualTo ("Bar"));

      var instance2 = (DOWithConstructors) CreateInstanceOfGeneratedType (type, 7);
      Assert.That (instance2.FirstArg, Is.EqualTo ("7"));
      Assert.That (instance2.SecondArg, Is.Null);
    }

    [Test]
    public void OverridesGetPublicDomainObjectType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.That (type.GetMethod ("GetPublicDomainObjectTypeImplementation", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void OverridesGetPublicDomainObjectTypeToReturnBaseType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);
      Assert.That (instance.GetPublicDomainObjectType (), Is.EqualTo (typeof (DOWithVirtualProperties)));
      Assert.That (type.GetMethod ("GetPublicDomainObjectTypeImplementation", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void DosNotOverrideVirtualProperties ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      Assert.That (type.GetProperty ("PropertyWithGetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("PropertyWithSetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("ProtectedProperty", _declaredInstanceFlags), Is.Null);
    }

    [Test]
    public void OverridesVirtualPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Null);

      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void OverriddenVirtualPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithVirtualProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
    }

    [Test]
    public void OverridesVirtualPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (0));
      instance.PropertyWithGetterAndSetter = 17;
      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (17));

      Assert.That (instance.PropertyWithGetterOnly, Is.Null);
      instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.That (instance.PropertyWithGetterOnly, Is.EqualTo ("hear, hear"));

      Assert.That (instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime>(), Is.EqualTo (new DateTime()));
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.That (instance.Properties[typeof (DOWithVirtualProperties), "PropertyWithSetterOnly"].GetValue<DateTime> (), Is.EqualTo (new DateTime (2260, 1, 2)));

      typeof (DOWithVirtualProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 67, _declaredInstanceFlags, null, null, null);
      Assert.That (typeof (DOWithVirtualProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
                                                   .GetValue (instance, _declaredInstanceFlags, null, null, null), Is.EqualTo (67));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (0));
      instance.GetAndCheckCurrentPropertyName();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void OverriddenPropertiesCleanUpCurrentPropertyNameEvenOnExceptionInGetter ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);

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
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);

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
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);
      Dev.Null = instance.PropertyNotInMapping;
    }

    [Test]
    public void IgnoresMixedProperties ()
    {
      Type mixedBaseType = TypeFactory.GetConcreteType (typeof (TargetClassForPersistentMixin));
      // save mixin scope to enable peverifying the intercepted type
      Assert.That (MixinTypeUtility.HasMixin (mixedBaseType, typeof (MixinAddingPersistentProperties)), Is.True);
      Type type = CreateTypeGenerator (typeof (TargetClassForPersistentMixin), mixedBaseType).BuildType ();
      Assert.That (type.GetProperties (_declaredInstanceFlags), Is.Empty);
    }

    [Test]
    public void DosNotImplementAbstractProperties ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.That (type.GetProperty ("PropertyWithGetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("PropertyWithSetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetProperty ("ProtectedProperty", _declaredInstanceFlags), Is.Null);
    }

    [Test]
    public void ImplementsAbstractPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Null);

      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void ImplementsAbstractPropertyAccessors_CollectionSetters ()
    {
      Type type = CreateTypeGenerator (typeof (ClassWithAbstractRelatedCollectionSetter)).BuildType ();
      Assert.That (type.GetMethod (typeof (ClassWithAbstractRelatedCollectionSetter).FullName + ".get_RelatedObjects", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (ClassWithAbstractRelatedCollectionSetter).FullName + ".set_RelatedObjects", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void ImplementedAbstractPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAbstractProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
    }

    [Test]
    public void ImplementsAbstractPropertiesSoThatCurrentPropertyWorks ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) CreateInstanceOfGeneratedType (type);

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (0));
      instance.PropertyWithGetterAndSetter = 17;
      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (17));

      Assert.That (instance.PropertyWithGetterOnly, Is.Null);
      instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithGetterOnly"].SetValue ("hear, hear");
      Assert.That (instance.PropertyWithGetterOnly, Is.EqualTo ("hear, hear"));

      Assert.That (instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> (), Is.EqualTo (new DateTime ()));
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.That (instance.Properties[typeof (DOWithAbstractProperties), "PropertyWithSetterOnly"].GetValue<DateTime> (), Is.EqualTo (new DateTime (2260, 1, 2)));

      typeof (DOWithAbstractProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 4711, _declaredInstanceFlags, null, null, null);
      Assert.That (typeof (DOWithAbstractProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
                                                    .GetValue (instance, _declaredInstanceFlags, null, null, null), Is.EqualTo (4711));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertyGettersCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) CreateInstanceOfGeneratedType (type);

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (0));
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current property", MatchType = MessageMatch.Contains)]
    public void ImplementedAbstractPropertySettersCleanUpCurrentPropertyName ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAbstractProperties)).BuildType();
      var instance = (DOWithAbstractProperties) CreateInstanceOfGeneratedType (type);

      instance.PropertyWithGetterAndSetter = 17;
      instance.GetAndCheckCurrentPropertyName ();
    }

    [Test]
    public void ImplementsAutomaticPropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterOnly", _declaredInstanceFlags), Is.Null);

      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Not.Null);

      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags), Is.Not.Null);
    }

    [Test]
    public void ImplementedAutomaticPropertyAccessors_KeepVisibility ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithSetterOnly", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags).IsPublic, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".get_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
      Assert.That (type.GetMethod (typeof (DOWithAutomaticProperties).FullName + ".set_ProtectedProperty", _declaredInstanceFlags).IsFamily, Is.True);
    }

    [Test]
    public void ImplementsAutomaticProperties_ViaPropertyIndexer ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithAutomaticProperties)).BuildType ();
      var instance = (DOWithAutomaticProperties) CreateInstanceOfGeneratedType (type);

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (0));
      instance.PropertyWithGetterAndSetter = 17;
      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (17));
      Assert.That (instance.Properties.Find ("PropertyWithGetterAndSetter").GetValueWithoutTypeCheck (), Is.EqualTo (17));
      instance.Properties.Find ("PropertyWithGetterAndSetter").SetValueWithoutTypeCheck (8);
      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (8));

      Assert.That (instance.PropertyWithGetterOnly, Is.Null);
      instance.Properties.Find ("PropertyWithGetterOnly").SetValue ("hear, hear");
      Assert.That (instance.PropertyWithGetterOnly, Is.EqualTo ("hear, hear"));

      Assert.That (instance.Properties.Find ("PropertyWithSetterOnly").GetValue<DateTime> (), Is.EqualTo (new DateTime ()));
      instance.PropertyWithSetterOnly = new DateTime (2260, 1, 2);
      Assert.That (instance.Properties.Find ("PropertyWithSetterOnly").GetValue<DateTime> (), Is.EqualTo (new DateTime (2260, 1, 2)));

      typeof (DOWithAutomaticProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
          .SetValue (instance, 4711, _declaredInstanceFlags, null, null, null);
      Assert.That (instance.Properties.Find ("ProtectedProperty").GetValueWithoutTypeCheck (), Is.EqualTo (4711));
      instance.Properties.Find ("ProtectedProperty").SetValueWithoutTypeCheck (9);
      Assert.That (typeof (DOWithAutomaticProperties).GetProperty ("ProtectedProperty", _declaredInstanceFlags)
                                                     .GetValue (instance, _declaredInstanceFlags, null, null, null), Is.EqualTo (9));
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClassWithProps "
        + "as its member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property).")]
    public void ThrowsOnAbstractPropertyNotInMapping ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClassWithProps)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
      "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClass as its "
      + "member Foo (on type NonInstantiableAbstractClass) is abstract (and not an automatic property).")]
    public void ThrowsOnAbstractMethod ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException),
        ExpectedMessage =
        "Cannot instantiate type Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableSealedClass as it is sealed.")]
    public void ThrowsOnSealedBaseType ()
    {
      CreateTypeGenerator (typeof (NonInstantiableSealedClass)).BuildType();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type Remotion.Data.UnitTests."
        + "DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableAbstractClassWithoutAttribute as it is abstract and not instantiable.")]
    public void ThrowsOnAbstractClassWithoutInstantiableAttribute ()
    {
      CreateTypeGenerator (typeof (NonInstantiableAbstractClassWithoutAttribute)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage = "Cannot instantiate type "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.NonInstantiableNonDomainClass as it is not part of the mapping.")]
    public void ThrowsOnClassWithoutClassDefinition ()
    {
      CreateTypeGenerator (typeof (NonInstantiableNonDomainClass)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
        "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.ClassWithNonVirtualAutomaticPropertyGetter' "
        + "as its member 'RelatedObjects' has a non-virtual get accessor.")]
    public void ThrowsOnNonVirtualAutomaticPropertyGetter ()
    {
      CreateTypeGenerator (typeof (ClassWithNonVirtualAutomaticPropertyGetter)).BuildType ();
    }

    [Test]
    [ExpectedException (typeof (NonInterceptableTypeException), ExpectedMessage =
      "Cannot instantiate type 'Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.ClassWithNonVirtualAutomaticPropertySetter' "
      + "as its member 'RelatedObjects' has a non-virtual set accessor.")]
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
      Assert.That (typeof (ISerializable).IsAssignableFrom (type), Is.True);
    }

    [Test]
    public void GeneratedTypeCanBeSerialized ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType ();
      var instance = (DOWithVirtualProperties) CreateInstanceOfGeneratedType (type);
      instance.PropertyWithGetterAndSetter = 17;

      Tuple<ClientTransaction, DOWithVirtualProperties> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOWithVirtualProperties> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.Item1.EnterDiscardingScope ())
      {
        Assert.That (data.Item2.PropertyWithGetterAndSetter, Is.EqualTo (17));
      }
    }

    [Test]
    public void GeneratedTypeCanBeSerializedWhenItImplementsISerializable ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingISerializable)).BuildType ();
      var instance = (DOImplementingISerializable) CreateInstanceOfGeneratedType (type, "Start");
      instance.PropertyWithGetterAndSetter = 23;
      Assert.That (instance.MemberHeldAsField, Is.EqualTo ("Start"));

      Tuple<ClientTransaction, DOImplementingISerializable> data =
          Serializer.SerializeAndDeserialize (
              new Tuple<ClientTransaction, DOImplementingISerializable> (ClientTransactionScope.CurrentTransaction, instance));

      using (data.Item1.EnterDiscardingScope ())
      {
        Assert.That (data.Item2.PropertyWithGetterAndSetter, Is.EqualTo (23));
        Assert.That (data.Item2.MemberHeldAsField, Is.EqualTo ("Start-GetObjectData-Ctor"));
      }
    }

    [Test]
    public void SerializationConstructorPresentEvenIfBaseDoesntImplementISerializable ()
    {
      Type type = CreateTypeGenerator (typeof (NonSerializableDO)).BuildType();
      Assert.That (type.GetConstructor (_declaredInstanceFlags, null, new[] {typeof (SerializationInfo), typeof (StreamingContext)}, null), Is.Not.Null);
    }

    [Test]
    public void ShadowedPropertiesAreSeparatelyOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOHidingVirtualProperties)).BuildType ();
      var instance = (DOHidingVirtualProperties) CreateInstanceOfGeneratedType (type);
      DOWithVirtualProperties instanceAsBase = instance;

      instance.PropertyWithGetterAndSetter = 1;
      instanceAsBase.PropertyWithGetterAndSetter = 2;

      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (1));
      Assert.That (instanceAsBase.PropertyWithGetterAndSetter, Is.EqualTo (2));
    }

    [Test]
    public void RealEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithRealRelationEndPoint)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithRealRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags), Is.Not.Null);

      var instance = (DOWithRealRelationEndPoint) CreateInstanceOfGeneratedType (type);
      var relatedObject = (DOWithVirtualRelationEndPoint) LifetimeService.NewObject (TestableClientTransaction, typeof (DOWithVirtualRelationEndPoint), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.That (instance.RelatedObject, Is.SameAs (relatedObject));
    }

    [Test]
    public void VirtualEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualRelationEndPoint)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithVirtualRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags), Is.Not.Null);

      var instance = (DOWithVirtualRelationEndPoint) CreateInstanceOfGeneratedType (type);
      var relatedObject = (DOWithRealRelationEndPoint) LifetimeService.NewObject (TestableClientTransaction, typeof (DOWithRealRelationEndPoint), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.That (instance.RelatedObject, Is.SameAs (relatedObject));
    }

    [Test]
    public void UnidirectionalEndPointDefinitionsAreOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithUnidirectionalRelationEndPoint)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithUnidirectionalRelationEndPoint).FullName + ".get_RelatedObject", _declaredInstanceFlags), Is.Not.Null);

      var instance = (DOWithUnidirectionalRelationEndPoint) CreateInstanceOfGeneratedType (type);
      var relatedObject = (DOWithVirtualProperties) LifetimeService.NewObject (TestableClientTransaction, typeof (DOWithVirtualProperties), ParamList.Empty);
      instance.RelatedObject = relatedObject;
      Assert.That (instance.RelatedObject, Is.SameAs (relatedObject));
    }

    [Test]
    public void PropertyAccessorsIndirectlySealedAreNotOverridden ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithIndirectlySealedPropertyAccessors)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithIndirectlySealedPropertyAccessors).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
    }

    [Test]
    public void SpikeAssertingBaseDefinitionFitsMethodOnBaseType ()
    {
      MethodInfo sealedMethod = typeof (DOWithIndirectlySealedPropertyAccessors).GetMethod ("get_PropertyWithGetterAndSetter", _declaredInstanceFlags);
      MethodInfo unsealedMethod = typeof (DOWithVirtualProperties).GetMethod ("get_PropertyWithGetterAndSetter", _declaredInstanceFlags);

      Assert.That (sealedMethod, Is.Not.Null);
      Assert.That (unsealedMethod, Is.Not.Null);

      Assert.That (unsealedMethod, Is.Not.EqualTo (sealedMethod));
      Assert.That (unsealedMethod, Is.EqualTo (sealedMethod.GetBaseDefinition()));
      Assert.That (unsealedMethod.GetBaseDefinition(), Is.EqualTo (sealedMethod.GetBaseDefinition ()));
    }

    [Test]
    public void AbstractPropertyAccessorsIndirectlyImplementedAreOverriddenNotImplemented ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingAbstractPropertyAccessors)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOImplementingAbstractPropertyAccessors).FullName + ".get_PropertyWithGetterAndSetter",
                                   _declaredInstanceFlags), Is.Not.Null);
      var instance = (DOImplementingAbstractPropertyAccessors) CreateInstanceOfGeneratedType (type);
      
      // assert that getter and setter are correctly propagated to base implementation
      instance.PropertyWithGetterAndSetter = 3;
      Assert.That (instance.PropertyWithGetterAndSetter, Is.EqualTo (10));
    }

    [Test]
    public void DifferentPublicAndBaseType ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties), typeof (DerivedDO)).BuildType();
      Assert.That (typeof (DerivedDO).IsAssignableFrom (type), Is.True);

      var instance = (DerivedDO) CreateInstanceOfGeneratedType (type);
      Assert.That (instance.GetPublicDomainObjectType (), Is.EqualTo (typeof (DOWithVirtualProperties)));
    }

    [Test]
    public void PublicTypeIsUsedForOverrideAnalysis ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualProperties), typeof (DerivedDO)).BuildType ();
      Assert.That (type.GetMethod (typeof (DerivedDO).FullName + ".get_VirtualPropertyOnDerivedClass", _declaredInstanceFlags), Is.Null);
    }

    [Test]
    public void DoesNotImplementAbstractStorageClassNonePropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOImplementingAbstractStorageClassNoneProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOImplementingAbstractStorageClassNoneProperties).FullName + ".get_PropertyImplementingGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOImplementingAbstractStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithAbstractStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
    }

    [Test]
    public void DoesNotOverrideVirtualStorageClassNonePropertyAccessors ()
    {
      Type type = CreateTypeGenerator (typeof (DOWithVirtualStorageClassNoneProperties)).BuildType ();

      Assert.That (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);

      type = CreateTypeGenerator (typeof (DOOverridingVirtualStorageClassNoneProperties)).BuildType ();
      Assert.That (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".get_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOWithVirtualStorageClassNoneProperties).FullName + ".set_PropertyWithGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOOverridingVirtualStorageClassNoneProperties).FullName + ".get_PropertyOverridingGetterAndSetter", _declaredInstanceFlags), Is.Null);
      Assert.That (type.GetMethod (typeof (DOOverridingVirtualStorageClassNoneProperties).FullName + ".set_PropertyOverridingGetterAndSetter", _declaredInstanceFlags), Is.Null);
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
          (DOWithVirtualStorageClassNoneProperties) LifetimeService.NewObject (TestableClientTransaction, typeof (DOWithVirtualStorageClassNoneProperties), ParamList.Empty);
      PrivateInvoke.InvokeNonPublicMethod (instance, "PerformConstructorCheck");
    }

    [Test]
    public void InterceptedPropertyCollectorIgnoresPropertyInformationObjectsWhichCannotBeConvertedToPropertyInfo ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (DOWithVirtualProperties));
      var propertyDefinitionCollection = new PropertyDefinitionCollection();
      propertyDefinitionCollection.Add (
          new PropertyDefinition (
              classDefinition,
              MockRepository.GenerateStub<IPropertyInformation>(),
              "CustomProperty", 
              false,
              false,
              null,
              StorageClass.Transaction));
      classDefinition.SetPropertyDefinitions (propertyDefinitionCollection);
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      classDefinition.SetDerivedClasses (Enumerable.Empty<ClassDefinition>());
      var mappingLoaderStub = CreatekMappingLoaderStub (new[] { classDefinition });
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration (mappingLoaderStub, persistenceModelLoaderStub);
      MappingConfiguration.SetCurrent (configuration);

      CreateTypeGenerator (typeof (DOWithVirtualProperties)).BuildType();
    }

    private IMappingLoader CreatekMappingLoaderStub (ClassDefinition[] classDefinitions)
    {
      var mappingLoaderStub = MockRepository.GenerateStub<IMappingLoader>();
      mappingLoaderStub.Stub (stub => stub.GetClassDefinitions()).Return (classDefinitions);
      mappingLoaderStub.Stub (stub => stub.GetRelationDefinitions (null)).IgnoreArguments().Return (new RelationDefinition[0]);
      mappingLoaderStub.Stub (stub => stub.ResolveTypes).Return (true);
      mappingLoaderStub.Stub (stub => stub.NameResolver).Return (new ReflectionBasedNameResolver());
      mappingLoaderStub.Stub (stub => stub.CreateClassDefinitionValidator()).Return (new ClassDefinitionValidator());
      mappingLoaderStub.Stub (stub => stub.CreatePropertyDefinitionValidator()).Return (new PropertyDefinitionValidator());
      mappingLoaderStub.Stub (stub => stub.CreateRelationDefinitionValidator()).Return (new RelationDefinitionValidator());
      mappingLoaderStub.Stub (stub => stub.CreateSortExpressionValidator()).Return (new SortExpressionValidator());
      return mappingLoaderStub;
    }

    private IPersistenceModelLoader CreatePersistenceModelLoaderStub ()
    {
      var persistenceModelLoaderStub = MockRepository.GenerateStub<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Stub (stub => stub.ApplyPersistenceModelToHierarchy (Arg<ClassDefinition>.Is.Anything))
          .WhenCalled (
              mi => ((ClassDefinition) mi.Arguments[0]).SetStorageEntity (TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition)));
      persistenceModelLoaderStub
          .Stub (stub => stub.CreatePersistenceMappingValidator (Arg<ClassDefinition>.Is.Anything))
          .Return (new PersistenceMappingValidator());
      return persistenceModelLoaderStub;
    }

    private object CreateInstanceOfGeneratedType (Type type, params object[] args)
    {
      return ObjectInititalizationContextScopeHelper.CallWithNewObjectInitializationContext (
          TestableClientTransaction, 
          new ObjectID(type.BaseType, Guid.NewGuid()),
          () => Activator.CreateInstance (type, args));
    }
  }
}
