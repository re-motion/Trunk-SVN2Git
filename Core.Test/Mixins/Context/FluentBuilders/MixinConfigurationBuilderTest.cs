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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.UnitTests.Mixins.Context.FluentBuilders
{
  [TestFixture]
  public class MixinConfigurationBuilderTest
  {
    [Test]
    public void Intitialization_WithoutParent ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      Assert.IsNull (builder.ParentConfiguration);
      Assert.That (builder.ClassContextBuilders, Is.Empty);
      MixinConfiguration configuration = builder.BuildConfiguration();
      Assert.AreEqual (0, configuration.ClassContexts.Count);
    }

    [Test]
    public void Intitialization_WithParent ()
    {
      MixinConfiguration parent = new MixinConfiguration (null);
      parent.ClassContexts.Add (new ClassContext (typeof (string)));

      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parent);
      Assert.AreSame (parent, builder.ParentConfiguration);
      Assert.That (builder.ClassContextBuilders, Is.Empty);

      MixinConfiguration configuration = builder.BuildConfiguration ();
      Assert.AreEqual (1, configuration.ClassContexts.Count);
      Assert.IsTrue (configuration.ClassContexts.ContainsWithInheritance (typeof (string)));
    }

    [Test]
    public void ForClass_NonGeneric ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      Assert.AreSame (typeof (BaseType1), classBuilder.TargetType);
      Assert.AreSame (builder, classBuilder.Parent);
      Assert.That (builder.ClassContextBuilders, List.Contains (classBuilder));
    }

    [Test]
    public void ForClass_Twice()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass (typeof (BaseType1));
      ClassContextBuilder classBuilder2 = builder.ForClass (typeof (BaseType1));
      Assert.AreSame (classBuilder, classBuilder2);
    }

    [Test]
    public void ForClass_Generic ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.AreSame (typeof (BaseType1), classBuilder.TargetType);
      Assert.AreSame (builder, classBuilder.Parent);
      Assert.That (builder.ClassContextBuilders, List.Contains (classBuilder));
    }

    [Test]
    public void ForClass_WithExistingContext ()
    {
      ClassContext existingContext = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      parentConfiguration.ClassContexts.Add (existingContext);

      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parentConfiguration);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Not.Empty);
    }

    [Test]
    public void ForClass_WithoutExistingContext_NullParentConfiguration ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
    }

    [Test]
    public void ForClass_WithoutExistingContext_WithParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfiguration (null);
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (parentConfiguration);
      ClassContextBuilder classBuilder = builder.ForClass<BaseType1> ();
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
    }

    [Test]
    public void BuildConfiguration ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      builder.ForClass<BaseType1> ();
      MixinConfiguration configurtation = builder.BuildConfiguration();
      Assert.AreEqual (1, configurtation.ClassContexts.Count);
      Assert.IsTrue (configurtation.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
    }

    [Test]
    public void EnterScope ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
      using (new MixinConfigurationBuilder (null).ForClass<BaseType4> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void ClassContextInheritance_Base_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass<NullTarget>().AddMixin (typeof (NullMixin)).BuildConfiguration();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<DerivedNullTarget>().AddMixin (typeof (NullMixin2)).BuildConfiguration();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (DerivedNullTarget));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration =
          new MixinConfigurationBuilder (null).ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin)).BuildConfiguration ();
      MixinConfiguration configuration =
          new MixinConfigurationBuilder (parentConfiguration).ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2)).BuildConfiguration ();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (GenericTargetClass<int>));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (DerivedGenericTargetClass<int>));
      Assert.AreEqual (4, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)));
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromParentConfiguration ()
    {
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      MixinConfiguration configuration = new MixinConfigurationBuilder (parentConfiguration)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();
      
      ClassContext derivedContext1 = configuration.ClassContexts.GetWithInheritance (typeof (DerivedNullTarget));
      Assert.AreEqual (1, derivedContext1.Mixins.Count);
      Assert.IsTrue (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)));

      ClassContext derivedContext2 = configuration.ClassContexts.GetWithInheritance (typeof (GenericTargetClass<int>));
      Assert.AreEqual (1, derivedContext2.Mixins.Count);
      Assert.IsTrue (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void ClassContextInheritance_Base_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (NullMixin2))
          .ForClass<NullTarget> ().AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (DerivedNullTarget));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_TypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (GenericTargetClass<int>));
      Assert.AreEqual (2, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void ClassContextInheritance_BaseAndTypeDefinition_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass<DerivedGenericTargetClass<int>> ().AddMixin (typeof (NullMixin4))
          .ForClass (typeof (DerivedGenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin2))
          .ForClass (typeof (GenericTargetClass<int>)).AddMixin (typeof (NullMixin3))
          .BuildConfiguration ();
      ClassContext derivedContext = configuration.ClassContexts.GetWithInheritance (typeof (DerivedGenericTargetClass<int>));
      Assert.AreEqual (4, derivedContext.Mixins.Count);
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin3)));
      Assert.IsTrue (derivedContext.Mixins.ContainsKey (typeof (NullMixin4)));
    }

    [Test]
    public void ClassContextInheritance_WithOverrides_FromSameConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (NullTarget)).AddMixin (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<>)).AddMixin (typeof (NullMixin))
          .ForClass<DerivedNullTarget> ().AddMixin (typeof (DerivedNullMixin))
          .ForClass<GenericTargetClass<int>> ().AddMixin (typeof (DerivedNullMixin))
          .BuildConfiguration ();

      ClassContext derivedContext1 = configuration.ClassContexts.GetWithInheritance (typeof (DerivedNullTarget));
      Assert.AreEqual (1, derivedContext1.Mixins.Count);
      Assert.IsTrue (derivedContext1.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext1.Mixins.ContainsKey (typeof (NullMixin)));

      ClassContext derivedContext2 = configuration.ClassContexts.GetWithInheritance (typeof (GenericTargetClass<int>));
      Assert.AreEqual (1, derivedContext2.Mixins.Count);
      Assert.IsTrue (derivedContext2.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsFalse (derivedContext2.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void CompleteInterfaceRegistration ()
    {
      MixinConfiguration configuration = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType6))
          .AddMixin (typeof (BT6Mixin1))
          .AddCompleteInterface (typeof (ICBT6Mixin1)).BuildConfiguration();
      ClassContext resolvedContext = configuration.ResolveInterface (typeof (ICBT6Mixin1));
      Assert.IsNotNull (resolvedContext);
      Assert.AreEqual (typeof (BaseType6), resolvedContext.Type);
      Assert.AreEqual (1, resolvedContext.Mixins.Count);
      Assert.IsTrue (resolvedContext.Mixins.ContainsKey (typeof (BT6Mixin1)));
    }

    [Test]
    public void AddMixinToClass ()
    {
      MockRepository mockRepository = new MockRepository();
      MixinConfigurationBuilder builder = mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);

      Type targetType = typeof (object);
      Type mixinType = typeof (string);
      Type[] explicitDependencies = new Type[] { typeof (int) };
      Type[] suppressedMixins = new Type[] { typeof (double) };

      ClassContextBuilder classBuilderMock = mockRepository.CreateMock<ClassContextBuilder> (builder, targetType, null);
      MixinContextBuilder mixinBuilderMock = mockRepository.CreateMock<MixinContextBuilder> (classBuilderMock, mixinType);

      using (mockRepository.Ordered ())
      {
        Expect.Call (builder.AddMixinToClass (MixinKind.Extending, targetType, mixinType, explicitDependencies, suppressedMixins))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (builder.ForClass (targetType)).Return (classBuilderMock);
        Expect.Call (classBuilderMock.AddMixin (mixinType)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.OfKind (MixinKind.Extending)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.WithDependencies (explicitDependencies)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.SuppressMixins (suppressedMixins)).Return (classBuilderMock);
      }

      mockRepository.ReplayAll ();
      MixinConfigurationBuilder returnedBuilder = builder.AddMixinToClass (MixinKind.Extending, targetType, mixinType, explicitDependencies, suppressedMixins);
      Assert.AreSame (builder, returnedBuilder);
      mockRepository.VerifyAll ();
    }

    [Test]
    public void AddMixinToClass_Used ()
    {
      MockRepository mockRepository = new MockRepository ();
      MixinConfigurationBuilder builder = mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      Type targetType = typeof (object);
      Type mixinType = typeof (string);
      Type[] explicitDependencies = new Type[0];
      Type[] suppressedMixins = new Type[0];

      ClassContextBuilder classBuilderMock = mockRepository.CreateMock<ClassContextBuilder> (builder, targetType, null);
      MixinContextBuilder mixinBuilderMock = mockRepository.CreateMock<MixinContextBuilder> (classBuilderMock, mixinType);

      using (mockRepository.Ordered ())
      {
        Expect.Call (builder.AddMixinToClass (MixinKind.Used, targetType, mixinType, explicitDependencies, suppressedMixins))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (builder.ForClass (targetType)).Return (classBuilderMock);
        Expect.Call (classBuilderMock.AddMixin (mixinType)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.OfKind (MixinKind.Used)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.WithDependencies (explicitDependencies)).Return (mixinBuilderMock);
        Expect.Call (mixinBuilderMock.SuppressMixins (suppressedMixins)).Return (classBuilderMock);
      }

      mockRepository.ReplayAll ();
      MixinConfigurationBuilder returnedBuilder = builder.AddMixinToClass (MixinKind.Used, targetType, mixinType, explicitDependencies, suppressedMixins);
      Assert.AreSame (builder, returnedBuilder);
      mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin type System.Int32 applied to target class System.Object "
        + "suppresses itself.")]
    public void AddMixinToClass_WithSelfSuppressor ()
    {
      MixinConfigurationBuilder builder = new MixinConfigurationBuilder (null);
      builder.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (int), new Type[0], new Type[] {typeof (int)});
    }
  }
}
