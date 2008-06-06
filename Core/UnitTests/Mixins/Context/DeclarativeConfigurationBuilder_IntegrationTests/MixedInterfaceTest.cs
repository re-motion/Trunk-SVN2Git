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
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class MixedInterfaceTest
  {
    [Test]
    public void MixedInterface_GetsClassContext_ViaUses ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void MixedInterface_GetsClassContext_ViaExtends ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (IMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void ImplementingClass_InheritsMixins ()
    {
      ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (MixinExtendingMixedInterface)));
    }

    [Test]
    public void Definition_ForImplementingClass ()
    {
      Assert.IsTrue (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithMixedInterface)).Mixins.ContainsKey (typeof (NullMixin)));
    }
  }
}
