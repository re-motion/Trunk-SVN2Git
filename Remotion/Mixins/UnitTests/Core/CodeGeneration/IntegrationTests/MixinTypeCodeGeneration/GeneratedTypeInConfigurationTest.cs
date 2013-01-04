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
using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration.TestDomain;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      var moduleScope = ConcreteTypeBuilderTestHelper.GetCurrentModuleManager().Scope;
      var typeEmitter = new CustomClassEmitter (moduleScope, "GeneratedMixinTypeWithOverriddenMethodWorks", typeof (Mixin<object>));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMethod> ().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod), ParamList.Empty);
        Assert.That (Mixin.Get (generatedType, instance).ToString (), Is.EqualTo ("Overridden!"));
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      var typeEmitter = new CustomClassEmitter (ConcreteTypeBuilderTestHelper.GetCurrentModuleManager ().Scope,
          "GeneratedTargetTypeOverridingMixinMethodWorks", typeof (object));
      typeEmitter.CreateMethod ("ToString", MethodAttributes.Public, typeof (string), new Type[0])
          .ImplementByReturning (new ConstReference ("Generated _and_ overridden").ToExpression ())
          .AddCustomAttribute (new CustomAttributeBuilder (typeof (OverrideMixinAttribute).GetConstructor (Type.EmptyTypes), new object[0]));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (SimpleMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType, ParamList.Empty);
        Assert.That (Mixin.Get<SimpleMixin> (instance).ToString (), Is.EqualTo ("Generated _and_ overridden"));
      }
    }
  }
}
