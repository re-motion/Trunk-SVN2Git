/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using NUnit.Framework;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class MixedMixinTest : CodeGenerationBaseTest
  {
    [Test]
    public void DoubleMixinOverrides_CreateMixinInstance ()
    {
      MixinMixingClass instance = ObjectFactory.Create<MixinMixingClass> ().With ();
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (instance));
    }

    [Test]
    public void DoubleMixinOverrides_CreateClassInstance ()
    {
      ClassWithMixedMixin instance = ObjectFactory.Create<ClassWithMixedMixin> ().With();
      Assert.IsNotNull (Mixin.Get<MixinMixingClass> (instance));
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (Mixin.Get<MixinMixingClass> (instance)));

      Assert.AreEqual ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)", instance.StringMethod (3));
    }

    [Test]
    public void MixedMixin_OverriddenByTargetClass ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<TargetClassOverridingMixinMember>().AddMixin<MixinWithOverridableMember>()
          .ForClass<MixinWithOverridableMember>().AddMixin<MixinOverridingToString>()
          .EnterScope())
      {
        var instance = ObjectFactory.Create<TargetClassOverridingMixinMember>().With();
        Assert.That (Mixin.Get<MixinWithOverridableMember> (instance).ToString(), NUnit.Framework.SyntaxHelpers.Text.StartsWith ("Overridden: "));
      }
    }
  }
}
