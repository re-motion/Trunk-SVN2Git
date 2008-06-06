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
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class DependencyTest : CodeGenerationBaseTest
  {
    [Test]
    public void CircularThisDependenciesWork ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithCircularThisDependency1), typeof (MixinWithCircularThisDependency2)).EnterScope())
      {
        object o = ObjectFactory.Create<NullTarget> ().With ();
        ICircular2 c1 = (ICircular2) o;
        Assert.AreEqual ("MixinWithCircularThisDependency2.Circular12-MixinWithCircularThisDependency1.Circular1-"
            + "MixinWithCircularThisDependency2.Circular2", c1.Circular12 ());
      }
    }

    [Test]
    public void ThisCallToClassImplementingInternalInterface ()
    {
      ClassImplementingInternalInterface ciii = ObjectFactory.Create<ClassImplementingInternalInterface> ().With ();
      MixinWithClassFaceImplementingInternalInterface mixin = Mixin.Get<MixinWithClassFaceImplementingInternalInterface> (ciii);
      Assert.AreEqual ("ClassImplementingInternalInterface.Foo", mixin.GetStringViaThis ());
    }

    [Test]
    public void ThisCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> ().With ();
      MixinWithIndirectRequirements mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.AreEqual ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
          + "ClassImplementingIndirectRequirements.Method3", mixin.GetStuffViaThis ());
    }
  }
}
