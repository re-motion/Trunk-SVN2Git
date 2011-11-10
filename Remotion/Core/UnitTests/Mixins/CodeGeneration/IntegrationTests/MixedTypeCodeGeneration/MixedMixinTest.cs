// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.TestDomain;
using ClassOverridingSingleMixinMethod=Remotion.UnitTests.Mixins.CodeGeneration.TestDomain.ClassOverridingSingleMixinMethod;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class MixedMixinTest : CodeGenerationBaseTest
  {
    [Test]
    public void DoubleMixinOverrides_CreateMixinInstance ()
    {
      MixinMixingClass instance = ObjectFactory.Create<MixinMixingClass> (ParamList.Empty);
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (instance));
    }

    [Test]
    public void DoubleMixinOverrides_CreateClassInstance ()
    {
      ClassWithMixedMixin instance = ObjectFactory.Create<ClassWithMixedMixin> (ParamList.Empty);
      Assert.IsNotNull (Mixin.Get<MixinMixingClass> (instance));
      Assert.IsNotNull (Mixin.Get<MixinMixingMixin> (Mixin.Get<MixinMixingClass> (instance)));

      Assert.AreEqual ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)", instance.StringMethod (3));
    }

    [Test]
    public void MixedMixin_OverriddenByTargetClass ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassOverridingSingleMixinMethod>().AddMixin<MixinWithOverridableMember>()
          .ForClass<MixinWithOverridableMember>().AddMixin<MixinOverridingToString>()
          .EnterScope())
      {
        var instance = ObjectFactory.Create<ClassOverridingSingleMixinMethod>(ParamList.Empty);
        Assert.That (Mixin.Get<MixinWithOverridableMember> (instance).ToString(), Is.StringStarting("Overridden: "));
      }
    }
  }
}
