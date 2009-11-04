// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration.TestDomain;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      var moduleScope = ((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope;
      var typeEmitter = new CustomClassEmitter (moduleScope, "GeneratedMixinTypeWithOverriddenMethodWorks", typeof (Mixin<object>));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMethod> ().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod), ParamList.Empty);
        Assert.AreEqual ("Overridden!", Mixin.Get (generatedType, instance).ToString ());
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      var typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedTargetTypeOverridingMixinMethodWorks", typeof (object));
      typeEmitter.CreateMethod ("ToString", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturning (new ConstReference ("Generated _and_ overridden").ToExpression ())
          .AddCustomAttribute (new CustomAttributeBuilder (typeof (OverrideMixinAttribute).GetConstructor (Type.EmptyTypes), new object[0]));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (SimpleMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType, ParamList.Empty);
        Assert.AreEqual ("Generated _and_ overridden", Mixin.Get<SimpleMixin> (instance).ToString ());
      }
    }
  }
}
