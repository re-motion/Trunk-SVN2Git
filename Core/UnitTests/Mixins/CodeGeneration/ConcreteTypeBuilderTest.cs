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
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using Remotion.Context;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteTypeBuilderTest : CodeGenerationBaseTest
  {
    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder ()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (null);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreNotSame (t1, t2);
    }

    [Test]
    public void CacheEvenWorksForSerialization ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
      BaseType1 bt2 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.AreSame (bt1.GetType(), bt2.GetType());

      BaseType1[] array = Serializer.SerializeAndDeserialize (new BaseType1[] {bt1, bt2});
      Assert.AreSame (bt1.GetType(), array[0].GetType());
      Assert.AreSame (array[0].GetType(), array[1].GetType());
    }

    [Test]
    public void GeneratedMixinTypesAreCached()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>().With();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ConcreteTypeBuilder.SetCurrent (null);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreNotSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new ClassOverridingMixinMembers[] { c1, c2 });
      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType (), Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType ());
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      ConcreteTypeBuilder newBuilder = new ConcreteTypeBuilder ();
      Assert.IsFalse (ConcreteTypeBuilder.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { ConcreteTypeBuilder.SetCurrent (newBuilder); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (ConcreteTypeBuilder.HasCurrent);
      Assert.AreSame (newBuilder, ConcreteTypeBuilder.Current);
    }

    [Test]
    public void LockAndAccessScope ()
    {
      IModuleManager scope = ConcreteTypeBuilder.Current.Scope;
      ConcreteTypeBuilder.Current.LockAndAccessScope(delegate (IModuleManager lockedScope)
      {
        Assert.AreSame (scope, lockedScope);
      });
    }

    [Test]
    public void DefaultNameProviderIsGuid ()
    {
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.TypeNameProvider);
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.MixinTypeNameProvider);
    }

    [Test]
    public void CanSaveAndResetScope ()
    {
      MockRepository repository = new MockRepository();
      IModuleManager managerMock = repository.CreateMock<IModuleManager>();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = managerMock;

      string[] paths = new string[] { "Foos", "Bars", "Stripes" };

      using (repository.Ordered ())
      {
        Expect.Call (managerMock.HasSignedAssembly).Return (false);
        Expect.Call (managerMock.HasUnsignedAssembly).Return (true);
        Expect.Call (managerMock.SaveAssemblies ()).Return (paths);
      }

      repository.ReplayAll ();

      builder.SaveAndResetDynamicScope ();

      repository.VerifyAll ();
    }

    [Test]
    public void HandlesSaveWithoutGeneratedTypesGracefully ()
    {
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void ResetsScopeWhenSaving ()
    {
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      IModuleManager scopeBefore = builder.Scope;
      builder.SaveAndResetDynamicScope ();
      Assert.AreNotSame (scopeBefore, builder.Scope);
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSaving ()
    {
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      Assert.IsNotNull (builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
      Assert.IsNotNull (builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration)));
      builder.SaveAndResetDynamicScope ();
      Assert.IsNotNull (builder.GetConcreteType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void SavingGeneratingCachingIntegration ()
    {
      Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.IsNotEmpty (paths);
      Type concreteType2 = TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
      paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.IsNotEmpty (paths);
      Type concreteType3 = TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration);

      Assert.AreSame (concreteType1, TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType2, TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType3, TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Signed' into the cache "
        + "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
            + "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly")]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsSigned ()
    {
      ModuleManager moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (true);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.StrongNamedModule.Assembly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Unsigned' into the cache "
        + "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
            + "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly")]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsUnsigned ()
    {
      ModuleManager moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (false);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.WeakNamedModule.Assembly);
    }

    [Test]
    public void LoadAddsLoadedBaseTypesToTheCache ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      
      AppDomainRunner.Run (delegate (object[] args)
      {
        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;

        MockRepository repository = new MockRepository ();
        IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
        ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

        Expect.Call (moduleManagerMock.SignedAssemblyName).Return ("FooS");
        Expect.Call (moduleManagerMock.UnsignedAssemblyName).Return ("FooU");
        // expecting _no_ other actions on the scope when loading and accessing types from saved module

        repository.ReplayAll ();

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
        string expectedTypeName = (string) args[0];
        Assert.AreEqual (expectedTypeName, concreteType.FullName);
        Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

        repository.VerifyAll ();
      }, concreteTypeName);
    }

    [Test]
    public void LoadDoesntReplaceTypes ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;
        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";

        Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1));

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        Type concreteType1b = TypeFactory.GetConcreteType (typeof (BaseType1));
        Assert.AreSame (concreteType1, concreteType1b);
            
        string outerTypeName = (string) args[0];
        Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);

      }, concreteTypeName);
    }

    [Test]
    public void LoadAddsLoadedMixinTypesToTheCache ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;

        MockRepository repository = new MockRepository ();
        IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
        ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

        Expect.Call (moduleManagerMock.SignedAssemblyName).Return ("FooS");
        Expect.Call (moduleManagerMock.UnsignedAssemblyName).Return ("FooU");
        // expecting _no_ other actions on the scope when loading and accessing types from saved module

        repository.ReplayAll ();

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        MixinDefinition innerMixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
        Assert.IsNotNull (innerMixinDefinition);

        Type concreteType = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        string expectedTypeName = (string) args[0];
        Assert.AreEqual (expectedTypeName, concreteType.FullName);
        Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

        repository.VerifyAll ();
      }, concreteTypeName);
    }

    [Test]
    public void LoadDoesntReplaceMixinTypes ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;
        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";

        MixinDefinition innerMixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];

        Type concreteType1a = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        Type concreteType1b = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        Assert.AreSame (concreteType1a, concreteType1b);

        string outerTypeName = (string) args[0];
        Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);
      }, concreteTypeName);
    }

    [Test]
    public void LoadStillAllowsGeneration ()
    {
      TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate
      {
        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;
        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";
        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        MockRepository repository = new MockRepository ();

        IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
        IModuleManager realScope = ConcreteTypeBuilder.Current.Scope;
        ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

        Expect.Call (moduleManagerMock.SignedAssemblyName).Return ("whatever");
        Expect.Call (moduleManagerMock.UnsignedAssemblyName).Return ("whatever");

        repository.Replay (moduleManagerMock);

        TargetClassDefinition innerClassDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration);
        MixinDefinition innerMixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSingleMixinMethod)).Mixins[typeof (MixinWithSingleAbstractMethod)];

        repository.BackToRecord (moduleManagerMock);

        using (repository.Ordered ())
        {
          Expect.Call (moduleManagerMock.CreateTypeGenerator (innerClassDefinition, GuidNameProvider.Instance, GuidNameProvider.Instance))
              .Return (realScope.CreateTypeGenerator (innerClassDefinition, GuidNameProvider.Instance, GuidNameProvider.Instance));
          Expect.Call (moduleManagerMock.CreateTypeGenerator (innerMixinDefinition.TargetClass, GuidNameProvider.Instance, GuidNameProvider.Instance))
              .Return (realScope.CreateTypeGenerator (innerMixinDefinition.TargetClass, GuidNameProvider.Instance, GuidNameProvider.Instance));
        }

        repository.ReplayAll ();

        // causes CreateTypeGenerator
        TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
        // causes CreateMixinTypeGenerator
        ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        // causes nothing, was loaded
        TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
        // causes nothing, was loaded
        innerMixinDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
        ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);
        
        repository.VerifyAll ();
      });
    }

    [Test]
    public void GetConcreteMixinTypeBeforeGetConcreteTypeWorks ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseOverridingMixinMember> ().Clear().AddMixins (typeof (MixinWithOverridableMember)).EnterScope())
      {
        Type t = ConcreteTypeBuilder.Current.GetConcreteMixinType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseOverridingMixinMember)).Mixins[0]);
        Assert.IsNotNull (t);
        Assert.IsTrue (typeof (MixinWithOverridableMember).IsAssignableFrom (t));

        BaseOverridingMixinMember instance = ObjectFactory.Create<BaseOverridingMixinMember> ().With ();
        Assert.AreSame (t, Mixin.Get<MixinWithOverridableMember> (instance).GetType ());
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "No concrete mixin type is required for the given configuration "
        + "(mixin Remotion.UnitTests.Mixins.CodeGeneration.SampleTypes.MixinWithOverridableMember and target class "
            + "Remotion.UnitTests.Mixins.SampleTypes.NullTarget).",
        MatchType = MessageMatch.Contains)]
    public void GetConcreteMixinTypeThrowsIfNoMixinTypeGenerated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithOverridableMember)).EnterScope())
      {
        ConcreteTypeBuilder.Current.GetConcreteMixinType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[0]);
      }
    }

    [Test]
    public void InitializeUnconstructedInstanceDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository();
      IMixinTarget mockMixinTarget = mockRepository.CreateMock<IMixinTarget>();
      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();
      
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      //expect
      mockScope.InitializeMixinTarget (mockMixinTarget);

      mockRepository.ReplayAll ();

      builder.InitializeUnconstructedInstance (mockMixinTarget);

      mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginDeserializationDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository ();

      Type deserializedType = typeof (object);
      IObjectReference objectReference = mockRepository.CreateMock<IObjectReference> ();
      SerializationInfo info = new SerializationInfo (deserializedType, new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      Func<Type, Type> transformer = delegate { return null; };

      Expect.Call (mockScope.BeginDeserialization (transformer, info, context)).Return (objectReference);

      mockRepository.ReplayAll ();

      Assert.AreSame (objectReference, builder.BeginDeserialization (transformer, info, context));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FinishDeserializationDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository ();

      IObjectReference objectReference = mockRepository.CreateMock<IObjectReference> ();
      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      //expect
      mockScope.FinishDeserialization (objectReference);

      mockRepository.ReplayAll ();

      builder.FinishDeserialization (objectReference);

      mockRepository.VerifyAll ();
    }
  }
}
