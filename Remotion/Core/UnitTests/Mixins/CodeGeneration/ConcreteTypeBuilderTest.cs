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
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Context;
using System.Linq;

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
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (AlternativeTypeBuilder);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreNotSame (t1, t2);
    }

    [Test]
    public void CacheEvenWorksForSerialization()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>(ParamList.Empty);
      BaseType1 bt2 = ObjectFactory.Create<BaseType1>(ParamList.Empty);

      Assert.AreSame (bt1.GetType(), bt2.GetType());

      BaseType1[] array = Serializer.SerializeAndDeserialize (new[] {bt1, bt2});
      Assert.AreSame (bt1.GetType(), array[0].GetType());
      Assert.AreSame (array[0].GetType(), array[1].GetType());
    }

    [Test]
    public void GeneratedMixinTypesAreCached()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      ConcreteTypeBuilder.SetCurrent (AlternativeTypeBuilder);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.AreNotSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers>(ParamList.Empty);

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new[] {c1, c2});
      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType(), Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType());
    }

    [Test]
    public void CurrentIsGlobalSingleton()
    {
      ConcreteTypeBuilder.SetCurrent (null);

      var newBuilder = new ConcreteTypeBuilder();
      Assert.IsFalse (ConcreteTypeBuilder.HasCurrent);
      var setterThread = new Thread ((ThreadStart) delegate { ConcreteTypeBuilder.SetCurrent (newBuilder); });
      setterThread.Start();
      setterThread.Join();

      Assert.IsTrue (ConcreteTypeBuilder.HasCurrent);
      Assert.AreSame (newBuilder, ConcreteTypeBuilder.Current);
    }

    [Test]
    public void LockAndAccessScope()
    {
      IModuleManager scope = ConcreteTypeBuilder.Current.Scope;
      ConcreteTypeBuilder.Current.LockAndAccessScope (lockedScope => Assert.AreSame (scope, lockedScope));
    }

    [Test]
    public void DefaultNameProviderIsGuid()
    {
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.TypeNameProvider);
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.MixinTypeNameProvider);
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
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void ResetsScopeWhenSaving()
    {
      var builder = new ConcreteTypeBuilder();
      IModuleManager scopeBefore = builder.Scope;
      builder.SaveAndResetDynamicScope();
      Assert.AreNotSame (scopeBefore, builder.Scope);
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSaving()
    {
      var builder = new ConcreteTypeBuilder();
      Assert.IsNotNull (
          builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
      Assert.IsNotNull (
          builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration)));
      builder.SaveAndResetDynamicScope();
      Assert.IsNotNull (
          builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3), GenerationPolicy.ForceGeneration)));
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

      Assert.AreSame (concreteType1, TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType2, TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType3, TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration));
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
        Assert.AreSame (loadedType, concreteType);
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
        Type concreteType =
            builder.GetConcreteMixinType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[0]).
                GeneratedType;
        Assert.AreSame (loadedType, concreteType);
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
        Type concreteType1 = ConcreteTypeBuilder.Current.GetConcreteMixinType (
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[0]).GeneratedType;
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assemblyMock);
        Type concreteType2 = ConcreteTypeBuilder.Current.GetConcreteMixinType (
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[0]).GeneratedType;

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
          MixinConfiguration.BuildFromActive().ForClass<TargetClassOverridingMixinMember>().Clear().AddMixins (typeof (MixinWithOverridableMember)).
              EnterScope())
      {
        Type t =
            ConcreteTypeBuilder.Current.GetConcreteMixinType (
                TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassOverridingMixinMember)).Mixins[0]).GeneratedType;
        Assert.IsNotNull (t);
        Assert.IsTrue (typeof (MixinWithOverridableMember).IsAssignableFrom (t));

        TargetClassOverridingMixinMember instance = ObjectFactory.Create<TargetClassOverridingMixinMember>(ParamList.Empty);
        Assert.AreSame (t, Mixin.Get<MixinWithOverridableMember> (instance).GetType());
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
        ConcreteTypeBuilder.Current.GetConcreteMixinType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[0]);
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

      Assert.AreSame (objectReference, builder.BeginDeserialization (transformer, info, context));

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
      TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

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

      builder.GetConcreteType (definition);
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
      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (loadedType, false);
      if (attribute == null)
      {
        var mixinAttribute = AttributeUtility.GetCustomAttribute<ConcreteMixinTypeAttribute> (loadedType, false);
        attribute = new ConcreteMixedTypeAttribute (mixinAttribute.Data);
      }

      var classContext = attribute.GetClassContext();
      var mixinConfiguration = new MixinConfiguration();
      mixinConfiguration.ClassContexts.Add (classContext);
      return mixinConfiguration;
    }
  }
}