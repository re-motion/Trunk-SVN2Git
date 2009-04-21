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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTest : CodeGenerationBaseTest
  {
    [Test]
    public void SignedBaseClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (object)).Clear().EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BaseType1)).Clear().EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassSignedMixinGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (List<int>)).Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (List<int>), GenerationPolicy.ForceGeneration);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void UnsignedBaseClassSignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (NullTarget), GenerationPolicy.ForceGeneration);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (concreteType.Assembly));
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinWithOverride ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingToString> ().Clear().AddMixins (typeof (MixinOverridingToString)).EnterScope())
      {
        object instance = ObjectFactory.Create<ClassOverridingToString>(ParamList.Empty);
        Assert.AreEqual ("Overridden: ClassOverridingToString", instance.ToString());
      }
    }
  }
}
