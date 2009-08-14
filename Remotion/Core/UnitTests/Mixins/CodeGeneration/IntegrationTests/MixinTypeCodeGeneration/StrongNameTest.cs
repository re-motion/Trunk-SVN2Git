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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
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
      using (MixinConfiguration.BuildFromActive().ForClass<ArrayList> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ArrayList));
        MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0];

        Type generatedType = ConcreteTypeBuilder.Current
            .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier())
            .GeneratedType;

        Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.True);
      }
    }

    [Test]
    public void UnsignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (BaseType1));
        MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0];

        Type generatedType = ConcreteTypeBuilder.Current
            .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier())
            .GeneratedType;

        Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);
      }
    }

    [Test]
    public void SignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<UnsignedClass> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (UnsignedClass));
        MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0];

        Type generatedType = ConcreteTypeBuilder.Current
            .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier())
            .GeneratedType;
        Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);

        Assert.That (Mixin.Get<EquatableMixin<UnsignedClass>> (ObjectFactory.Create<UnsignedClass> (ParamList.Empty)).ToString (), Is.EqualTo ("Overridden"));
      }
    }

    [Test]
    public void UnsignedMixinWithSignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (NullTarget));
        MixinDefinition mixinDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (requestingClass).Mixins[0];

        Type generatedType = ConcreteTypeBuilder.Current
            .GetConcreteMixinType (requestingClass, mixinDefinition.GetConcreteMixinTypeIdentifier())
            .GeneratedType;


        Assert.That (ReflectionUtility.IsAssemblySigned (generatedType.Assembly), Is.False);
        Assert.That (ObjectFactory.Create<NullTarget> (ParamList.Empty).ToString (), Is.EqualTo ("Overridden"));
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
