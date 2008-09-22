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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class MixinPropertyFinderTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindPropertyInfos_IncludeBasePropertiesTrue ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (TargetClassA), true);

      Assert.That (EnumerableUtility.ToArray (
          propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (TargetClassA)))),
          Is.EquivalentTo (
              new []
                  {
                      GetProperty (typeof (MixinBase), "P0a"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_IncludeBasePropertiesFalse ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (TargetClassA), false);

      Assert.That (EnumerableUtility.ToArray (
          propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (TargetClassA)))),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerived ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (TargetClassB), false);

      Assert.That (EnumerableUtility.ToArray (
          propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (TargetClassB)))),
          Is.EquivalentTo (
              new []
                  {
                      GetProperty (typeof (MixinB), "P6"),
                      GetProperty (typeof (MixinE), "P9"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerivedMixinNotOnBase ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (TargetClassC), false);

      Assert.That (EnumerableUtility.ToArray (
          propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (TargetClassC)))),
          Is.EquivalentTo (
              new []
                  {
                      GetProperty (typeof (DerivedMixinNotOnBase), "DerivedMixinProperty"),
                      GetProperty (typeof (MixinNotOnBase), "MixinProperty"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_WithDiamondShapedInheritance ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (DiamondTarget), false);

      Assert.That (EnumerableUtility.ToArray (
          propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (DiamondTarget)))),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (DiamondBase), "PBase"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForMixinAppliedAboveInheritanceRoot ()
    {
      MixinPropertyFinder propertyFinder = CreatePropertyFinder (typeof (InheritanceRootInheritingPersistentMixin), true);

      Assert.That (
          EnumerableUtility.ToArray (
              propertyFinder.FindPropertyInfosOnMixins (CreateReflectionBasedClassDefinition (typeof (InheritanceRootInheritingPersistentMixin)))),
          Is.EquivalentTo (
              new[]
                {
                    GetProperty (typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentProperty"),
                    GetProperty (typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot), "PersistentRelationProperty")
                }));
    }

    private MixinPropertyFinder CreatePropertyFinder (Type type, bool includeBaseProperties)
    {
      return new MixinPropertyFinder (typeof (StubPropertyFinderBase), new PersistentMixinFinder (type, includeBaseProperties), includeBaseProperties,
          new ReflectionBasedNameResolver ());
    }
  }
}
