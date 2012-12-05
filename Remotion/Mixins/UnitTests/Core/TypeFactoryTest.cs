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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class TypeFactoryTest
  {
    [Test]
    public void GetConcreteType_NoTypeGeneratedIfNoConfig ()
    {
      Assert.That (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)), Is.False);
      Assert.That (TypeFactory.GetConcreteType (typeof (object)), Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetConcreteType_NoTypeGenerated_IfGeneratedTypeIsGiven ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.That (TypeFactory.GetConcreteType (concreteType), Is.SameAs (concreteType));
    }

    [Test]
    public void InitializeUnconstructedInstance ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) FormatterServices.GetSafeUninitializedObject (concreteType);
      TypeFactory.InitializeUnconstructedInstance (bt3 as IMixinTarget);
      BT3Mixin1 bt3m1 = Mixin.Get<BT3Mixin1> (bt3);
      Assert.That (bt3m1, Is.Not.Null, "Mixin must have been created");
      Assert.That (bt3m1.Target, Is.SameAs (bt3), "Mixin must have been initialized");
    }
  }
}
