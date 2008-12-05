// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class InheritanceDefinitionBuilderTest
  {
    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.ReceivedInterfaces.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedFaceDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinFaceDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredFaceTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinFaceDependingOnInheritedInterface)];
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.ThisDependencies.ContainsKey (typeof (IMixinIII4)));
    }

    [Test]
    public void InheritedBaseDependencies ()
    {
      TargetClassDefinition bt1 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (
              typeof (BaseType1),
              typeof (MixinBaseDependingOnInheritedInterface),
              typeof (MixinIntroducingInheritedInterface));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (bt1.RequiredBaseCallTypes.ContainsKey (typeof (IMixinIII4)));

      MixinDefinition m1 = bt1.Mixins[typeof (MixinBaseDependingOnInheritedInterface)];
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII1)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII2)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII3)));
      Assert.IsTrue (m1.BaseDependencies.ContainsKey (typeof (IMixinIII4)));
    }


  }
}
