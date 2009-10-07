// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Samples;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTest : CodeGenerationBaseTest
  {
    [Test]
    public void SignedMixinWithSignedTargetClassGeneratedIntoSignedAssembly ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ArrayList), typeof (EquatableMixin<>));
      Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.True);
    }

    [Test]
    public void UnsignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (BaseType1), typeof (UnsignedMixin));
      Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);
    }

    [Test]
    public void SignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (UnsignedClass), typeof (EquatableMixin<>));
      Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);

      using (MixinConfiguration.BuildFromActive().ForClass<UnsignedClass>().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        var mixedInstance = ObjectFactory.Create<UnsignedClass> (ParamList.Empty);
        var mixinInstance = Mixin.Get<EquatableMixin<UnsignedClass>> (mixedInstance);
        Assert.That (mixinInstance.ToString(), Is.EqualTo ("Overridden"));
      }
    }

    [Test]
    public void UnsignedMixinWithSignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (NullTarget), typeof (UnsignedMixin));
      Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);
        
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (UnsignedMixin)).EnterScope ())
      {
        var mixedInstance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Assert.That (mixedInstance.ToString (), Is.EqualTo ("Overridden"));
      }
    }
  }

  public class UnsignedMixin : Mixin<object>
  {
    [OverrideTarget]
    protected new string ToString ()
    {
      return "Overridden";
    }
  }

  public class UnsignedClass
  {
    [OverrideMixin]
    public new string ToString ()
    {
      return "Overridden";
    }
  }
}