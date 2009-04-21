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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWorks()
    {
      var typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
                                                "GeneratedTypeInConfigurationTest.GeneratedMixinTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins (generatedType).EnterScope())
      {
        object instance = ObjectFactory.Create (typeof (NullTarget), ParamList.Empty);
        Assert.IsNotNull (Mixin.Get (generatedType, instance));
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks()
    {
      var typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
                                                "GeneratedTypeInConfigurationTest.GeneratedTargetTypeWorks", typeof (object));
      Type generatedType = typeEmitter.BuildType();

      using (MixinConfiguration.BuildFromActive().ForClass (generatedType).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        object instance = ObjectFactory.Create (generatedType, ParamList.Empty);
        Assert.IsNotNull (Mixin.Get (typeof (NullMixin), instance));
      }
    }
  }
}
