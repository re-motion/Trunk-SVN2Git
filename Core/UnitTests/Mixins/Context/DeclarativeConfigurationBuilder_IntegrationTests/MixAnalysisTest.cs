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
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixAnalysisTest
  {
    [Test]
    public void MixAttributeIsAnalyzed ()
    {
      Assert.IsTrue (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (MixinForGlobalMix)));
      Assert.IsTrue (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (AdditionalDependencyForGlobalMix)));
    }

    [Test]
    public void AdditionalDependenciesAreAnalyzed ()
    {
      Assert.IsTrue (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins[typeof (MixinForGlobalMix)]
          .MixinDependencies.ContainsKey (typeof (AdditionalDependencyForGlobalMix)));
    }

    [Test]
    public void SuppressedMixinsAreAnalyzed ()
    {
      Assert.IsFalse (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClassForGlobalMix)).Mixins
          .ContainsKey (typeof (SuppressedMixinForGlobalMix)));
    }
  }
}
