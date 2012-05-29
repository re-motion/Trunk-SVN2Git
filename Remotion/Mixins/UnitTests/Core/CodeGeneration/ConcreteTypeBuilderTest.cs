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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Rhino.Mocks;
using ClassOverridingSingleMixinMethod = Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain.ClassOverridingSingleMixinMethod;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ConcreteTypeBuilderTest : CodeGenerationBaseTest
  {
    private IModuleManager _moduleManagerMockForLoading;

    private ConcreteTypeBuilder _builder;
    private ConcreteTypeBuilder _builderWithModuleManagerMock;
    
    public override void SetUp ()
    {
      base.SetUp();

      _moduleManagerMockForLoading = new MockRepository().StrictMock<IModuleManager>();
      _moduleManagerMockForLoading.Expect (mock => mock.SignedAssemblyName).Return ("FooS");
      _moduleManagerMockForLoading.Expect (mock => mock.UnsignedAssemblyName).Return ("FooU");
      // expecting _no_ other actions on the scope when loading and accessing types from saved module

      _moduleManagerMockForLoading.Replay();

      _builder = SavedTypeBuilder; // use a shared builder to save resources
      _builderWithModuleManagerMock = new ConcreteTypeBuilder (_moduleManagerMockForLoading, new GuidNameProvider (), new GuidNameProvider ());
    }

    [Test]
    public void GetConcreteType_TypesAreCached ()
    {
      Type t1 = _builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType1)));
      Type t2 = _builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType1)));
      Assert.That (t2, Is.SameAs (t1));
    }
    
    [Test]
    public void GetConcreteType_CacheEvenWorksForSerialization ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var bt2 = ObjectFactory.Create<BaseType1> (ParamList.Empty);

      Assert.That (bt2.GetType(), Is.SameAs (bt1.GetType()));

      BaseType1[] array = Serializer.SerializeAndDeserialize (new[] { bt1, bt2 });
      Assert.That (array[0].GetType(), Is.SameAs (bt1.GetType()));
      Assert.That (array[1].GetType(), Is.SameAs (array[0].GetType()));
    }

    [Test]
    public void GetConstructorLookupInfo ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType1));
      var info1 = _builder.GetConstructorLookupInfo (classContext, true);
      var info2 = _builder.GetConstructorLookupInfo (classContext, true);

      Assert.That (info1, Is.SameAs (info2));

      var instance = ParamList.Empty.InvokeConstructor (info1);

      var expectedType = _builder.GetConcreteType (classContext);
      Assert.That (instance, Is.TypeOf (expectedType));
    }

    [Test]
    public void GetConcreteMixinType_GeneratedMixinTypesAreCached ()
    {
      var requestingClass = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var concreteMixinTypeIdentifier = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();
      var t1 = _builder.GetConcreteMixinType (concreteMixinTypeIdentifier);
      var t2 = _builder.GetConcreteMixinType (concreteMixinTypeIdentifier);

      Assert.That (t1, Is.SameAs (t2));
    }

    [Test]
    public void GetConcreteMixinType_MixinTypeCacheEvenWorksForSerialization ()
    {
      var c1 = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);
      var c2 = ObjectFactory.Create<ClassOverridingMixinMembers> (ParamList.Empty);

      Assert.That (Mixin.Get<MixinWithAbstractMembers> (c2).GetType(), Is.SameAs (Mixin.Get<MixinWithAbstractMembers> (c1).GetType()));

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new[] { c1, c2 });
      Assert.That (Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType(), Is.SameAs (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType()));
    }

    [Test]
    public void Current_IsGlobalSingleton ()
    {
      ConcreteTypeBuilder.SetCurrent (null);

      var newBuilder = new ConcreteTypeBuilder (new ModuleManager(), new GuidNameProvider(), new GuidNameProvider());
      Assert.That (ConcreteTypeBuilder.HasCurrent, Is.False);
      var setterThread = new Thread (() => ConcreteTypeBuilder.SetCurrent (newBuilder));
      setterThread.Start();
      setterThread.Join();

      Assert.That (ConcreteTypeBuilder.HasCurrent, Is.True);
      Assert.That (ConcreteTypeBuilder.Current, Is.SameAs (newBuilder));
    }

    [Test]
    public void LockAndAccessScope()
    {
      IModuleManager scope = _builder.Scope;
      _builder.LockAndAccessScope (lockedScope => Assert.That (lockedScope, Is.SameAs (scope)));
    }

    [Test]
    public void DefaultNameProviderIsGuid ()
    {
      Assert.That (_builder.TypeNameProvider, Is.TypeOf<GuidNameProvider>());
      Assert.That (_builder.MixinTypeNameProvider, Is.TypeOf<GuidNameProvider>());
    }

    [Test]
    public void SaveAndResetDynamicScope ()
    {
      var repository = new MockRepository();
      var managerMock = repository.StrictMock<IModuleManager>();

      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder (managerMock);

      var paths = new[] { "Foos", "Bars", "Stripes" };

      using (repository.Ordered())
      {
        managerMock.Expect (mock => mock.HasSignedAssembly).Return (false);
        managerMock.Expect (mock => mock.HasUnsignedAssembly).Return (true);
        managerMock.Expect (mock => mock.SaveAssemblies()).Return (paths);
        managerMock.Expect (mock => mock.Reset());
      }

      repository.ReplayAll();

      builder.SaveGeneratedConcreteTypes();

      repository.VerifyAll();
    }

    [Test]
    public void SaveAndResetDynamicScope_HandlesNoGeneratedTypesGracefully ()
    {
      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder();
      string[] paths = builder.SaveGeneratedConcreteTypes();
      Assert.That (paths.Length, Is.EqualTo (0));
    }

    [Test]
    public void SaveAndResetDynamicScope_ResetsScopeWhenSaving ()
    {
      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder();
      var signedAssemblyNameBefore = builder.Scope.SignedAssemblyName;
      var unsignedAssemblyNameBefore = builder.Scope.UnsignedAssemblyName;

      builder.SaveGeneratedConcreteTypes();

      Assert.That (builder.Scope.HasAssemblies, Is.False);
      Assert.That (builder.Scope.SignedAssemblyName, Is.Not.EqualTo (signedAssemblyNameBefore));
      Assert.That (builder.Scope.UnsignedAssemblyName, Is.Not.EqualTo (unsignedAssemblyNameBefore));
    }

    [Test]
    public void SaveAndResetDynamicScope_CanContinueToGenerateTypesAfterSaving ()
    {
      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder();
      var bt1Context = ClassContextObjectMother.Create(typeof (BaseType1));
      var bt2Context = ClassContextObjectMother.Create(typeof (BaseType2));

      Assert.That (builder.GetConcreteType (bt1Context), Is.Not.Null);
      Assert.That (builder.GetConcreteType (bt2Context), Is.Not.Null);
      builder.SaveGeneratedConcreteTypes();
      var bt3Context = ClassContextObjectMother.Create(typeof (BaseType3));
      Assert.That (builder.GetConcreteType (bt3Context), Is.Not.Null);
    }

    [Test]
    public void SaveAndResetDynamicScope_IntegrationWithCaching ()
    {
      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder();

      Type concreteType1 = builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType1)));
      string[] paths = builder.SaveGeneratedConcreteTypes();
      Assert.That (paths, Is.Not.Empty);

      Type concreteType2 = builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType2)));
      paths = builder.SaveGeneratedConcreteTypes();
      Assert.That (paths, Is.Not.Empty);

      Type concreteType3 = builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType3)));
      Assert.That (builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType1))), Is.SameAs (concreteType1));
      Assert.That (builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType2))), Is.SameAs (concreteType2));
      Assert.That (builder.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType3))), Is.SameAs (concreteType3));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Signed.*' into the cache "
                                                                      +
                                                                      "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
                                                                      +
                                                                      "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly"
        , MatchType = MessageMatch.Regex)]
    public void LoadAssemblyIntoCache_ThrowsWhenLoadedAssemblyHasSameName_AsSigned ()
    {
      var moduleManager = (ModuleManager) _builder.Scope;
      moduleManager.Scope.ObtainDynamicModule (true);
      _builder.LoadConcreteTypes (moduleManager.Scope.StrongNamedModule.Assembly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Unsigned.*' into the cache "
                                                                      +
                                                                      "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
                                                                      +
                                                                      "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly"
        , MatchType = MessageMatch.Regex)]
    public void LoadAssemblyIntoCache_ThrowsWhenLoadedAssemblyHasSameName_AsUnsigned ()
    {
      var moduleManager = (ModuleManager) _builder.Scope;
      moduleManager.Scope.ObtainDynamicModule (false);
      _builder.LoadConcreteTypes (moduleManager.Scope.WeakNamedModule.Assembly);
    }

    [Test]
    public void LoadAssemblyIntoCache_AddsLoadedBaseTypesToTheCache ()
    {
      var loadedType = typeof (LoadableConcreteMixedTypeForBaseType1);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);

      _builderWithModuleManagerMock.LoadConcreteTypes (assemblyMock);

      var concreteType = _builderWithModuleManagerMock.GetConcreteType (GetClassContextForLoadedType (loadedType));
      Assert.That (concreteType, Is.SameAs (loadedType));

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void LoadAssemblyIntoCache_DoesntReplaceTypes ()
    {
      var loadedType = typeof (LoadableConcreteMixedTypeForBaseType1);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);

      var classContext = GetClassContextForLoadedType (loadedType);

      Type concreteType1 = _builder.GetConcreteType (classContext);
      _builder.LoadConcreteTypes (assemblyMock);
      Type concreteType2 = _builder.GetConcreteType (classContext);

      Assert.That (concreteType2, Is.SameAs (concreteType1));
      Assert.That (concreteType2, Is.Not.SameAs (loadedType));
    }

    [Test]
    public void LoadAssemblyIntoCache_AddsLoadedMixinTypesToTheCache ()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType, typeof (LoadableConcreteMixedTypeForClassOverridingMixinMembers));

      _builderWithModuleManagerMock.LoadConcreteTypes (assemblyMock);

      var requestingClass = GetClassContextForLoadedType (typeof (LoadableConcreteMixedTypeForClassOverridingMixinMembers));
      var concreteMixinIdentifier = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();

      Type concreteType = _builderWithModuleManagerMock.GetConcreteMixinType (concreteMixinIdentifier).GeneratedType;
      Assert.That (concreteType, Is.SameAs (loadedType));

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void LoadAssemblyIntoCache_DoesntReplaceMixinTypes ()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);

      var requestingClass = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var concreteMixinIdentifier = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();

      Type concreteType1 = _builder.GetConcreteMixinType (concreteMixinIdentifier).GeneratedType;
      _builder.LoadConcreteTypes (assemblyMock);
      Type concreteType2 = _builder.GetConcreteMixinType (concreteMixinIdentifier).GeneratedType;

      Assert.That (concreteType2, Is.SameAs (concreteType1));
      Assert.That (concreteType2, Is.Not.SameAs (loadedType));
    }

    [Test]
    public void LoadAssemblyIntoCache_StillAllowsGeneration ()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);
      var typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator>();
      typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));

      _builderWithModuleManagerMock.LoadConcreteTypes (assemblyMock);
      _moduleManagerMockForLoading.BackToRecord();
      _moduleManagerMockForLoading
          .Expect (
              mock => mock.CreateTypeGenerator (
                  Arg<TargetClassDefinition>.Is.Anything,
                  Arg<IConcreteMixedTypeNameProvider>.Is.Anything,
                  Arg.Is (_builderWithModuleManagerMock)))
          .Return (typeGeneratorMock);
      _moduleManagerMockForLoading.Replay();

      var nonLoadedType = _builderWithModuleManagerMock.GetConcreteType (ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1)));
      Assert.That (nonLoadedType, Is.SameAs (typeof (string)));

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void GetConcreteMixinType_BeforeGetConcreteTypeWorks ()
    {
      var requestingClass = ClassContextObjectMother.Create(typeof (ClassOverridingSingleMixinMethod), typeof (MixinWithOverridableMember));
      var concreteMixinIdentifier = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();

      Type t = _builder.GetConcreteMixinType (concreteMixinIdentifier).GeneratedType;

      Assert.That (t, Is.Not.Null);
      Assert.That (typeof (MixinWithOverridableMember).IsAssignableFrom (t), Is.True);

      var concreteMixedType = _builder.GetConcreteType (requestingClass);
      var instance = Activator.CreateInstance (concreteMixedType);
      Assert.That (Mixin.Get<MixinWithOverridableMember> (instance).GetType(), Is.SameAs (t));
    }

    [Test]
    public void InitializeUnconstructedInstance_DelegatesToScope ()
    {
      var mockRepository = new MockRepository();
      var mockMixinTarget = mockRepository.StrictMock<IMixinTarget>();
      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder (mockScope);

      mockScope.Expect (mock => mock.InitializeMixinTarget (mockMixinTarget));

      mockRepository.ReplayAll();

      builder.InitializeUnconstructedInstance (mockMixinTarget);

      mockRepository.VerifyAll();
    }

    [Test]
    public void BeginDeserialization_DelegatesToScope ()
    {
      var mockRepository = new MockRepository();

      Type deserializedType = typeof (object);
      var objectReference = mockRepository.StrictMock<IObjectReference>();
      var info = new SerializationInfo (deserializedType, new FormatterConverter());
      var context = new StreamingContext();

      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder (mockScope);

      Func<Type, Type> transformer = delegate { return null; };

      mockScope.Expect (mock => mock.BeginDeserialization (transformer, info, context)).Return (objectReference);

      mockRepository.ReplayAll();

      Assert.That (builder.BeginDeserialization (transformer, info, context), Is.SameAs (objectReference));

      mockRepository.VerifyAll();
    }

    [Test]
    public void FinishDeserialization_DelegatesToScope ()
    {
      var mockRepository = new MockRepository();

      var objectReference = mockRepository.StrictMock<IObjectReference>();
      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = ConcreteTypeBuilderObjectMother.CreateConcreteTypeBuilder (mockScope);

      mockScope.Expect (mock => mock.FinishDeserialization (objectReference));

      mockRepository.ReplayAll();

      builder.FinishDeserialization (objectReference);

      mockRepository.VerifyAll();
    }

    [Test]
    public void Scope ()
    {
      var fakeScope = MockRepository.GenerateStub<IModuleManager>();
      var builder = new ConcreteTypeBuilder (fakeScope, new GuidNameProvider (), new GuidNameProvider ());

      Assert.That (builder.Scope, Is.SameAs (fakeScope));
    }

    [Test]
    public void Scope_NewEstablished_AfterConcreteTypeBuilderSetNull ()
    {
      var oldModuleManager = ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current);
      Assert.That (oldModuleManager != null);

      ConcreteTypeBuilder.SetCurrent (null);
      Assert.That (ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current), Is.Not.Null);
      Assert.That (ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current), Is.Not.SameAs (oldModuleManager));
    }

    [Test]
    public void Scope_NewScopeHasNewAssemblyNames_AfterConcreteTypeBuilderSetNull ()
    {
      string oldSignedName = ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current).SignedAssemblyName;
      string oldUnsignedName = ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current).UnsignedAssemblyName;

      ConcreteTypeBuilder.SetCurrent (null);
      Assert.That (ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current).SignedAssemblyName, Is.Not.EqualTo (oldSignedName));
      Assert.That (
          ConcreteTypeBuilderTestHelper.GetIModuleManager (ConcreteTypeBuilder.Current).UnsignedAssemblyName, Is.Not.EqualTo (oldUnsignedName));
    }

    [Test]
    public void NameProvider_IsUsedWhenTypeIsGenerated ()
    {
      var mixedTypeNameProvider = MockRepository.GenerateMock<IConcreteMixedTypeNameProvider>();
      var mixinTypeNameProvider = MockRepository.GenerateMock<IConcreteMixinTypeNameProvider>();
      var moduleManagerMock = MockRepository.GenerateMock<IModuleManager>();
      var typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator>();
      var builder = new ConcreteTypeBuilder (moduleManagerMock, mixedTypeNameProvider, mixinTypeNameProvider);

      moduleManagerMock
          .Expect (
              mock => mock.CreateTypeGenerator (
                  Arg<TargetClassDefinition>.Is.Anything,
                  Arg.Is (mixedTypeNameProvider),
                  Arg.Is (builder)))
          .Return (typeGeneratorMock);
      moduleManagerMock.Replay();

      typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));
      typeGeneratorMock.Replay();

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));
      builder.GetConcreteType (classContext);
      moduleManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void DefaultInstance_ViaDefaultServiceLocator ()
    {
      var instance = new DefaultServiceLocator().GetInstance<IConcreteTypeBuilder>();

      Assert.That (instance, Is.TypeOf<ConcreteTypeBuilder> ());
      Assert.That (((ConcreteTypeBuilder) instance).Scope, Is.TypeOf<ModuleManager> ());
      Assert.That (((ConcreteTypeBuilder) instance).TypeNameProvider, Is.TypeOf<GuidNameProvider> ());
      Assert.That (((ConcreteTypeBuilder) instance).MixinTypeNameProvider, Is.TypeOf<GuidNameProvider> ());
    }

    private _Assembly SetupAssemblyMockForLoading (params Type[] loadedTypes)
    {
      var assemblyMock = MockRepository.GenerateMock<_Assembly>();
      assemblyMock.Expect (mock => mock.GetName()).Return (new AssemblyName ("TestAssembly"));
      assemblyMock.Expect (mock => mock.GetExportedTypes()).Return (loadedTypes);
      assemblyMock.Replay();
      return assemblyMock;
    }

    private ClassContext GetClassContextForLoadedType (Type loadedType)
    {
      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (loadedType, false);
      return attribute.GetClassContext();
    }
  }
}