// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Mixins.Context;
using ClassOverridingSingleMixinMethod=Remotion.UnitTests.Mixins.CodeGeneration.TestDomain.ClassOverridingSingleMixinMethod;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteTypeBuilderTest : CodeGenerationBaseTest
  {
    private IModuleManager _moduleManagerMockForLoading;

    public override void SetUp()
    {
      base.SetUp();

      _moduleManagerMockForLoading = new MockRepository().StrictMock<IModuleManager>();
      _moduleManagerMockForLoading.Expect (mock => mock.SignedAssemblyName).Return ("FooS");
      _moduleManagerMockForLoading.Expect (mock => mock.UnsignedAssemblyName).Return ("FooU");
      // expecting _no_ other actions on the scope when loading and accessing types from saved module

      _moduleManagerMockForLoading.Replay();
    }

    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (t2, Is.SameAs (t1));
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (AlternativeTypeBuilder);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (t2, Is.Not.SameAs (t1));
    }

    [Test]
    public void CacheEvenWorksForSerialization()
    {
      var bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      var bt2 = ObjectFactory.Create<BaseType1>(ParamList.Empty);

      Assert.That (bt2.GetType (), Is.SameAs (bt1.GetType ()));

      BaseType1[] array = Serializer.SerializeAndDeserialize (new[] {bt1, bt2});
      Assert.That (array[0].GetType (), Is.SameAs (bt1.GetType ()));
      Assert.That (array[1].GetType (), Is.SameAs (array[0].GetType ()));
    }

    [Test]
    public void GeneratedMixinTypesAreCached()
    {
      var c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      var c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.That (Mixin.Get<MixinWithAbstractMembers> (c2).GetType (), Is.SameAs (Mixin.Get<MixinWithAbstractMembers> (c1).GetType ()));
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder()
    {
      var c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      ConcreteTypeBuilder.SetCurrent (AlternativeTypeBuilder);
      var c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.That (Mixin.Get<MixinWithAbstractMembers> (c2).GetType (), Is.Not.SameAs (Mixin.Get<MixinWithAbstractMembers> (c1).GetType ()));
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization()
    {
      var c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      var c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.That (Mixin.Get<MixinWithAbstractMembers> (c2).GetType (), Is.SameAs (Mixin.Get<MixinWithAbstractMembers> (c1).GetType ()));

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new[] {c1, c2});
      Assert.That (Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType (), Is.SameAs (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType ()));
    }

    [Test]
    public void CurrentIsGlobalSingleton()
    {
      ConcreteTypeBuilder.SetCurrent (null);

      var newBuilder = new ConcreteTypeBuilder();
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
      IModuleManager scope = ConcreteTypeBuilder.Current.Scope;
      ConcreteTypeBuilder.Current.LockAndAccessScope (lockedScope => Assert.That (lockedScope, Is.SameAs (scope)));
    }

    [Test]
    public void DefaultNameProviderIsGuid()
    {
      Assert.That (ConcreteTypeBuilder.Current.TypeNameProvider, Is.SameAs (GuidNameProvider.Instance));
      Assert.That (ConcreteTypeBuilder.Current.MixinTypeNameProvider, Is.SameAs (GuidNameProvider.Instance));
    }

    [Test]
    public void CanSaveAndResetScope()
    {
      var repository = new MockRepository();
      var managerMock = repository.StrictMock<IModuleManager>();

      var builder = new ConcreteTypeBuilder {Scope = managerMock};

      var paths = new[] {"Foos", "Bars", "Stripes"};

      using (repository.Ordered())
      {
        Expect.Call (managerMock.HasSignedAssembly).Return (false);
        Expect.Call (managerMock.HasUnsignedAssembly).Return (true);
        Expect.Call (managerMock.SaveAssemblies()).Return (paths);
      }

      repository.ReplayAll();

      builder.SaveAndResetDynamicScope();

      repository.VerifyAll();
    }

    [Test]
    public void HandlesSaveWithoutGeneratedTypesGracefully()
    {
      var builder = new ConcreteTypeBuilder();
      string[] paths = builder.SaveAndResetDynamicScope();
      Assert.That (paths.Length, Is.EqualTo (0));
    }

    [Test]
    public void ResetsScopeWhenSaving()
    {
      var builder = new ConcreteTypeBuilder();
      IModuleManager scopeBefore = builder.Scope;
      builder.SaveAndResetDynamicScope();
      Assert.That (builder.Scope, Is.Not.SameAs (scopeBefore));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSaving()
    {
      var builder = new ConcreteTypeBuilder();
      var bt1Context = TargetClassDefinitionUtility.GetContext (typeof (BaseType1), MixinConfiguration.ActiveConfiguration, GenerationPolicy.ForceGeneration);
      var bt2Context = TargetClassDefinitionUtility.GetContext (typeof (BaseType2), MixinConfiguration.ActiveConfiguration, GenerationPolicy.ForceGeneration);

      Assert.That (builder.GetConcreteType (bt1Context), Is.Not.Null);
      Assert.That (builder.GetConcreteType (bt2Context), Is.Not.Null);
      builder.SaveAndResetDynamicScope();
      var bt3Context = TargetClassDefinitionUtility.GetContext(typeof (BaseType3), MixinConfiguration.ActiveConfiguration, GenerationPolicy.ForceGeneration);
      Assert.That (builder.GetConcreteType (bt3Context), Is.Not.Null);
    }

    [Test]
    public void SavingGeneratingCachingIntegration()
    {
      ConcreteTypeBuilder.SetCurrent (null);

      Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      Assert.IsNotEmpty (paths);
      Type concreteType2 = TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
      paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      Assert.IsNotEmpty (paths);
      Type concreteType3 = TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration);

      Assert.That (TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration), Is.SameAs (concreteType1));
      Assert.That (TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration), Is.SameAs (concreteType2));
      Assert.That (TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration), Is.SameAs (concreteType3));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Signed.*' into the cache "
                                                                      +
                                                                      "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
                                                                      +
                                                                      "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly"
        , MatchType = MessageMatch.Regex)]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsSigned()
    {
      var moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (true);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.StrongNamedModule.Assembly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Unsigned.*' into the cache "
                                                                      +
                                                                      "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
                                                                      +
                                                                      "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly"
        , MatchType = MessageMatch.Regex)]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsUnsigned()
    {
      var moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (false);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.WeakNamedModule.Assembly);
    }

    [Test]
    public void LoadAddsLoadedBaseTypesToTheCache()
    {
      var loadedType = typeof (LoadableConcreteMixedTypeForBaseType1);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);
      var builder = new ConcreteTypeBuilder {Scope = _moduleManagerMockForLoading};
      ConcreteTypeBuilder.SetCurrent (builder);

      builder.LoadAssemblyIntoCache (assemblyMock);

      using (SetupMixinConfigurationForLoadedType (loadedType).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
        Assert.That (concreteType, Is.SameAs (loadedType));
      }

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void LoadDoesntReplaceTypes()
    {
      var loadedType = typeof (LoadableConcreteMixedTypeForBaseType1);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);

      using (SetupMixinConfigurationForLoadedType (loadedType).EnterScope())
      {
        Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assemblyMock);
        Type concreteType2 = TypeFactory.GetConcreteType (typeof (BaseType1));

        Assert.That (concreteType2, Is.SameAs (concreteType1));
        Assert.That (concreteType2, Is.Not.SameAs (loadedType));
      }
    }

    [Test]
    public void LoadAddsLoadedMixinTypesToTheCache()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType, typeof (LoadableConcreteMixedTypeForClassOverridingMixinMembers));
      var builder = new ConcreteTypeBuilder {Scope = _moduleManagerMockForLoading};
      ConcreteTypeBuilder.SetCurrent (builder);

      builder.LoadAssemblyIntoCache (assemblyMock);

      using (SetupMixinConfigurationForLoadedType (loadedType).EnterScope())
      {
        var requestingClass = TargetClassDefinitionUtility.GetContext (
            typeof (ClassOverridingMixinMembers), 
            MixinConfiguration.ActiveConfiguration, 
            GenerationPolicy.GenerateOnlyIfConfigured);
        var concreteMixinIdentifier = 
            TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();

        Type concreteType = builder.GetConcreteMixinType (requestingClass, concreteMixinIdentifier).GeneratedType;
        Assert.That (concreteType, Is.SameAs (loadedType));
      }

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void LoadDoesntReplaceMixinTypes()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);

      using (SetupMixinConfigurationForLoadedType (loadedType).EnterScope())
      {
        var requestingClass = TargetClassDefinitionUtility.GetContext (
            typeof (ClassOverridingMixinMembers),
            MixinConfiguration.ActiveConfiguration,
            GenerationPolicy.GenerateOnlyIfConfigured);
        var concreteMixinIdentifier =
            TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier ();

        Type concreteType1 = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, concreteMixinIdentifier).GeneratedType;
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assemblyMock);
        Type concreteType2 = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, concreteMixinIdentifier).GeneratedType;

        Assert.That (concreteType2, Is.SameAs (concreteType1));
        Assert.That (concreteType2, Is.Not.SameAs (loadedType));
      }
    }

    [Test]
    public void LoadStillAllowsGeneration()
    {
      var loadedType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers);
      var assemblyMock = SetupAssemblyMockForLoading (loadedType);
      var typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator>();
      typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));

      var builder = new ConcreteTypeBuilder();
      ConcreteTypeBuilder.SetCurrent (builder);
      builder.Scope = _moduleManagerMockForLoading;

      builder.LoadAssemblyIntoCache (assemblyMock);
      _moduleManagerMockForLoading.BackToRecord();
      _moduleManagerMockForLoading.Expect (mock => mock.CreateTypeGenerator (Arg<CodeGenerationCache>.Is.Anything,
                                                                             Arg<TargetClassDefinition>.Is.Anything, Arg<INameProvider>.Is.Anything,
                                                                             Arg<INameProvider>.Is.Anything)).Return (typeGeneratorMock);
      _moduleManagerMockForLoading.Replay();

      var nonLoadedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (nonLoadedType, Is.SameAs (typeof (string)));

      _moduleManagerMockForLoading.VerifyAllExpectations();
    }

    [Test]
    public void GetConcreteMixinTypeBeforeGetConcreteTypeWorks()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSingleMixinMethod>().Clear().AddMixins (typeof (MixinWithOverridableMember)).
              EnterScope())
      {
        var requestingClass = TargetClassDefinitionUtility.GetContext (
            typeof (ClassOverridingSingleMixinMethod), 
            MixinConfiguration.ActiveConfiguration, 
            GenerationPolicy.GenerateOnlyIfConfigured);
        var concreteMixinIdentifier = 
            TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier();

        Type t = ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, concreteMixinIdentifier).GeneratedType;
        Assert.That (t, Is.Not.Null);
        Assert.That (typeof (MixinWithOverridableMember).IsAssignableFrom (t), Is.True);

        var instance = ObjectFactory.Create<ClassOverridingSingleMixinMethod>(ParamList.Empty);
        Assert.That (Mixin.Get<MixinWithOverridableMember> (instance).GetType (), Is.SameAs (t));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "No concrete mixin type is required for the given configuration "
                                                                      +
                                                                      "(mixin Remotion.UnitTests.Mixins.CodeGeneration.TestDomain.MixinWithOverridableMember and target class "
                                                                      + "Remotion.UnitTests.Mixins.SampleTypes.NullTarget).",
        MatchType = MessageMatch.Contains)]
    public void GetConcreteMixinTypeThrowsIfNoMixinTypeGenerated()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (typeof (MixinWithOverridableMember)).EnterScope())
      {
        var requestingClass = TargetClassDefinitionUtility.GetContext (
            typeof (NullTarget),
            MixinConfiguration.ActiveConfiguration,
            GenerationPolicy.GenerateOnlyIfConfigured);
        var concreteMixinIdentifier =
            TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0].GetConcreteMixinTypeIdentifier ();
        ConcreteTypeBuilder.Current.GetConcreteMixinType (requestingClass, concreteMixinIdentifier);
      }
    }

    [Test]
    public void InitializeUnconstructedInstanceDelegatesToScope()
    {
      var mockRepository = new MockRepository();
      var mockMixinTarget = mockRepository.StrictMock<IMixinTarget>();
      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      //expect
      mockScope.InitializeMixinTarget (mockMixinTarget);

      mockRepository.ReplayAll();

      builder.InitializeUnconstructedInstance (mockMixinTarget);

      mockRepository.VerifyAll();
    }

    [Test]
    public void BeginDeserializationDelegatesToScope()
    {
      var mockRepository = new MockRepository();

      Type deserializedType = typeof (object);
      var objectReference = mockRepository.StrictMock<IObjectReference>();
      var info = new SerializationInfo (deserializedType, new FormatterConverter());
      var context = new StreamingContext();

      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      Func<Type, Type> transformer = delegate { return null; };

      Expect.Call (mockScope.BeginDeserialization (transformer, info, context)).Return (objectReference);

      mockRepository.ReplayAll();

      Assert.That (builder.BeginDeserialization (transformer, info, context), Is.SameAs (objectReference));

      mockRepository.VerifyAll();
    }

    [Test]
    public void FinishDeserializationDelegatesToScope()
    {
      var mockRepository = new MockRepository();

      var objectReference = mockRepository.StrictMock<IObjectReference>();
      var mockScope = mockRepository.StrictMock<IModuleManager>();

      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      //expect
      mockScope.FinishDeserialization (objectReference);

      mockRepository.ReplayAll();

      builder.FinishDeserialization (objectReference);

      mockRepository.VerifyAll();
    }

    [Test]
    public void NewScopeEstablished_AfterSetNull()
    {
      IModuleManager oldModuleManager = ConcreteTypeBuilder.Current.Scope;
      Assert.That (oldModuleManager != null);

      ConcreteTypeBuilder.Current.Scope = null;
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.Null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.SameAs (oldModuleManager));
    }

    [Test]
    public void NewScope_HasNewAssemblyNames()
    {
      string oldSignedName = ConcreteTypeBuilder.Current.Scope.SignedAssemblyName;
      string oldUnsignedName = ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName;

      ConcreteTypeBuilder.Current.Scope = null;
      Assert.That (ConcreteTypeBuilder.Current.Scope.SignedAssemblyName, Is.Not.EqualTo (oldSignedName));
      Assert.That (ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName, Is.Not.EqualTo (oldUnsignedName));
    }

    [Test]
    public void NewScopeEstablished_AfterConcreteTypeBuilderSetNull()
    {
      IModuleManager oldModuleManager = ConcreteTypeBuilder.Current.Scope;
      Assert.That (oldModuleManager != null);

      ConcreteTypeBuilder.SetCurrent (null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.Null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.SameAs (oldModuleManager));
    }

    [Test]
    public void NewScopeHasNewAssemblyNames_AfterConcreteTypeBuilderSetNull()
    {
      string oldSignedName = ConcreteTypeBuilder.Current.Scope.SignedAssemblyName;
      string oldUnsignedName = ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName;

      ConcreteTypeBuilder.SetCurrent (null);
      Assert.That (ConcreteTypeBuilder.Current.Scope.SignedAssemblyName, Is.Not.EqualTo (oldSignedName));
      Assert.That (ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName, Is.Not.EqualTo (oldUnsignedName));
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated()
    {
      var nameProviderFake1 = MockRepository.GenerateMock<INameProvider>();
      var nameProviderFake2 = MockRepository.GenerateMock<INameProvider>();
      var moduleManagerMock = MockRepository.GenerateMock<IModuleManager>();
      var typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator>();
      var builder = new ConcreteTypeBuilder
                      {TypeNameProvider = nameProviderFake1, MixinTypeNameProvider = nameProviderFake2, Scope = moduleManagerMock};

      moduleManagerMock.Expect (mock => mock.CreateTypeGenerator (Arg<CodeGenerationCache>.Is.Anything, Arg<TargetClassDefinition>.Is.Anything,
                                                                  Arg<INameProvider>.Is.Same (nameProviderFake1),
                                                                  Arg<INameProvider>.Is.Same (nameProviderFake2))).Return (typeGeneratorMock);
      moduleManagerMock.Replay();

      typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));
      typeGeneratorMock.Replay();

      var classContext = TargetClassDefinitionUtility.GetContext (typeof (BaseType1), MixinConfiguration.ActiveConfiguration, GenerationPolicy.GenerateOnlyIfConfigured);
      builder.GetConcreteType (classContext);
      moduleManagerMock.VerifyAllExpectations();
    }

    private _Assembly SetupAssemblyMockForLoading (params Type[] loadedTypes)
    {
      var assemblyMock = MockRepository.GenerateMock<_Assembly>();
      assemblyMock.Expect (mock => mock.GetName()).Return (new AssemblyName ("TestAssembly"));
      assemblyMock.Expect (mock => mock.GetExportedTypes()).Return (loadedTypes);
      assemblyMock.Replay();
      return assemblyMock;
    }

    private MixinConfiguration SetupMixinConfigurationForLoadedType (Type loadedType)
    {
      object[] classContextData;
      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (loadedType, false);
      if (attribute != null)
      {
        classContextData = attribute.ClassContextData;
      }
      else
      {
        var mixinAttribute = AttributeUtility.GetCustomAttribute<ConcreteMixinTypeAttribute> (loadedType, false);
        classContextData = mixinAttribute.ClassContextData;
      }

      var classContext = ClassContext.Deserialize (new AttributeClassContextDeserializer (classContextData));
      var mixinConfiguration = new MixinConfiguration();
      mixinConfiguration.ClassContexts.Add (classContext);
      return mixinConfiguration;
    }
  }
}