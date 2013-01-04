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
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWorks()
    {
      var moduleManager = ConcreteTypeBuilderTestHelper.GetCurrentModuleManager ();
      var typeEmitter = new CustomClassEmitter (moduleManager.Scope, "GeneratedTypeInConfigurationTest.GeneratedMixinTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (NullTarget), ParamList.Empty);
        Assert.That (Mixin.Get (generatedType, instance), Is.Not.Null);
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks()
    {
      var moduleManager = ConcreteTypeBuilderTestHelper.GetCurrentModuleManager ();
      var typeEmitter = new CustomClassEmitter (moduleManager.Scope, "GeneratedTypeInConfigurationTest.GeneratedTargetTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType, ParamList.Empty);
        Assert.That (Mixin.Get (typeof (NullMixin), instance), Is.Not.Null);
      }
    }
  }
}
