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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTest : CodeGenerationBaseTest
  {
    [Test]
    public void SignedBaseClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (object)).Clear().EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (object));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.True);
      }
    }

    [Test]
    public void UnsignedBaseClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (BaseType1)).Clear().EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.False);
      }
    }

    [Test]
    public void SignedBaseClassSignedMixinGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (List<int>)).Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (List<int>));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.True);
      }
    }

    [Test]
    public void UnsignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.False);
      }
    }

    [Test]
    public void UnsignedBaseClassSignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (object)).EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (BaseType1));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.False);
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Type concreteType = TypeGenerationHelper.ForceTypeGeneration (typeof (NullTarget));
        Assert.That (MixinTypeUtility.IsGeneratedConcreteMixedType (concreteType), Is.True);
        Assert.That (ReflectionUtility.IsAssemblySigned (concreteType.Assembly), Is.False);
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinWithOverride ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingToString> ().Clear().AddMixins (typeof (MixinOverridingToString)).EnterScope())
      {
        object instance = ObjectFactory.Create<ClassOverridingToString>(ParamList.Empty);
        Assert.That (instance.ToString(), Is.EqualTo ("Overridden: ClassOverridingToString"));
      }
    }
  }
}
