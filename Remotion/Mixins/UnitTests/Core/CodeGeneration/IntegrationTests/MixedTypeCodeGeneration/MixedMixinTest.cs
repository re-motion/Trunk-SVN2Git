// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using ClassOverridingSingleMixinMethod = Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain.ClassOverridingSingleMixinMethod;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class MixedMixinTest : CodeGenerationBaseTest
  {
    [Test]
    public void DoubleMixinOverrides_CreateMixinInstance ()
    {
      MixinMixingClass instance = ObjectFactory.Create<MixinMixingClass> (ParamList.Empty);
      Assert.That (Mixin.Get<MixinMixingMixin> (instance), Is.Not.Null);
    }

    [Test]
    public void DoubleMixinOverrides_CreateClassInstance ()
    {
      ClassWithMixedMixin instance = ObjectFactory.Create<ClassWithMixedMixin> (ParamList.Empty);
      Assert.That (Mixin.Get<MixinMixingClass> (instance), Is.Not.Null);
      Assert.That (Mixin.Get<MixinMixingMixin> (Mixin.Get<MixinMixingClass> (instance)), Is.Not.Null);

      Assert.That (instance.StringMethod (3), Is.EqualTo ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)"));
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

    [Test]
    [ExpectedException (typeof (SerializationException))]
    public void MixedMixin_Serialization ()
    {
      var instance = ObjectFactory.Create<ClassWithMixedMixin> (ParamList.Empty);

      var deserialized = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserialized.StringMethod (3), Is.EqualTo ("MixinMixingMixin-MixinMixingClass-ClassWithMixedMixin.StringMethod (3)"));
    }
  }
}
