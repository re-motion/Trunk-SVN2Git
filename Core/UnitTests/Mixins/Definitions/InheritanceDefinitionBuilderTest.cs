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
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTest
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.IntroducedInterfaces.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII4)));
    }


  }
}
