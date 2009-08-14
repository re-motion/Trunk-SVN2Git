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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class TypeFactoryTest
  {
    [Test]
    public void GetConcreteType_NoTypeGeneratedIfNoConfig_ByDefault ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      Assert.That (TypeFactory.GetConcreteType (typeof (object)), Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetConcreteType_NoTypeGenerated_IfGeneratedTypeIsGivenByDefault ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (TypeFactory.GetConcreteType (concreteType), Is.SameAs (concreteType));
    }

    [Test]
    public void GetConcreteType_TypeGeneratedIfNoConfigViaPolicy ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.That (concreteType, Is.Not.SameAs (typeof (object)));
      Assert.That (concreteType.BaseType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetConcreteType_TypeGeneratedIfGeneratedTypeIsGivenViaPolicy ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type concreteType2 = TypeFactory.GetConcreteType (concreteType, GenerationPolicy.ForceGeneration);
      Assert.That (concreteType2, Is.Not.SameAs (concreteType));
      Assert.That (concreteType2.BaseType, Is.SameAs (concreteType));
    }

    [Test]
    public void InitializeUnconstructedInstance ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) FormatterServices.GetSafeUninitializedObject (concreteType);
      TypeFactory.InitializeUnconstructedInstance (bt3 as IMixinTarget);
      BT3Mixin1 bt3m1 = Mixin.Get<BT3Mixin1> (bt3);
      Assert.That (bt3m1, Is.Not.Null, "Mixin must have been created");
      Assert.That (bt3m1.This, Is.SameAs (bt3), "Mixin must have been initialized");
    }
  }
}
