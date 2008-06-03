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
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class MixedMixinTest
  {
    public class TargetClass
    {
    }

    [Extends (typeof (TargetClass))]
    public class One
    {
    }

    [Extends (typeof (One))]
    public class Two
    {
    }

    [Test]
    public void MixinOnMixinDoesNotInfluenceTargetClass ()
    {
      ClassContext c1;
      ClassContext c2;

      using (MixinConfiguration.BuildFromActive().ForClass<TargetClass> ().Clear().AddMixins (typeof (One)).EnterScope())
      {
        c1 = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass));
        using (MixinConfiguration.BuildFromActive().ForClass<One> ().Clear().AddMixins (typeof (Two)).EnterScope())
        {
          c2 = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass));
        }
      }

      Assert.AreEqual (c1, c2);

      TargetClassDefinition targetClass = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (TargetClass));
      Assert.AreEqual (typeof (TargetClass), targetClass.Type);
      Assert.AreEqual (1, targetClass.Mixins.Count);
      Assert.AreEqual (typeof (One), targetClass.Mixins[0].Type);
    }

    [Test]
    public void MixinOnMixinYieldsTargetClassDefinitionForMixin ()
    {
      TargetClassDefinition targetClassForMixin = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (One));
      Assert.AreEqual (typeof (One), targetClassForMixin.Type);
      Assert.AreEqual (1, targetClassForMixin.Mixins.Count);
      Assert.AreEqual (typeof (Two), targetClassForMixin.Mixins[0].Type);
    }
  }
}
