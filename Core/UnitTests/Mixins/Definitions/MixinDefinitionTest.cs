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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MixinDefinitionTest
  {
    [Test]
    public void NeedsDerivedMixinType_True_OverriddenMember()
    {
      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers))
          .Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (definition.NeedsDerivedMixinType (), Is.True);
    }

    [Test]
    public void NeedsDerivedMixinType_True_ProtectedOverrider ()
    {
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().AddMixin<MixinWithProtectedOverrider>().EnterScope())
      {
        var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (MixinWithProtectedOverrider)];
        Assert.That (definition.NeedsDerivedMixinType(), Is.True);
      }
    }

    [Test]
    public void NeedsDerivedMixinType_False ()
    {
      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (definition.NeedsDerivedMixinType (), Is.False);
    }

    [Test]
    public void GetConcreteMixinTypeCacheKey_NoOverrides ()
    {
      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (definition.GetConcreteMixinTypeCacheKey (), Is.SameAs (definition));
    }

    [Test]
    public void GetConcreteMixinTypeCacheKey_Overrides_TypeOverridesMethod ()
    {
      var overrider = typeof (DerivedClassOverridingMixinMethod).GetMethod ("M1");

      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (DerivedClassOverridingMixinMethod)).Mixins[typeof (MixinWithMethodsOverriddenByDifferentClasses)];
      var key = Tuple.NewTuple (typeof (MixinWithMethodsOverriddenByDifferentClasses), new SetBasedCacheKey<MethodInfo> (overrider));
      Assert.That (definition.GetConcreteMixinTypeCacheKey (), Is.EqualTo (key));
    }

    [Test]
    public void GetConcreteMixinTypeCacheKey_Overrides_TypeOverridesMethod_AndBaseOverridesOtherMethod ()
    {
      var overrider1 = typeof (DerivedClassOverridingMixinMethod).GetMethod ("M1");
      var overrider2 = typeof (DerivedDerivedClassOverridingMixinMethod).GetMethod ("M2");

      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (DerivedDerivedClassOverridingMixinMethod)).Mixins[typeof (MixinWithMethodsOverriddenByDifferentClasses)];
      var key = Tuple.NewTuple (typeof (MixinWithMethodsOverriddenByDifferentClasses), new SetBasedCacheKey<MethodInfo> (overrider1, overrider2));
      Assert.That (definition.GetConcreteMixinTypeCacheKey (), Is.EqualTo (key));
    }

    [Test]
    public void GetConcreteMixinTypeCacheKey_Overrides_BaseOverrides ()
    {
      var overrider1 = typeof (DerivedClassOverridingMixinMethod).GetMethod ("M1");
      var overrider2 = typeof (DerivedDerivedClassOverridingMixinMethod).GetMethod ("M2");

      var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (DerivedDerivedDerivedClassOverridingMixinMethod)).Mixins[typeof (MixinWithMethodsOverriddenByDifferentClasses)];
      var key = Tuple.NewTuple (typeof (MixinWithMethodsOverriddenByDifferentClasses), new SetBasedCacheKey<MethodInfo> (overrider1, overrider2));
      Assert.That (definition.GetConcreteMixinTypeCacheKey (), Is.EqualTo (key));
    }

    [Test]
    public void GetOrderRelevateDependencies()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseType3> ()
          .AddMixin<BT3Mixin1> ().WithDependency<NullMixin>()
          .AddMixin<NullMixin>().EnterScope ())
      {
        var definition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
        var dependencies = definition.GetOrderRelevantDependencies ().ToArray();
        Assert.That (dependencies, Is.EquivalentTo (new DependencyDefinitionBase[] { definition.BaseDependencies[0], definition.MixinDependencies[0] }));
      }
    }

    [Test]
    public void ChildSpecificAccept ()
    {
      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor>();
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      
      var interfaceIntroduction = DefinitionObjectMother.CreateInterfaceIntroductionDefinition(definition);
      var nonInterfaceIntroduction = DefinitionObjectMother.CreateNonInterfaceIntroductionDefinition(definition);
      var attributeIntroduction = DefinitionObjectMother.CreateAttributeIntroductionDefinition(definition);
      var nonAttributeIntroduction = DefinitionObjectMother.CreateNonAttributeIntroductionDefinition(definition);
      var suppressedAttributeIntroduction = DefinitionObjectMother.CreateSuppressedAttributeIntroductionDefinition(definition);
      var thisDependency = DefinitionObjectMother.CreateThisDependencyDefinition(definition);
      var baseDependency = DefinitionObjectMother.CreateBaseDependencyDefinition(definition);
      var mixinDependency = DefinitionObjectMother.CreateMixinDependencyDefinition(definition);

      using (visitorMock.GetMockRepository ().Ordered ())
      {
        visitorMock.Expect (mock => mock.Visit (definition));
        visitorMock.Expect (mock => mock.Visit (interfaceIntroduction));
        visitorMock.Expect (mock => mock.Visit (nonInterfaceIntroduction));
        visitorMock.Expect (mock => mock.Visit (attributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (nonAttributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (suppressedAttributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (thisDependency));
        visitorMock.Expect (mock => mock.Visit (baseDependency));
        visitorMock.Expect (mock => mock.Visit (mixinDependency));
      }

      visitorMock.Replay ();
      PrivateInvoke.InvokeNonPublicMethod (definition, "ChildSpecificAccept", visitorMock);
      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetAllOverrides()
    {
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      
      var methodOverride = DefinitionObjectMother.CreateMethodDefinition (definition, definition.Type.GetMethod ("ToString"));
      var overriddenMethod = DefinitionObjectMother.CreateMethodDefinition (definition.TargetClass, definition.Type.GetMethod ("ToString"));
      DefinitionObjectMother.DeclareOverride(methodOverride, overriddenMethod);

      var propertyOverride = DefinitionObjectMother.CreatePropertyDefinition (definition, typeof (DateTime).GetProperty ("Now"));
      var overriddenProperty = DefinitionObjectMother.CreatePropertyDefinition (definition.TargetClass, typeof (DateTime).GetProperty ("Now"));
      DefinitionObjectMother.DeclareOverride (propertyOverride, overriddenProperty);

      var eventOverride = DefinitionObjectMother.CreateEventDefinition (definition, typeof (AppDomain).GetEvent ("ProcessExit"));
      var overriddenEvent = DefinitionObjectMother.CreateEventDefinition (definition.TargetClass, typeof (AppDomain).GetEvent ("ProcessExit"));
      DefinitionObjectMother.DeclareOverride (eventOverride, overriddenEvent);

      var nonOverride = DefinitionObjectMother.CreateMethodDefinition (definition, definition.Type.GetMethod ("GetHashCode"));

      var overrides = definition.GetAllOverrides ().ToArray();
      Assert.That (overrides, Is.EquivalentTo (new MemberDefinitionBase[] { methodOverride, propertyOverride, eventOverride }));
      Assert.That (overrides, List.Not.Contains (nonOverride));
    }
  }
}