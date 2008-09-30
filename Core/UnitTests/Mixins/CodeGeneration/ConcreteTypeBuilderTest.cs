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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using Remotion.Context;
using System.Linq;

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

      BaseType1[] array = Serializer.SerializeAndDeserialize (new[] {bt1, bt2});
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

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new[] { c1, c2 });
      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType (), Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType ());
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      var newBuilder = new ConcreteTypeBuilder ();
      Assert.IsFalse (ConcreteTypeBuilder.HasCurrent);
      var setterThread = new Thread ((ThreadStart) delegate { ConcreteTypeBuilder.SetCurrent (newBuilder); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (ConcreteTypeBuilder.HasCurrent);
      Assert.AreSame (newBuilder, ConcreteTypeBuilder.Current);
    }

    [Test]
    public void LockAndAccessScope ()
    {
      IModuleManager scope = ConcreteTypeBuilder.Current.Scope;
      ConcreteTypeBuilder.Current.LockAndAccessScope(lockedScope => Assert.AreSame (scope, lockedScope));
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
      var repository = new MockRepository();
      var managerMock = repository.StrictMock<IModuleManager>();

      var builder = new ConcreteTypeBuilder {Scope = managerMock};

      var paths = new[] { "Foos", "Bars", "Stripes" };

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
      var builder = new ConcreteTypeBuilder ();
      IModuleManager scopeBefore = builder.Scope;
      builder.SaveAndResetDynamicScope ();
      Assert.AreNotSame (scopeBefore, builder.Scope);
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSaving ()
    {
      var builder = new ConcreteTypeBuilder ();
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Signed.*' into the cache "
        + "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
            + "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly", MatchType = MessageMatch.Regex)]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsSigned ()
    {
      var moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (true);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.StrongNamedModule.Assembly);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot load assembly 'Remotion.Mixins.Generated.Unsigned.*' into the cache "
        + "because it has the same name as one of the dynamic assemblies used by the mixin engine. Having two assemblies with the same name loaded "
            + "into one AppDomain can cause strange and sporadic TypeLoadExceptions.\r\nParameter name: assembly", MatchType = MessageMatch.Regex)]
    public void LoadThrows_WhenLoadedAssemblyHasSameName_AsUnsigned ()
    {
      var moduleManager = (ModuleManager) ConcreteTypeBuilder.Current.Scope;
      moduleManager.Scope.ObtainDynamicModule (false);
      ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (moduleManager.Scope.WeakNamedModule.Assembly);
    }

    [Test]
    public void LoadAddsLoadedBaseTypesToTheCache ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      
      AppDomainRunner.Run (delegate (object[] args)
      {
        var expectedTypeName = (string) args[0];
        var modulePath = (string) args[1];

        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        var repository = new MockRepository ();
        var moduleManagerMock = repository.StrictMock<IModuleManager> ();
        ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

        Expect.Call (moduleManagerMock.SignedAssemblyName).Return ("FooS");
        Expect.Call (moduleManagerMock.UnsignedAssemblyName).Return ("FooU");
        // expecting _no_ other actions on the scope when loading and accessing types from saved module

        repository.ReplayAll ();

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
        Assert.AreEqual (expectedTypeName, concreteType.FullName);
        Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

        repository.VerifyAll ();
      }, concreteTypeName, paths.Single());
    }

    [Test]
    public void LoadDoesntReplaceTypes ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        var outerTypeName = (string) args[0];
        var modulePath = (string) args[1];

        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";

        Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1));

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        Type concreteType1b = TypeFactory.GetConcreteType (typeof (BaseType1));
        Assert.AreSame (concreteType1, concreteType1b);
            
        Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);

      }, concreteTypeName, paths.Single ());
    }

    [Test]
    public void LoadAddsLoadedMixinTypesToTheCache ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        var expectedTypeName = (string) args[0];
        var modulePath = (string) args[1];

        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        var repository = new MockRepository ();
        var moduleManagerMock = repository.StrictMock<IModuleManager> ();
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

        Assert.AreEqual (expectedTypeName, concreteType.FullName);
        Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

        repository.VerifyAll ();
      }, concreteTypeName, paths.Single ());
    }

    [Test]
    public void LoadDoesntReplaceMixinTypes ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        var outerTypeName = (string) args[0];
        var modulePath = (string) args[1];

        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";

        MixinDefinition innerMixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];

        Type concreteType1a = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        Type concreteType1b = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        Assert.AreSame (concreteType1a, concreteType1b);

        Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);
      }, concreteTypeName, paths.Single ());
    }

    [Test]
    public void LoadStillAllowsGeneration ()
    {
      TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
      {
        var modulePath = (string) args[0];

        // ensure SafeContext is initialized, otherwise, this will interfere with our test below
        Dev.Null = SafeContext.Instance;

        ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = "Bla";
        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (assembly);

        MixinDefinition innerMixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingSingleMixinMethod)).Mixins[typeof (MixinWithSingleAbstractMethod)];

        // causes CreateTypeGenerator
        Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
        Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (generatedType));
        Assert.That (generatedType.Assembly, Is.Not.SameAs (assembly));

        // causes CreateMixinTypeGenerator
        Type generatedMixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);
        Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (generatedMixinType));
        Assert.That (generatedMixinType.Assembly, Is.Not.SameAs (assembly));

        // causes nothing, was loaded
        Type loadedType = TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
        Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (loadedType));
        Assert.That (loadedType.Assembly, Is.SameAs (assembly));

        // causes nothing, was loaded
        innerMixinDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
        Type loadedMixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);
        Assert.That (MixinTypeUtility.IsGeneratedByMixinEngine (loadedMixinType));
        Assert.That (loadedMixinType.Assembly, Is.SameAs (assembly));
      }, paths.Single());
    }

    [Test]
    public void GetConcreteMixinTypeBeforeGetConcreteTypeWorks ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<TargetClassOverridingMixinMember> ().Clear().AddMixins (typeof (MixinWithOverridableMember)).EnterScope())
      {
        Type t = ConcreteTypeBuilder.Current.GetConcreteMixinType (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassOverridingMixinMember)).Mixins[0]);
        Assert.IsNotNull (t);
        Assert.IsTrue (typeof (MixinWithOverridableMember).IsAssignableFrom (t));

        TargetClassOverridingMixinMember instance = ObjectFactory.Create<TargetClassOverridingMixinMember> ().With ();
        Assert.AreSame (t, Mixin.Get<MixinWithOverridableMember> (instance).GetType ());
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "No concrete mixin type is required for the given configuration "
        + "(mixin Remotion.UnitTests.Mixins.CodeGeneration.TestDomain.MixinWithOverridableMember and target class "
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
      var mockRepository = new MockRepository();
      var mockMixinTarget = mockRepository.StrictMock<IMixinTarget>();
      var mockScope = mockRepository.StrictMock<IModuleManager> ();
      
      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      //expect
      mockScope.InitializeMixinTarget (mockMixinTarget);

      mockRepository.ReplayAll ();

      builder.InitializeUnconstructedInstance (mockMixinTarget);

      mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginDeserializationDelegatesToScope ()
    {
      var mockRepository = new MockRepository ();

      Type deserializedType = typeof (object);
      var objectReference = mockRepository.StrictMock<IObjectReference> ();
      var info = new SerializationInfo (deserializedType, new FormatterConverter ());
      var context = new StreamingContext ();

      var mockScope = mockRepository.StrictMock<IModuleManager> ();

      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      Func<Type, Type> transformer = delegate { return null; };

      Expect.Call (mockScope.BeginDeserialization (transformer, info, context)).Return (objectReference);

      mockRepository.ReplayAll ();

      Assert.AreSame (objectReference, builder.BeginDeserialization (transformer, info, context));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FinishDeserializationDelegatesToScope ()
    {
      var mockRepository = new MockRepository ();

      var objectReference = mockRepository.StrictMock<IObjectReference> ();
      var mockScope = mockRepository.StrictMock<IModuleManager> ();

      var builder = new ConcreteTypeBuilder {Scope = mockScope};

      //expect
      mockScope.FinishDeserialization (objectReference);

      mockRepository.ReplayAll ();

      builder.FinishDeserialization (objectReference);

      mockRepository.VerifyAll ();
    }

    [Test]
    public void NewScopeEstablished_AfterSetNull ()
    {
      IModuleManager oldModuleManager = ConcreteTypeBuilder.Current.Scope;
      Assert.That (oldModuleManager != null);

      ConcreteTypeBuilder.Current.Scope = null;
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.Null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.SameAs (oldModuleManager));
    }

    [Test]
    public void NewScope_HasNewAssemblyNames ()
    {
      string oldSignedName = ConcreteTypeBuilder.Current.Scope.SignedAssemblyName;
      string oldUnsignedName = ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName;

      ConcreteTypeBuilder.Current.Scope = null;
      Assert.That (ConcreteTypeBuilder.Current.Scope.SignedAssemblyName, Is.Not.EqualTo (oldSignedName));
      Assert.That (ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName, Is.Not.EqualTo (oldUnsignedName));
    }

    [Test]
    public void NewScopeEstablished_AfterConcreteTypeBuilderSetNull ()
    {
      IModuleManager oldModuleManager = ConcreteTypeBuilder.Current.Scope;
      Assert.That (oldModuleManager != null);

      ConcreteTypeBuilder.SetCurrent(null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.Null);
      Assert.That (ConcreteTypeBuilder.Current.Scope, Is.Not.SameAs (oldModuleManager));
    }

    [Test]
    public void NewScopeHasNewAssemblyNames_AfterConcreteTypeBuilderSetNull ()
    {
      string oldSignedName = ConcreteTypeBuilder.Current.Scope.SignedAssemblyName;
      string oldUnsignedName = ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName;

      ConcreteTypeBuilder.SetCurrent (null);
      Assert.That (ConcreteTypeBuilder.Current.Scope.SignedAssemblyName, Is.Not.EqualTo (oldSignedName));
      Assert.That (ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName, Is.Not.EqualTo (oldUnsignedName));
    }

  }
}
